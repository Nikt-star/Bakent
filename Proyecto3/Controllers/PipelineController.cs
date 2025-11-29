// Proyecto3/Controllers/PipelineController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto3.Core.Entities;
using Proyecto3.Core.Models;
using Proyecto3.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PipelineController : ControllerBase
    {
        private readonly TalentoInternoDbContext _context;

        public PipelineController(TalentoInternoDbContext context)
        {
            _context = context;
        }

        // [GET] api/pipeline/vacante/{vacanteId} - UC-C1: Ver Candidatos Aplicados/Matcheados
        [HttpGet("vacante/{vacanteId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Vacante_Aplicacion>))]
        public async Task<IActionResult> GetCandidatosEnPipeline(int vacanteId)
        {
            var candidatos = await _context.Set<Vacante_Aplicacion>()
                .Include(a => a.Colaborador)
                .Where(a => a.VacanteID == vacanteId)
                .OrderByDescending(a => a.FechaAplicacion)
                .ToListAsync();

            return Ok(candidatos);
        }

        // [GET] api/pipeline/vacante/{vacanteId}/por-estado - Listar candidatos por estado
        [HttpGet("vacante/{vacanteId}/por-estado")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetCandidatosPorEstado(int vacanteId)
        {
            var estadosPipeline = await _context.Set<Vacante_Aplicacion>()
                .Include(a => a.Colaborador)
                .Where(a => a.VacanteID == vacanteId)
                .GroupBy(a => a.Estado)
                .Select(g => new
                {
                    Estado = g.Key,
                    Cantidad = g.Count(),
                    Candidatos = g.Select(a => new
                    {
                        a.AplicacionID,
                        a.ColaboradorID,
                        a.Colaborador.NombreCompleto,
                        a.FechaAplicacion,
                        a.Interes,
                        a.Notas
                    }).ToList()
                })
                .ToListAsync();

            return Ok(estadosPipeline);
        }

        // [PUT] api/pipeline/{aplicacionId}/estado - UC-G1: Cambiar Estado de Candidato
        [HttpPut("{aplicacionId}/estado")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CambiarEstadoCandidato(int aplicacionId, [FromBody] Vacante_AplicacionUpdateDto dto)
        {
            var aplicacion = await _context.Set<Vacante_Aplicacion>()
                .Include(a => a.Colaborador)
                .FirstOrDefaultAsync(a => a.AplicacionID == aplicacionId);

            if (aplicacion == null)
                return NotFound(new { message = "Aplicación no encontrada" });

            // Validar que el estado sea válido
            var estadosValidos = new[] { "Aplicado", "Interesado", "En_Evaluacion", "Rechazado", "Seleccionado" };
            if (!estadosValidos.Contains(dto.Estado))
                return BadRequest(new { message = "Estado no válido" });

            var estadoAnterior = aplicacion.Estado;
            aplicacion.Estado = dto.Estado;

            if (!string.IsNullOrWhiteSpace(dto.Interes))
                aplicacion.Interes = dto.Interes;

            if (!string.IsNullOrWhiteSpace(dto.Notas))
                aplicacion.Notas = dto.Notas;

            _context.Set<Vacante_Aplicacion>().Update(aplicacion);

            // Crear notificación al candidato
            if (aplicacion.Colaborador != null)
            {
                var tipoNotificacion = dto.Estado switch
                {
                    "En_Evaluacion" => "Tu aplicación está siendo evaluada",
                    "Seleccionado" => "¡Felicitaciones! Has sido seleccionado para esta vacante",
                    "Rechazado" => "Lamentablemente no fuiste seleccionado en esta ocasión",
                    _ => $"Tu estado en el pipeline ha cambiado a {dto.Estado}"
                };

                var notificacion = new Notificacion
                {
                    ColaboradorID = aplicacion.ColaboradorID,
                    Titulo = "Actualización de Candidatura",
                    Mensaje = tipoNotificacion,
                    Tipo = "Vacante",
                    Leida = false,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Set<Notificacion>().Add(notificacion);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [GET] api/pipeline/{aplicacionId} - Ver detalle de aplicación
        [HttpGet("{aplicacionId}")]
        [ProducesResponseType(200, Type = typeof(Vacante_Aplicacion))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAplicacion(int aplicacionId)
        {
            var aplicacion = await _context.Set<Vacante_Aplicacion>()
                .Include(a => a.Colaborador)
                .Include(a => a.Vacante)
                .FirstOrDefaultAsync(a => a.AplicacionID == aplicacionId);

            if (aplicacion == null)
                return NotFound(new { message = "Aplicación no encontrada" });

            return Ok(aplicacion);
        }

        // [GET] api/pipeline/vacante/{vacanteId}/etapa-aplicado - Candidatos en etapa "Aplicado"
        [HttpGet("vacante/{vacanteId}/etapa-aplicado")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Vacante_Aplicacion>))]
        public async Task<IActionResult> GetCandidatosAplicados(int vacanteId)
        {
            var candidatos = await _context.Set<Vacante_Aplicacion>()
                .Include(a => a.Colaborador)
                .Where(a => a.VacanteID == vacanteId && a.Estado == "Aplicado")
                .ToListAsync();

            return Ok(candidatos);
        }

        // [GET] api/pipeline/vacante/{vacanteId}/etapa-evaluacion - Candidatos en evaluación
        [HttpGet("vacante/{vacanteId}/etapa-evaluacion")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Vacante_Aplicacion>))]
        public async Task<IActionResult> GetCandidatosEnEvaluacion(int vacanteId)
        {
            var candidatos = await _context.Set<Vacante_Aplicacion>()
                .Include(a => a.Colaborador)
                .Where(a => a.VacanteID == vacanteId && a.Estado == "En_Evaluacion")
                .ToListAsync();

            return Ok(candidatos);
        }

        // [GET] api/pipeline/vacante/{vacanteId}/seleccionados - Candidatos seleccionados
        [HttpGet("vacante/{vacanteId}/seleccionados")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Vacante_Aplicacion>))]
        public async Task<IActionResult> GetCandidatosSeleccionados(int vacanteId)
        {
            var candidatos = await _context.Set<Vacante_Aplicacion>()
                .Include(a => a.Colaborador)
                .Where(a => a.VacanteID == vacanteId && a.Estado == "Seleccionado")
                .ToListAsync();

            return Ok(candidatos);
        }

        // [POST] api/pipeline/{aplicacionId}/agregar-notas - Agregar notas a candidato
        [HttpPost("{aplicacionId}/agregar-notas")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AgregarNotas(int aplicacionId, [FromBody] dynamic request)
        {
            var aplicacion = await _context.Set<Vacante_Aplicacion>().FindAsync(aplicacionId);
            if (aplicacion == null)
                return NotFound(new { message = "Aplicación no encontrada" });

            string notas = request.notas ?? "";
            aplicacion.Notas = notas;

            _context.Set<Vacante_Aplicacion>().Update(aplicacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [GET] api/pipeline/vacante/{vacanteId}/resumen - Resumen del pipeline
        [HttpGet("vacante/{vacanteId}/resumen")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetResumenPipeline(int vacanteId)
        {
            var total = await _context.Set<Vacante_Aplicacion>()
                .Where(a => a.VacanteID == vacanteId)
                .CountAsync();

            var aplicados = await _context.Set<Vacante_Aplicacion>()
                .Where(a => a.VacanteID == vacanteId && a.Estado == "Aplicado")
                .CountAsync();

            var enEvaluacion = await _context.Set<Vacante_Aplicacion>()
                .Where(a => a.VacanteID == vacanteId && a.Estado == "En_Evaluacion")
                .CountAsync();

            var seleccionados = await _context.Set<Vacante_Aplicacion>()
                .Where(a => a.VacanteID == vacanteId && a.Estado == "Seleccionado")
                .CountAsync();

            var rechazados = await _context.Set<Vacante_Aplicacion>()
                .Where(a => a.VacanteID == vacanteId && a.Estado == "Rechazado")
                .CountAsync();

            return Ok(new
            {
                TotalCandidatos = total,
                Aplicados = aplicados,
                EnEvaluacion = enEvaluacion,
                Seleccionados = seleccionados,
                Rechazados = rechazados
            });
        }
    }
}
