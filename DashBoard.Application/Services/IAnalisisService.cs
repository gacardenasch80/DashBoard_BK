using DashBoard.Application.DTOs;

namespace DashBoard.Application.Services;

public interface IAnalisisService
{
    Task<IEnumerable<AnalisisDto>> GetAllAsync(Guid? usuarioId = null);
    Task<AnalisisResponseDto> GetByIdAsync(Guid id);
    Task<AnalisisResponseDto> CreateAsync(CreateAnalisisDto dto, Guid usuarioId);
    Task<AnalisisResponseDto> UpdateAsync(Guid id, UpdateAnalisisDto dto);
    Task DeleteAsync(Guid id);
}
