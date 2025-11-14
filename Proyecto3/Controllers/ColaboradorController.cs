// Proyecto3/Controllers/ColaboradorController.cs

using Microsoft.AspNetCore.Mvc;
using Proyecto3.Core.Entities;
using Proyecto3.Core.Interfaces; // Usa la interfaz del servicio
using System.Collections.Generic;
using System.Threading.Tasks;
using Proyecto3.Core.Models;

[Route("api/[controller]")]
[ApiController]
public class ColaboradorController : ControllerBase
{
    private readonly IColaboradorService _colaboradorService;

    // Inyección de Dependencias del Servicio
    public ColaboradorController(IColaboradorService colaboradorService)
    {
        _colaboradorService = colaboradorService;
    }

    // UC-C1: POST para crear un nuevo Colaborador con habilidades y certificaciones
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Colaborador))]
    public async Task<IActionResult> Post([FromBody] ColaboradorCreateDto dto)
    {
        var colaborador = await _colaboradorService.CreateFromDtoAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = colaborador.ColaboradorID }, colaborador);
    }

    // GET para leer todos los Colaboradores
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Colaborador>))]
    public async Task<IActionResult> GetAll()
    {
        var colaboradores = await _colaboradorService.GetAllColaboradoresAsync();
        return Ok(colaboradores);
    }

    // UC-C2: GET para obtener detalles de un Colaborador
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Colaborador))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var colaborador = await _colaboradorService.GetColaboradorDetailsAsync(id);
            return Ok(colaborador);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Añadir una sola habilidad a un colaborador existente
    [HttpPost("{id}/habilidades")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> AddHabilidad(int id, [FromBody] HabilidadCreateDto dto)
    {
        var updateDto = new ColaboradorUpdateDto { Habilidades = new List<HabilidadCreateDto> { dto } };
        try
        {
            await _colaboradorService.AddHabilidadesAndCertsAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Añadir una sola certificación a un colaborador existente
    [HttpPost("{id}/certificaciones")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> AddCertificacion(int id, [FromBody] CertificacionCreateDto dto)
    {
        var updateDto = new ColaboradorUpdateDto { Certificaciones = new List<CertificacionCreateDto> { dto } };
        try
        {
            await _colaboradorService.AddHabilidadesAndCertsAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // Añadir habilidades o certificaciones a un colaborador existente (lista)
    [HttpPut("{id}/add")]
    public async Task<IActionResult> AddToColaborador(int id, [FromBody] ColaboradorUpdateDto dto)
    {
        try
        {
            await _colaboradorService.AddHabilidadesAndCertsAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // UC-C3: PUT para actualizar un Colaborador
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Put(int id, [FromBody] Colaborador colaborador)
    {
        if (id != colaborador.ColaboradorID)
        {
            return BadRequest(); // IDs no coinciden
        }

        try
        {
            await _colaboradorService.UpdateColaboradorAsync(colaborador);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        // Retorna 204 No Content, que es estándar para actualizaciones exitosas
        return NoContent();
    }

    // UC-C4: DELETE para eliminar un Colaborador
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        await _colaboradorService.DeleteColaboradorAsync(id);
        // Retorna 204 No Content
        return NoContent();
    }
}