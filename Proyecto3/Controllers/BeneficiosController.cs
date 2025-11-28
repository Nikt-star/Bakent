// Proyecto3/Controllers/BeneficiosController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto3.Core.Entities;
using Proyecto3.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiosController : ControllerBase
    {
        private readonly TalentoInternoDbContext _context;

        public BeneficiosController(TalentoInternoDbContext context)
        {
            _context = context;
        }

        // [GET] api/beneficios - UC-B1: Ver Listado de Beneficios
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Beneficio>))]
        public async Task<IActionResult> GetBeneficios()
        {
            var beneficios = await _context.Set<Beneficio>()
                .Where(b => b.Activo)
                .ToListAsync();

            return Ok(beneficios);
        }

        // [GET] api/beneficios/{id} - Ver Detalle de un Beneficio
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Beneficio))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBeneficio(int id)
        {
            var beneficio = await _context.Set<Beneficio>().FindAsync(id);
            if (beneficio == null)
                return NotFound(new { message = "Beneficio no encontrado" });

            return Ok(beneficio);
        }

        // [GET] api/beneficios/categoria/{categoria} - Listar por Categoría
        [HttpGet("categoria/{categoria}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Beneficio>))]
        public async Task<IActionResult> GetBeneficiosPorCategoria(string categoria)
        {
            var beneficios = await _context.Set<Beneficio>()
                .Where(b => b.Activo && b.Categoria == categoria)
                .ToListAsync();

            return Ok(beneficios);
        }

        // [POST] api/beneficios - Crear Beneficio (Admin)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Beneficio))]
        public async Task<IActionResult> CreateBeneficio([FromBody] Beneficio beneficio)
        {
            if (string.IsNullOrWhiteSpace(beneficio.Nombre))
                return BadRequest(new { message = "El nombre del beneficio es requerido" });

            beneficio.FechaCreacion = System.DateTime.UtcNow;
            beneficio.Activo = true;

            _context.Set<Beneficio>().Add(beneficio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBeneficio), new { id = beneficio.BeneficioID }, beneficio);
        }

        // [PUT] api/beneficios/{id} - Editar Beneficio (Admin)
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateBeneficio(int id, [FromBody] Beneficio beneficio)
        {
            var existente = await _context.Set<Beneficio>().FindAsync(id);
            if (existente == null)
                return NotFound(new { message = "Beneficio no encontrado" });

            existente.Nombre = beneficio.Nombre ?? existente.Nombre;
            existente.Descripcion = beneficio.Descripcion ?? existente.Descripcion;
            existente.Categoria = beneficio.Categoria ?? existente.Categoria;
            existente.Activo = beneficio.Activo;

            _context.Set<Beneficio>().Update(existente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [DELETE] api/beneficios/{id} - Desactivar Beneficio (Admin)
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteBeneficio(int id)
        {
            var beneficio = await _context.Set<Beneficio>().FindAsync(id);
            if (beneficio == null)
                return NotFound(new { message = "Beneficio no encontrado" });

            beneficio.Activo = false;
            _context.Set<Beneficio>().Update(beneficio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
