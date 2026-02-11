using DashBoard.Core.Entities;
using DashBoard.Core.Interfaces;
using DashBoard.Infrastructure.Data;

namespace DashBoard.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Usuarios = new Repository<Usuario>(_context);
        Analisis = new Repository<Analisis>(_context);
    }

    public IRepository<Usuario> Usuarios { get; private set; }
    public IRepository<Analisis> Analisis { get; private set; }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context?.Dispose();
}
