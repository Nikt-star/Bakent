// Proyecto3/Controllers/RRHHController.cs

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
    public class RRHHController : ControllerBase
    {
        private readonly TalentoInternoDbContext _context;

        public RRHHController(TalentoInternoDbContext context)
        {
            _context = context;
        }

        // [POST] api/rrhh/certificaciones - UC-R1: Registrar Certificaciones de Terceros/Delegación
        [HttpPost("certificaciones")]
        [ProducesResponseType(201, Type = typeof(Certificacion))]
        public async Task<IActionResult> RegistrarCertificacionTerceros(int colaboradorId, [FromBody] CertificacionCreateDto dto)
        {
            // Validar que el colaborador existe
            var colaborador = await _context.Colaboradores.FindAsync(colaboradorId);
            if (colaborador == null)
                return NotFound(new { message = "Colaborador no encontrado" });

            var certificacion = new Certificacion
            {
                ColaboradorID = colaboradorId,
                NombreCertificacion = dto.NombreCertificacion,
                FechaObtencion = dto.FechaObtencion,
                Institucion = dto.Institucion
            };

            _context.Set<Certificacion>().Add(certificacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCertificacion), new { id = certificacion.CertificacionID }, certificacion);
        }

        // [GET] api/rrhh/certificaciones - Listar todas las certificaciones registradas
        [HttpGet("certificaciones")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Certificacion>))]
        public async Task<IActionResult> GetCertificaciones()
        {
            var certificaciones = await _context.Set<Certificacion>()
                .Include(c => c.Colaborador)
                .OrderByDescending(c => c.FechaObtencion)
                .ToListAsync();

            return Ok(certificaciones);
        }

        // [GET] api/rrhh/certificaciones/{id} - Obtener detalle de certificación
        [HttpGet("certificaciones/{id}")]
        [ProducesResponseType(200, Type = typeof(Certificacion))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCertificacion(int id)
        {
            var certificacion = await _context.Set<Certificacion>()
                .Include(c => c.Colaborador)
                .FirstOrDefaultAsync(c => c.CertificacionID == id);

            if (certificacion == null)
                return NotFound(new { message = "Certificación no encontrada" });

            return Ok(certificacion);
        }

        // [GET] api/rrhh/certificaciones/colaborador/{colaboradorId} - Listar certificaciones de un colaborador
        [HttpGet("certificaciones/colaborador/{colaboradorId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Certificacion>))]
        public async Task<IActionResult> GetCertificacionesPorColaborador(int colaboradorId)
        {
            var certificaciones = await _context.Set<Certificacion>()
                .Where(c => c.ColaboradorID == colaboradorId)
                .OrderByDescending(c => c.FechaObtencion)
                .ToListAsync();

            return Ok(certificaciones);
        }

        // [PUT] api/rrhh/certificaciones/{id} - Actualizar certificación registrada
        [HttpPut("certificaciones/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCertificacion(int id, [FromBody] CertificacionCreateDto dto)
        {
            var certificacion = await _context.Set<Certificacion>().FindAsync(id);
            if (certificacion == null)
                return NotFound(new { message = "Certificación no encontrada" });

            certificacion.NombreCertificacion = dto.NombreCertificacion ?? certificacion.NombreCertificacion;
            certificacion.FechaObtencion = dto.FechaObtencion ?? certificacion.FechaObtencion;
            certificacion.Institucion = dto.Institucion ?? certificacion.Institucion;

            _context.Set<Certificacion>().Update(certificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [DELETE] api/rrhh/certificaciones/{id} - Eliminar certificación registrada
        [HttpDelete("certificaciones/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteCertificacion(int id)
        {
            var certificacion = await _context.Set<Certificacion>().FindAsync(id);
            if (certificacion == null)
                return NotFound(new { message = "Certificación no encontrada" });

            _context.Set<Certificacion>().Remove(certificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ===== ENDPOINTS DE GESTIÓN DE HABILIDADES =====

        // [PUT] api/rrhh/habilidades/actualizar - UC-H1: Actualizar Skills y Nivel de Dominio (Por Evaluación)
        [HttpPut("habilidades/actualizar")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ActualizarHabilidadPorEvaluacion(
            int colaboradorId, 
            int skillId, 
            [FromBody] HabilidadCreateDto dto)
        {
            var habilidad = await _context.Set<HabilidadColaborador>()
                .FirstOrDefaultAsync(h => h.ColaboradorID == colaboradorId && h.SkillID == skillId);

            if (habilidad == null)
                return NotFound(new { message = "Habilidad no encontrada" });

            habilidad.NivelDominio = dto.NivelDominio;
            habilidad.FechaUltimaEvaluacion = DateTime.UtcNow;

            _context.Set<HabilidadColaborador>().Update(habilidad);

            // Crear notificación al colaborador
            var skill = await _context.Set<Skill>().FindAsync(skillId);
            var notificacion = new Notificacion
            {
                ColaboradorID = colaboradorId,
                Titulo = "Evaluación de Habilidad Completada",
                Mensaje = $"Tu habilidad '{skill?.NombreSkill}' ha sido evaluada. Nuevo nivel: {dto.NivelDominio}",
                Tipo = "Otra",
                Leida = false,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Set<Notificacion>().Add(notificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [GET] api/rrhh/habilidades/colaborador/{colaboradorId} - Ver habilidades de un colaborador
        [HttpGet("habilidades/colaborador/{colaboradorId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<HabilidadColaborador>))]
        public async Task<IActionResult> GetHabilidadesColaborador(int colaboradorId)
        {
            var habilidades = await _context.Set<HabilidadColaborador>()
                .Include(h => h.Skill)
                .Where(h => h.ColaboradorID == colaboradorId)
                .ToListAsync();

            return Ok(habilidades);
        }

        // ===== ENDPOINTS DE GESTIÓN DE USUARIOS =====

        // [PUT] api/rrhh/usuarios/{colaboradorId}/suspender - UC-U1: Suspender Cuenta de Usuario
        [HttpPut("usuarios/{colaboradorId}/suspender")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SuspenderUsuario(int colaboradorId)
        {
            var colaborador = await _context.Colaboradores.FindAsync(colaboradorId);
            if (colaborador == null)
                return NotFound(new { message = "Colaborador no encontrado" });

            // En una implementación real, aquí marcarías una propiedad 'Activo' o similar
            // Por ahora, vamos a registrar la acción en un log

            var notificacion = new Notificacion
            {
                ColaboradorID = colaboradorId,
                Titulo = "Cuenta Suspendida",
                Mensaje = "Tu cuenta ha sido suspendida. Contacta con RR.HH. para más información.",
                Tipo = "Otra",
                Leida = false,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Set<Notificacion>().Add(notificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [PUT] api/rrhh/usuarios/{colaboradorId}/banear - UC-U1: Banear Cuenta de Usuario
        [HttpPut("usuarios/{colaboradorId}/banear")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> BanearUsuario(int colaboradorId)
        {
            var colaborador = await _context.Colaboradores.FindAsync(colaboradorId);
            if (colaborador == null)
                return NotFound(new { message = "Colaborador no encontrado" });

            var notificacion = new Notificacion
            {
                ColaboradorID = colaboradorId,
                Titulo = "Cuenta Baneada",
                Mensaje = "Tu cuenta ha sido baneada permanentemente. No puedes realizar más acciones.",
                Tipo = "Otra",
                Leida = false,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Set<Notificacion>().Add(notificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [GET] api/rrhh/usuarios/activos - Listar usuarios activos
        [HttpGet("usuarios/activos")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Colaborador>))]
        public async Task<IActionResult> GetUsuariosActivos()
        {
            var colaboradores = await _context.Colaboradores.ToListAsync();
            return Ok(colaboradores);
        }

        // [GET] api/rrhh/reporte-certificaciones - Reporte general de certificaciones
        [HttpGet("reporte-certificaciones")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetReporteCertificaciones()
        {
            var reporte = await _context.Set<Certificacion>()
                .Include(c => c.Colaborador)
                .GroupBy(c => c.Institucion)
                .Select(g => new
                {
                    Institucion = g.Key,
                    TotalCertificaciones = g.Count(),
                    Colaboradores = g.Select(c => c.Colaborador != null ? c.Colaborador.NombreCompleto : "N/A").Distinct().ToList()
                })
                .ToListAsync();

            return Ok(reporte);
        }

        // [GET] api/rrhh/reporte-habilidades - Reporte de habilidades por colaborador
        [HttpGet("reporte-habilidades")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetReporteHabilidades()
        {
            var reporte = await _context.Set<HabilidadColaborador>()
                .Include(h => h.Colaborador)
                .Include(h => h.Skill)
                .GroupBy(h => h.Colaborador)
                .Select(g => new
                {
                    ColaboradorID = g.Key != null ? g.Key.ColaboradorID : 0,
                    Nombre = g.Key != null ? g.Key.NombreCompleto : "N/A",
                    TotalHabilidades = g.Count(),
                    Habilidades = g.Select(h => new 
                    { 
                        Skill = h.Skill != null ? h.Skill.NombreSkill : "N/A", 
                        h.NivelDominio 
                    }).ToList()
                })
                .ToListAsync();

            return Ok(reporte);
        }
    }
}
