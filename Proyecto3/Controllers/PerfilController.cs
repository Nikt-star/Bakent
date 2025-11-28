// Proyecto3/Controllers/PerfilController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto3.Core.Entities;
using Proyecto3.Infrastructure.Data;
using Proyecto3.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proyecto3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilController : ControllerBase
    {
        private readonly TalentoInternoDbContext _context;

        public PerfilController(TalentoInternoDbContext context)
        {
            _context = context;
        }

        // [POST] api/perfil - UC-P1: Crear Perfil Propio
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Perfil))]
        public async Task<IActionResult> CreatePerfil([FromBody] PerfilCreateDto dto)
        {
            // Validar que el colaborador existe
            var colaborador = await _context.Colaboradores.FindAsync(dto.ColaboradorID);
            if (colaborador == null)
                return NotFound(new { message = "Colaborador no encontrado" });

            // Verificar que no exista un perfil previo
            var perfilExistente = await _context.Set<Perfil>().FirstOrDefaultAsync(p => p.ColaboradorID == dto.ColaboradorID);
            if (perfilExistente != null)
                return Conflict(new { message = "El colaborador ya tiene un perfil creado" });

            var perfil = new Perfil
            {
                ColaboradorID = dto.ColaboradorID,
                Resumen = dto.Resumen,
                FotoUrl = dto.FotoUrl,
                LinkedIn = dto.LinkedIn,
                Telefono = dto.Telefono,
                Email = dto.Email,
                PerfilCompleto = !string.IsNullOrWhiteSpace(dto.Resumen) && !string.IsNullOrWhiteSpace(dto.Email),
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            };

            _context.Set<Perfil>().Add(perfil);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerfilByColaborador), new { colaboradorId = dto.ColaboradorID }, perfil);
        }

        // [GET] api/perfil/{colaboradorId} - UC-P2: Consultar Perfil Propio
        [HttpGet("{colaboradorId}")]
        [ProducesResponseType(200, Type = typeof(Perfil))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPerfilByColaborador(int colaboradorId)
        {
            var perfil = await _context.Set<Perfil>()
                .Include(p => p.Colaborador)
                .FirstOrDefaultAsync(p => p.ColaboradorID == colaboradorId);

            if (perfil == null)
                return NotFound(new { message = "Perfil no encontrado" });

            return Ok(perfil);
        }

        // [PUT] api/perfil/{colaboradorId} - UC-P3: Editar Perfil Propio
        [HttpPut("{colaboradorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePerfil(int colaboradorId, [FromBody] PerfilUpdateDto dto)
        {
            var perfil = await _context.Set<Perfil>()
                .FirstOrDefaultAsync(p => p.ColaboradorID == colaboradorId);

            if (perfil == null)
                return NotFound(new { message = "Perfil no encontrado" });

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrWhiteSpace(dto.Resumen))
                perfil.Resumen = dto.Resumen;
            if (dto.FotoUrl != null)
                perfil.FotoUrl = dto.FotoUrl;
            if (dto.LinkedIn != null)
                perfil.LinkedIn = dto.LinkedIn;
            if (dto.Telefono != null)
                perfil.Telefono = dto.Telefono;
            if (dto.Email != null)
                perfil.Email = dto.Email;

            // Verificar si el perfil está completo
            perfil.PerfilCompleto = !string.IsNullOrWhiteSpace(perfil.Resumen) && 
                                   !string.IsNullOrWhiteSpace(perfil.Email) &&
                                   !string.IsNullOrWhiteSpace(perfil.Telefono);
            perfil.FechaActualizacion = DateTime.UtcNow;

            _context.Set<Perfil>().Update(perfil);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [GET] api/perfil/{colaboradorId}/completo - Verificar si perfil está completo
        [HttpGet("{colaboradorId}/completo")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> IsPerfilCompleto(int colaboradorId)
        {
            var perfil = await _context.Set<Perfil>()
                .FirstOrDefaultAsync(p => p.ColaboradorID == colaboradorId);

            if (perfil == null)
                return NotFound(new { message = "Perfil no encontrado" });

            return Ok(new { perfilCompleto = perfil.PerfilCompleto });
        }
    }
}
