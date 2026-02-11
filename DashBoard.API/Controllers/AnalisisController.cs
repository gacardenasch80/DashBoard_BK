using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DashBoard.Application.DTOs;
using DashBoard.Application.Services;
using System.Security.Claims;

namespace DashBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalisisController : ControllerBase
{
    private readonly IAnalisisService _analisisService;

    public AnalisisController(IAnalisisService analisisService)
    {
        _analisisService = analisisService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool soloMios = false)
    {
        try
        {
            Guid? usuarioId = null;
            if (soloMios)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out var parsedId))
                    usuarioId = parsedId;
            }

            var analisis = await _analisisService.GetAllAsync(usuarioId);
            return Ok(analisis);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var analisis = await _analisisService.GetByIdAsync(id);
            return Ok(analisis);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnalisisDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var usuarioId))
                return BadRequest(new { message = "Usuario no válido" });

            var analisis = await _analisisService.CreateAsync(dto, usuarioId);
            return CreatedAtAction(nameof(GetById), new { id = analisis.Id }, analisis);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAnalisisDto dto)
    {
        try
        {
            var analisis = await _analisisService.UpdateAsync(id, dto);
            return Ok(analisis);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _analisisService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", details = ex.Message });
        }
    }
}
