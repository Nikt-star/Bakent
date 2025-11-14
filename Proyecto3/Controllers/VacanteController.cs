// Proyecto3/Controllers/VacanteController.cs

using Microsoft.AspNetCore.Mvc;
using Proyecto3.Core.Entities;
using Proyecto3.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class VacanteController : ControllerBase
{
    private readonly IVacanteRepository _vacanteRepository;

    // Inyección del Repositorio
    public VacanteController(IVacanteRepository vacanteRepository)
    {
        _vacanteRepository = vacanteRepository;
    }

    // [POST] api/vacante (UC-V1: Crear)
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(PerfilesVacante))]
    public async Task<IActionResult> Post([FromBody] PerfilesVacante vacante)
    {
        // Nota: Asegúrate de que los RequisitosVacante vengan incluidos en el objeto JSON.
        var nuevaVacante = await _vacanteRepository.AddAsync(vacante);
        return CreatedAtAction(nameof(GetById), new { id = nuevaVacante.VacanteID }, nuevaVacante);
    }

    // [GET] api/vacante (UC-V2: Listar)
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<PerfilesVacante>))]
    public async Task<IActionResult> GetAll()
    {
        var vacantes = await _vacanteRepository.GetAllAsync();
        return Ok(vacantes);
    }

    // [GET] api/vacante/{id} (UC-V2: Detalles)
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(PerfilesVacante))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        // Usa el método que carga los requisitos para mostrar el perfil completo
        var vacante = await _vacanteRepository.GetByIdWithRequisitosAsync(id);
        if (vacante == null) return NotFound();
        return Ok(vacante);
    }

    // [PUT] api/vacante/{id} (UC-V1: Actualizar)
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Put(int id, [FromBody] PerfilesVacante vacante)
    {
        if (id != vacante.VacanteID) return BadRequest();

        // La validación de existencia es manejada por el repositorio/EF Core
        await _vacanteRepository.UpdateAsync(vacante);
        return NoContent();
    }

    // [DELETE] api/vacante/{id} (UC-V1: Eliminar)
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        await _vacanteRepository.DeleteAsync(id);
        return NoContent();
    }
}