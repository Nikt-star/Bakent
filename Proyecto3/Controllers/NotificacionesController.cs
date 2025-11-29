// Proyecto3/Controllers/NotificacionesController.cs

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
    public class NotificacionesController : ControllerBase
    {
        private readonly TalentoInternoDbContext _context;

        public NotificacionesController(TalentoInternoDbContext context)
        {
            _context = context;
        }

        // [GET] api/notificaciones/colaborador/{colaboradorId} - UC-N1: Recibir Notificaciones
        [HttpGet("colaborador/{colaboradorId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Notificacion>))]
        public async Task<IActionResult> GetNotificacionesPorColaborador(int colaboradorId)
        {
            var notificaciones = await _context.Set<Notificacion>()
                .Where(n => n.ColaboradorID == colaboradorId)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync();

            return Ok(notificaciones);
        }

        // [GET] api/notificaciones/colaborador/{colaboradorId}/no-leidas - Ver notificaciones no leídas
        [HttpGet("colaborador/{colaboradorId}/no-leidas")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Notificacion>))]
        public async Task<IActionResult> GetNotificacionesNoLeidas(int colaboradorId)
        {
            var notificaciones = await _context.Set<Notificacion>()
                .Where(n => n.ColaboradorID == colaboradorId && !n.Leida)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync();

            return Ok(notificaciones);
        }

        // [GET] api/notificaciones/{notificacionId} - Ver detalle de notificación
        [HttpGet("{notificacionId}")]
        [ProducesResponseType(200, Type = typeof(Notificacion))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetNotificacion(int notificacionId)
        {
            var notificacion = await _context.Set<Notificacion>().FindAsync(notificacionId);
            if (notificacion == null)
                return NotFound(new { message = "Notificación no encontrada" });

            return Ok(notificacion);
        }

        // [POST] api/notificaciones - Crear Notificación (Sistema/Admin)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Notificacion))]
        public async Task<IActionResult> CreateNotificacion([FromBody] NotificacionCreateDto dto)
        {
            var notificacion = new Notificacion
            {
                ColaboradorID = dto.ColaboradorID,
                Titulo = dto.Titulo,
                Mensaje = dto.Mensaje,
                Tipo = dto.Tipo,
                Leida = false,
                FechaCreacion = DateTime.UtcNow,
                FechaLectura = null
            };

            _context.Set<Notificacion>().Add(notificacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotificacion), new { notificacionId = notificacion.NotificacionID }, notificacion);
        }

        // [PUT] api/notificaciones/{notificacionId}/marcar-leida - Marcar como leída
        [HttpPut("{notificacionId}/marcar-leida")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MarcarComoLeida(int notificacionId)
        {
            var notificacion = await _context.Set<Notificacion>().FindAsync(notificacionId);
            if (notificacion == null)
                return NotFound(new { message = "Notificación no encontrada" });

            notificacion.Leida = true;
            notificacion.FechaLectura = DateTime.UtcNow;

            _context.Set<Notificacion>().Update(notificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [PUT] api/notificaciones/colaborador/{colaboradorId}/marcar-todas-leidas - Marcar todas como leídas
        [HttpPut("colaborador/{colaboradorId}/marcar-todas-leidas")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> MarcarTodasComoLeidas(int colaboradorId)
        {
            var notificaciones = await _context.Set<Notificacion>()
                .Where(n => n.ColaboradorID == colaboradorId && !n.Leida)
                .ToListAsync();

            foreach (var notificacion in notificaciones)
            {
                notificacion.Leida = true;
                notificacion.FechaLectura = DateTime.UtcNow;
            }

            _context.Set<Notificacion>().UpdateRange(notificaciones);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [DELETE] api/notificaciones/{notificacionId} - Eliminar notificación
        [HttpDelete("{notificacionId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteNotificacion(int notificacionId)
        {
            var notificacion = await _context.Set<Notificacion>().FindAsync(notificacionId);
            if (notificacion == null)
                return NotFound(new { message = "Notificación no encontrada" });

            _context.Set<Notificacion>().Remove(notificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [GET] api/notificaciones/colaborador/{colaboradorId}/contar - Contar no leídas
        [HttpGet("colaborador/{colaboradorId}/contar")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ContarNotificacionesNoLeidas(int colaboradorId)
        {
            var cantidad = await _context.Set<Notificacion>()
                .Where(n => n.ColaboradorID == colaboradorId && !n.Leida)
                .CountAsync();

            return Ok(new { cantidad });
        }
    }
}
