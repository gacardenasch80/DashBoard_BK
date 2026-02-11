using System.Text.Json;
using AutoMapper;
using DashBoard.Application.DTOs;
using DashBoard.Core.Entities;
using DashBoard.Core.Interfaces;

namespace DashBoard.Application.Services;

public class AnalisisService : IAnalisisService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AnalisisService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AnalisisDto>> GetAllAsync(Guid? usuarioId = null)
    {
        IEnumerable<Analisis> analisis;

        if (usuarioId.HasValue)
            analisis = await _unitOfWork.Analisis.FindAsync(a => a.UsuarioId == usuarioId.Value);
        else
            analisis = await _unitOfWork.Analisis.GetAllAsync();

        return _mapper.Map<IEnumerable<AnalisisDto>>(analisis);
    }

    public async Task<AnalisisResponseDto> GetByIdAsync(Guid id)
    {
        var analisis = await _unitOfWork.Analisis.GetByIdAsync(id);
        if (analisis == null)
            throw new KeyNotFoundException($"Análisis con ID {id} no encontrado");

        return _mapper.Map<AnalisisResponseDto>(analisis);
    }

    public async Task<AnalisisResponseDto> CreateAsync(CreateAnalisisDto dto, Guid usuarioId)
    {
        var analisis = new Analisis
        {
            Id = Guid.NewGuid(),
            NombreAnalisis = dto.NombreAnalisis,
            UsuarioId = usuarioId,
            JsonData = JsonSerializer.Serialize(dto.JsonData),
            FiltrosAplicados = dto.FiltrosAplicados != null ? JsonSerializer.Serialize(dto.FiltrosAplicados) : null,
            TotalFacturas = dto.TotalFacturas,
            ValorTotal = dto.ValorTotal,
            FechaCreacion = DateTime.UtcNow
        };

        await _unitOfWork.Analisis.AddAsync(analisis);
        await _unitOfWork.CompleteAsync();

        return await GetByIdAsync(analisis.Id);
    }

    public async Task<AnalisisResponseDto> UpdateAsync(Guid id, UpdateAnalisisDto dto)
    {
        var analisis = await _unitOfWork.Analisis.GetByIdAsync(id);
        if (analisis == null)
            throw new KeyNotFoundException($"Análisis con ID {id} no encontrado");

        // Actualizar solo los campos proporcionados
        if (dto.NombreAnalisis != null)
            analisis.NombreAnalisis = dto.NombreAnalisis;

        if (dto.FiltrosAplicados != null)
            analisis.FiltrosAplicados = JsonSerializer.Serialize(dto.FiltrosAplicados);

        if (dto.TotalFacturas.HasValue)
            analisis.TotalFacturas = dto.TotalFacturas.Value;

        if (dto.ValorTotal.HasValue)
            analisis.ValorTotal = dto.ValorTotal.Value;

        await _unitOfWork.CompleteAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var analisis = await _unitOfWork.Analisis.GetByIdAsync(id);
        if (analisis == null)
            throw new KeyNotFoundException($"Análisis con ID {id} no encontrado");

        await _unitOfWork.Analisis.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
    }
}
