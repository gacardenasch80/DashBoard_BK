using AutoMapper;
using DashBoard.Application.DTOs;
using DashBoard.Core.Entities;
using DashBoard.Core.Interfaces;

namespace DashBoard.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UsuarioService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
    {
        var usuarios = await _unitOfWork.Usuarios.GetAllAsync();
        return _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);
    }

    public async Task<UsuarioDto> GetByIdAsync(Guid id)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        if (usuario == null)
            throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");

        return _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto)
    {
        if (await _unitOfWork.Usuarios.ExistsAsync(u => u.Username == dto.Username))
            throw new InvalidOperationException($"El username '{dto.Username}' ya existe");

        var usuario = _mapper.Map<Usuario>(dto);
        usuario.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        usuario.FechaCreacion = DateTime.UtcNow;

        await _unitOfWork.Usuarios.AddAsync(usuario);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<UsuarioDto> UpdateAsync(Guid id, UpdateUsuarioDto dto)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        if (usuario == null)
            throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");

        if (!string.IsNullOrEmpty(dto.Nombres))
            usuario.Nombres = dto.Nombres;

        if (!string.IsNullOrEmpty(dto.Apellidos))
            usuario.Apellidos = dto.Apellidos;

        if (!string.IsNullOrEmpty(dto.Password))
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        usuario.FechaModificacion = DateTime.UtcNow;

        await _unitOfWork.Usuarios.UpdateAsync(usuario);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task DeleteAsync(Guid id)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        if (usuario == null)
            throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");

        await _unitOfWork.Usuarios.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
    }
}
