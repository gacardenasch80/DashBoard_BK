using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DashBoard.Application.Mappings;
using DashBoard.Application.Services;
using DashBoard.Core.Interfaces;
using DashBoard.Infrastructure.Data;
using DashBoard.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CONFIGURACIÓN DE BASE DE DATOS
// ========================================

var useSqlServer = builder.Configuration.GetValue<bool>("DatabaseSettings:UseSqlServer");
var sqliteConnection = builder.Configuration.GetConnectionString("SqliteConnection") ?? "Data Source=./Database/dashboard.db";
var sqlServerConnection = builder.Configuration.GetConnectionString("SqlServerConnection");
var databasePath = builder.Configuration.GetValue<string>("DatabaseSettings:DatabasePath") ?? "./Database";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlServer && !string.IsNullOrEmpty(sqlServerConnection))
    {
        options.UseSqlServer(sqlServerConnection);
        Console.WriteLine("✅ Configurado para usar SQL Server");
    }
    else
    {
        // Crear carpeta de base de datos si no existe
        var fullPath = Path.IsPathRooted(databasePath) 
            ? databasePath 
            : Path.Combine(Directory.GetCurrentDirectory(), databasePath);
            
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
            Console.WriteLine($"📁 Carpeta creada: {fullPath}");
        }

        options.UseSqlite(sqliteConnection);
        Console.WriteLine($"✅ Configurado para usar SQLite: {sqliteConnection}");
    }
});

// ========================================
// CONFIGURACIÓN DE PAYLOADS GRANDES (SIN LÍMITES)
// ========================================

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue; // Sin límite práctico
    options.MemoryBufferThreshold = int.MaxValue;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.MaxDepth = 128; // Más profundidad para objetos anidados
        options.JsonSerializerOptions.DefaultBufferSize = 64 * 1024 * 1024; // 64MB buffer
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = null; // Sin límite
    serverOptions.Limits.MinRequestBodyDataRate = null;
    serverOptions.Limits.MinResponseDataRate = null;
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
});

// ========================================
// AUTOMAPPER
// ========================================

builder.Services.AddAutoMapper(typeof(MappingProfile));

// ========================================
// AUTENTICACIÓN JWT
// ========================================

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// ========================================
// CORS
// ========================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:5500" };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ========================================
// INYECCIÓN DE DEPENDENCIAS
// ========================================

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAnalisisService, AnalisisService>();

// ========================================
// SWAGGER
// ========================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DashBoard API",
        Version = "v1",
        Description = "API para gestión de análisis de facturas médicas con autenticación JWT",
        Contact = new OpenApiContact
        {
            Name = "Soporte Técnico",
            Email = "soporte@dashboard.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Ejemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ========================================
// CREAR BASE DE DATOS AUTOMÁTICAMENTE
// ========================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        Console.WriteLine("🔄 Inicializando base de datos...");

        // Crear la base de datos si no existe
        if (context.Database.EnsureCreated())
        {
            Console.WriteLine("✅ Base de datos creada exitosamente");
            Console.WriteLine("✅ Tablas creadas: Usuarios, Analisis");
            Console.WriteLine("✅ Usuario Admin insertado");
        }
        else
        {
            Console.WriteLine("ℹ️  Base de datos ya existe");
        }

        // Verificar que el usuario Admin existe
        var adminExists = context.Usuarios.Any(u => u.Username == "Admin");
        if (!adminExists)
        {
            Console.WriteLine("⚠️  Usuario Admin no encontrado, creando...");
            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            
            context.Usuarios.Add(new DashBoard.Core.Entities.Usuario
            {
                Id = adminId,
                Nombres = "Administrador",
                Apellidos = "Sistema",
                Username = "Admin",
                Password = hashedPassword,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            });
            
            context.SaveChanges();
            Console.WriteLine("✅ Usuario Admin creado");
        }

        Console.WriteLine("✅ Base de datos lista para usar");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al inicializar base de datos: {ex.Message}");
        Console.WriteLine($"   Detalle: {ex.InnerException?.Message}");
        Console.WriteLine($"   Stack: {ex.StackTrace}");
    }
}

// ========================================
// MIDDLEWARE
// ========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DashBoard API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("");
Console.WriteLine("════════════════════════════════════════════");
Console.WriteLine("🚀 DashBoard API Iniciada");
Console.WriteLine("════════════════════════════════════════════");
Console.WriteLine($"📁 Base de datos: {(useSqlServer ? "SQL Server" : "SQLite")}");
Console.WriteLine($"🔐 JWT Issuer: {jwtSettings["Issuer"]}");
Console.WriteLine($"📖 Swagger: {(app.Environment.IsDevelopment() ? "Habilitado en /" : "Deshabilitado")}");
Console.WriteLine("════════════════════════════════════════════");
Console.WriteLine("");
Console.WriteLine("👤 Usuario por defecto:");
Console.WriteLine("   Username: Admin");
Console.WriteLine("   Password: 123456");
Console.WriteLine("");

app.Run();
