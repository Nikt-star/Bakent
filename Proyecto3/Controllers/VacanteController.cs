// Proyecto3/Controllers/VacanteController.cs

using Microsoft.AspNetCore.Mvc;
using Proyecto3.Core.Entities;
using Proyecto3.Core.Interfaces;
using Proyecto3.Core.Models;
using Proyecto3.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class VacanteController : ControllerBase
{
    private readonly IVacanteRepository _vacanteRepository;
    private readonly TalentoInternoDbContext _context;

    public VacanteController(IVacanteRepository vacanteRepository, TalentoInternoDbContext context)
    {
        _vacanteRepository = vacanteRepository;
        _context = context;
    }

    // [POST] api/vacante - UC-V1: Registrar Nueva Vacante (Administración)
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(PerfilesVacante))]
    public async Task<IActionResult> Post([FromBody] PerfilesVacante vacante)
    {
        var nuevaVacante = await _vacanteRepository.AddAsync(vacante);
        return CreatedAtAction(nameof(GetById), new { id = nuevaVacante.VacanteID }, nuevaVacante);
    }

    // [GET] api/vacante - UC-V2: Listar Vacantes Activas
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<PerfilesVacante>))]
    public async Task<IActionResult> GetAll()
    {
        var vacantes = await _vacanteRepository.GetAllAsync();
        // Filtrar solo las activas
        var vacantesActivas = vacantes.Where(v => v.Activa).ToList();
        return Ok(vacantesActivas);
    }

    // [GET] api/vacante/{id} - UC-V2: Ver Detalles de Vacante
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(PerfilesVacante))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var vacante = await _vacanteRepository.GetByIdWithRequisitosAsync(id);
        if (vacante == null) 
            return NotFound(new { message = "Vacante no encontrada" });
        return Ok(vacante);
    }

    // [PUT] api/vacante/{id} - Actualizar Vacante
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Put(int id, [FromBody] PerfilesVacante vacante)
    {
        if (id != vacante.VacanteID) 
            return BadRequest(new { message = "El ID de la vacante no coincide" });

        await _vacanteRepository.UpdateAsync(vacante);
        return NoContent();
    }

    // [DELETE] api/vacante/{id} - Eliminar Vacante
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        await _vacanteRepository.DeleteAsync(id);
        return NoContent();
    }

    // ===== NUEVOS ENDPOINTS PARA APLICACIONES Y INTERÉS =====

    // [POST] api/vacante/{vacanteId}/aplicar - UC-V3: Aplicar a una Vacante
    [HttpPost("{vacanteId}/aplicar")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> AplicarAVacante(int vacanteId, [FromBody] Vacante_AplicacionCreateDto dto)
    {
        // Validar que la vacante existe
        var vacante = await _vacanteRepository.GetByIdWithRequisitosAsync(vacanteId);
        if (vacante == null)
            return NotFound(new { message = "Vacante no encontrada" });

        // Validar que el colaborador existe
        var colaborador = await _context.Colaboradores.FindAsync(dto.ColaboradorID);
        if (colaborador == null)
            return NotFound(new { message = "Colaborador no encontrado" });

        // Verificar que no exista una aplicación previa
        var aplicacionExistente = await _context.Set<Vacante_Aplicacion>()
            .FirstOrDefaultAsync(a => a.VacanteID == vacanteId && a.ColaboradorID == dto.ColaboradorID);
        
        if (aplicacionExistente != null)
            return Conflict(new { message = "El colaborador ya ha aplicado a esta vacante" });

        var aplicacion = new Vacante_Aplicacion
        {
            VacanteID = vacanteId,
            ColaboradorID = dto.ColaboradorID,
            FechaAplicacion = System.DateTime.UtcNow,
            Estado = "Aplicado",
            Interes = dto.Interes ?? "Medio"
        };

        _context.Set<Vacante_Aplicacion>().Add(aplicacion);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAplicacionesPorVacante), new { vacanteId }, aplicacion);
    }

    // [POST] api/vacante/{vacanteId}/mostrar-interes - UC-V3: Mostrar Interés en Vacante
    [HttpPost("{vacanteId}/mostrar-interes")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> MostrarInteres(int vacanteId, [FromBody] Vacante_AplicacionCreateDto dto)
    {
        var vacante = await _vacanteRepository.GetByIdWithRequisitosAsync(vacanteId);
        if (vacante == null)
            return NotFound(new { message = "Vacante no encontrada" });

        var colaborador = await _context.Colaboradores.FindAsync(dto.ColaboradorID);
        if (colaborador == null)
            return NotFound(new { message = "Colaborador no encontrado" });

        var aplicacion = await _context.Set<Vacante_Aplicacion>()
            .FirstOrDefaultAsync(a => a.VacanteID == vacanteId && a.ColaboradorID == dto.ColaboradorID);

        if (aplicacion == null)
        {
            // Crear nueva aplicación con estado "Interesado"
            aplicacion = new Vacante_Aplicacion
            {
                VacanteID = vacanteId,
                ColaboradorID = dto.ColaboradorID,
                FechaAplicacion = System.DateTime.UtcNow,
                Estado = "Interesado",
                Interes = dto.Interes ?? "Alto"
            };
            _context.Set<Vacante_Aplicacion>().Add(aplicacion);
        }
        else
        {
            // Actualizar el estado a interesado
            aplicacion.Estado = "Interesado";
            aplicacion.Interes = dto.Interes ?? "Alto";
        }

        await _context.SaveChangesAsync();
        return Ok(aplicacion);
    }

    // [GET] api/vacante/{vacanteId}/aplicaciones - Ver Aplicaciones por Vacante
    [HttpGet("{vacanteId}/aplicaciones")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAplicacionesPorVacante(int vacanteId)
    {
        var aplicaciones = await _context.Set<Vacante_Aplicacion>()
            .Include(a => a.Colaborador)
            .Where(a => a.VacanteID == vacanteId)
            .ToListAsync();

        return Ok(aplicaciones);
    }

    // [GET] api/vacante/colaborador/{colaboradorId}/mis-aplicaciones - Ver mis aplicaciones
    [HttpGet("colaborador/{colaboradorId}/mis-aplicaciones")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> MisAplicaciones(int colaboradorId)
    {
        var aplicaciones = await _context.Set<Vacante_Aplicacion>()
            .Include(a => a.Vacante)
            .Where(a => a.ColaboradorID == colaboradorId)
            .ToListAsync();

        return Ok(aplicaciones);
    }

    // [GET] api/vacante/colaborador/{colaboradorId}/vacantes-aplicadas - Ver vacantes donde he aplicado
    [HttpGet("colaborador/{colaboradorId}/vacantes-aplicadas")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> VacantesAplicadas(int colaboradorId)
    {
        var vacantes = await _context.Set<Vacante_Aplicacion>()
            .Include(a => a.Vacante)
            .Where(a => a.ColaboradorID == colaboradorId)
            .Select(a => a.Vacante)
            .Distinct()
            .ToListAsync();

        return Ok(vacantes);
    }
}