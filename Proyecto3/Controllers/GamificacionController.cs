// Proyecto3/Controllers/GamificacionController.cs

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
    public class GamificacionController : ControllerBase
    {
        private readonly TalentoInternoDbContext _context;

        public GamificacionController(TalentoInternoDbContext context)
        {
            _context = context;
        }

        // [GET] api/gamificacion/puntos/{colaboradorId} - UC-G1: Ver Puntos de Colaborador
        [HttpGet("puntos/{colaboradorId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetPuntosTotal(int colaboradorId)
        {
            var total = await _context.Set<Punto>()
                .Where(p => p.ColaboradorID == colaboradorId)
                .SumAsync(p => (int?)p.Cantidad) ?? 0;

            return Ok(new { totalPuntos = total });
        }

        // [GET] api/gamificacion/historial/{colaboradorId} - UC-G5: Ver Log de Puntos
        [HttpGet("historial/{colaboradorId}")]
        [ProducesResponseType(200, Type = typeof(LogPuntosDto))]
        public async Task<IActionResult> GetHistorialPuntos(int colaboradorId)
        {
            var total = await _context.Set<Punto>()
                .Where(p => p.ColaboradorID == colaboradorId)
                .SumAsync(p => (int?)p.Cantidad) ?? 0;

            var historial = await _context.Set<Punto>()
                .Where(p => p.ColaboradorID == colaboradorId)
                .OrderByDescending(p => p.FechaOtorgamiento)
                .Select(p => new PuntoRegistroDto
                {
                    Fecha = p.FechaOtorgamiento,
                    Cantidad = p.Cantidad,
                    Razon = p.Razon
                })
                .ToListAsync();

            var logDto = new LogPuntosDto
            {
                TotalPuntos = total,
                Historial = historial
            };

            return Ok(logDto);
        }

        // [POST] api/gamificacion/otorgar-puntos - Otorgar Puntos (Sistema/Admin)
        [HttpPost("otorgar-puntos")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> OtorgarPuntos([FromBody] PuntoDto dto)
        {
            // Validar que el colaborador existe
            var colaborador = await _context.Colaboradores.FindAsync(dto.ColaboradorID);
            if (colaborador == null)
                return NotFound(new { message = "Colaborador no encontrado" });

            // Calcular total acumulado
            var totalAnterior = await _context.Set<Punto>()
                .Where(p => p.ColaboradorID == dto.ColaboradorID)
                .SumAsync(p => (int?)p.Cantidad) ?? 0;

            var punto = new Punto
            {
                ColaboradorID = dto.ColaboradorID,
                Cantidad = dto.Cantidad,
                Razon = dto.Razon,
                FechaOtorgamiento = DateTime.UtcNow,
                TotalAcumulado = totalAnterior + dto.Cantidad
            };

            _context.Set<Punto>().Add(punto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHistorialPuntos), new { colaboradorId = dto.ColaboradorID }, punto);
        }

        // [POST] api/gamificacion/validar-certificacion - UC-G2: Validar Certificación y Otorgar Puntos
        [HttpPost("validar-certificacion")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ValidarCertificacionYOtorgarPuntos(int certificacionId, int colaboradorId)
        {
            var certificacion = await _context.Set<Certificacion>()
                .FindAsync(certificacionId);

            if (certificacion == null || certificacion.ColaboradorID != colaboradorId)
                return NotFound(new { message = "Certificación no encontrada" });

            // Otorgar puntos por certificación
            var totalAnterior = await _context.Set<Punto>()
                .Where(p => p.ColaboradorID == colaboradorId)
                .SumAsync(p => (int?)p.Cantidad) ?? 0;

            var punto = new Punto
            {
                ColaboradorID = colaboradorId,
                Cantidad = 50, // Puntos por certificación
                Razon = $"Certificación validada: {certificacion.NombreCertificacion}",
                FechaOtorgamiento = DateTime.UtcNow,
                TotalAcumulado = totalAnterior + 50
            };

            _context.Set<Punto>().Add(punto);

            // Crear notificación
            var notificacion = new Notificacion
            {
                ColaboradorID = colaboradorId,
                Titulo = "¡Certificación Validada!",
                Mensaje = $"Tu certificación '{certificacion.NombreCertificacion}' ha sido validada. Has ganado 50 puntos.",
                Tipo = "Certificacion",
                Leida = false,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Set<Notificacion>().Add(notificacion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Certificación validada y puntos otorgados", puntosOtorgados = 50 });
        }

        // [POST] api/gamificacion/marcar-perfil-completo - Verificar y otorgar puntos por perfil completo
        [HttpPost("marcar-perfil-completo")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> OtorgarPuntosPorPerfilCompleto(int colaboradorId)
        {
            var perfil = await _context.Set<Perfil>()
                .FirstOrDefaultAsync(p => p.ColaboradorID == colaboradorId);

            if (perfil == null || !perfil.PerfilCompleto)
                return BadRequest(new { message = "El perfil no está completo" });

            // Verificar si ya se otorgaron puntos
            var puntoExistente = await _context.Set<Punto>()
                .FirstOrDefaultAsync(p => p.ColaboradorID == colaboradorId && p.Razon == "Perfil Completado");

            if (puntoExistente != null)
                return BadRequest(new { message = "Ya se han otorgado puntos por perfil completo" });

            // Otorgar puntos
            var totalAnterior = await _context.Set<Punto>()
                .Where(p => p.ColaboradorID == colaboradorId)
                .SumAsync(p => (int?)p.Cantidad) ?? 0;

            var punto = new Punto
            {
                ColaboradorID = colaboradorId,
                Cantidad = 100,
                Razon = "Perfil Completado",
                FechaOtorgamiento = DateTime.UtcNow,
                TotalAcumulado = totalAnterior + 100
            };

            _context.Set<Punto>().Add(punto);

            // Crear notificación
            var notificacion = new Notificacion
            {
                ColaboradorID = colaboradorId,
                Titulo = "¡Perfil Completado!",
                Mensaje = "¡Felicitaciones! Tu perfil está completo. Has ganado 100 puntos.",
                Tipo = "Puntos",
                Leida = false,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Set<Notificacion>().Add(notificacion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Puntos otorgados por perfil completo", puntosOtorgados = 100 });
        }

        // [POST] api/gamificacion/marcar-curso-completado - UC-G3: Marcar Curso Completado y Otorgar Puntos
        [HttpPost("marcar-curso-completado")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MarcarCursoCompletado(int colaboradorId, int cursoId)
        {
            var progreso = await _context.Set<Progreso_Curso>()
                .FirstOrDefaultAsync(pc => pc.ColaboradorID == colaboradorId && pc.CursoID == cursoId);

            if (progreso == null)
                return NotFound(new { message = "Progreso del curso no encontrado" });

            if (progreso.Completado)
                return BadRequest(new { message = "El curso ya fue marcado como completado" });

            progreso.Completado = true;
            progreso.PorcentajeCompletacion = 100;
            progreso.FechaCompletacion = DateTime.UtcNow;

            // Otorgar puntos por curso completado
            var totalAnterior = await _context.Set<Punto>()
                .Where(p => p.ColaboradorID == colaboradorId)
                .SumAsync(p => (int?)p.Cantidad) ?? 0;

            var punto = new Punto
            {
                ColaboradorID = colaboradorId,
                Cantidad = 75,
                Razon = "Curso Completado",
                FechaOtorgamiento = DateTime.UtcNow,
                TotalAcumulado = totalAnterior + 75
            };

            _context.Set<Progreso_Curso>().Update(progreso);
            _context.Set<Punto>().Add(punto);

            // Crear notificación
            var notificacion = new Notificacion
            {
                ColaboradorID = colaboradorId,
                Titulo = "¡Curso Completado!",
                Mensaje = $"¡Excelente! Completaste un curso. Has ganado 75 puntos.",
                Tipo = "Desarrollo",
                Leida = false,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Set<Notificacion>().Add(notificacion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Curso marcado como completado y puntos otorgados", puntosOtorgados = 75 });
        }

        // [GET] api/gamificacion/leaderboard - Leaderboard de puntos
        [HttpGet("leaderboard")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetLeaderboard([FromQuery] int top = 10)
        {
            var leaderboard = await _context.Set<Punto>()
                .GroupBy(p => p.ColaboradorID)
                .Select(g => new
                {
                    ColaboradorID = g.Key,
                    TotalPuntos = g.Sum(p => p.Cantidad)
                })
                .OrderByDescending(x => x.TotalPuntos)
                .Take(top)
                .Join(_context.Colaboradores, x => x.ColaboradorID, c => c.ColaboradorID, 
                      (x, c) => new { c.ColaboradorID, c.NombreCompleto, x.TotalPuntos })
                .ToListAsync();

            return Ok(leaderboard);
        }
    }
}
