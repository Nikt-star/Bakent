// Proyecto3/Controllers/DesarrolloController.cs

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
    public class DesarrolloController : ControllerBase
    {
        private readonly TalentoInternoDbContext _context;

        public DesarrolloController(TalentoInternoDbContext context)
        {
            _context = context;
        }

        // ===== ENDPOINTS DE CURSOS =====

        // [GET] api/desarrollo/cursos - UC-D1: Listar Cursos Relevantes
        [HttpGet("cursos")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Curso>))]
        public async Task<IActionResult> GetCursos()
        {
            var cursos = await _context.Set<Curso>()
                .Where(c => c.Activo)
                .ToListAsync();

            return Ok(cursos);
        }

        // [GET] api/desarrollo/cursos/categoria/{categoria} - Listar cursos por categoría
        [HttpGet("cursos/categoria/{categoria}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Curso>))]
        public async Task<IActionResult> GetCursosPorCategoria(string categoria)
        {
            var cursos = await _context.Set<Curso>()
                .Where(c => c.Activo && c.Categoria == categoria)
                .ToListAsync();

            return Ok(cursos);
        }

        // [GET] api/desarrollo/cursos/{id} - Obtener detalle de un curso
        [HttpGet("cursos/{id}")]
        [ProducesResponseType(200, Type = typeof(Curso))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCurso(int id)
        {
            var curso = await _context.Set<Curso>().FindAsync(id);
            if (curso == null)
                return NotFound(new { message = "Curso no encontrado" });

            return Ok(curso);
        }

        // [POST] api/desarrollo/cursos - Crear Curso (Admin)
        [HttpPost("cursos")]
        [ProducesResponseType(201, Type = typeof(Curso))]
        public async Task<IActionResult> CreateCurso([FromBody] CursoCreateDto dto)
        {
            var curso = new Curso
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Categoria = dto.Categoria,
                DuracionHoras = dto.DuracionHoras,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Set<Curso>().Add(curso);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCurso), new { id = curso.CursoID }, curso);
        }

        // [PUT] api/desarrollo/cursos/{id} - Actualizar Curso (Admin)
        [HttpPut("cursos/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCurso(int id, [FromBody] CursoCreateDto dto)
        {
            var curso = await _context.Set<Curso>().FindAsync(id);
            if (curso == null)
                return NotFound(new { message = "Curso no encontrado" });

            curso.Nombre = dto.Nombre ?? curso.Nombre;
            curso.Descripcion = dto.Descripcion ?? curso.Descripcion;
            curso.Categoria = dto.Categoria ?? curso.Categoria;
            curso.DuracionHoras = dto.DuracionHoras > 0 ? dto.DuracionHoras : curso.DuracionHoras;

            _context.Set<Curso>().Update(curso);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ===== ENDPOINTS DE PROGRESO =====

        // [GET] api/desarrollo/progreso/{colaboradorId} - UC-D2: Visualizar Progreso de Cursos
        [HttpGet("progreso/{colaboradorId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Progreso_Curso>))]
        public async Task<IActionResult> GetProgresoCursos(int colaboradorId)
        {
            var progresos = await _context.Set<Progreso_Curso>()
                .Include(p => p.Curso)
                .Where(p => p.ColaboradorID == colaboradorId)
                .ToListAsync();

            return Ok(progresos);
        }

        // [GET] api/desarrollo/progreso/{colaboradorId}/completados - Cursos completados
        [HttpGet("progreso/{colaboradorId}/completados")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Progreso_Curso>))]
        public async Task<IActionResult> GetCursosCompletados(int colaboradorId)
        {
            var progresos = await _context.Set<Progreso_Curso>()
                .Include(p => p.Curso)
                .Where(p => p.ColaboradorID == colaboradorId && p.Completado)
                .ToListAsync();

            return Ok(progresos);
        }

        // [GET] api/desarrollo/progreso/{colaboradorId}/en-progreso - Cursos en progreso
        [HttpGet("progreso/{colaboradorId}/en-progreso")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Progreso_Curso>))]
        public async Task<IActionResult> GetCursosEnProgreso(int colaboradorId)
        {
            var progresos = await _context.Set<Progreso_Curso>()
                .Include(p => p.Curso)
                .Where(p => p.ColaboradorID == colaboradorId && !p.Completado)
                .ToListAsync();

            return Ok(progresos);
        }

        // [POST] api/desarrollo/progreso - Iniciar Curso
        [HttpPost("progreso")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> InitiarCurso([FromBody] Progreso_CursoCreateDto dto)
        {
            // Verificar que el colaborador existe
            var colaborador = await _context.Colaboradores.FindAsync(dto.ColaboradorID);
            if (colaborador == null)
                return NotFound(new { message = "Colaborador no encontrado" });

            // Verificar que el curso existe
            var curso = await _context.Set<Curso>().FindAsync(dto.CursoID);
            if (curso == null)
                return NotFound(new { message = "Curso no encontrado" });

            // Verificar que no exista un progreso previo
            var existente = await _context.Set<Progreso_Curso>()
                .FirstOrDefaultAsync(p => p.ColaboradorID == dto.ColaboradorID && p.CursoID == dto.CursoID);
            
            if (existente != null)
                return Conflict(new { message = "El colaborador ya inició este curso" });

            var progreso = new Progreso_Curso
            {
                ColaboradorID = dto.ColaboradorID,
                CursoID = dto.CursoID,
                PorcentajeCompletacion = 0,
                Completado = false,
                FechaInicio = DateTime.UtcNow,
                FechaCompletacion = null
            };

            _context.Set<Progreso_Curso>().Add(progreso);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProgresoCursos), new { colaboradorId = dto.ColaboradorID }, progreso);
        }

        // [PUT] api/desarrollo/progreso/{progresoId} - UC-D2: Actualizar Progreso
        [HttpPut("progreso/{progresoId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProgreso(int progresoId, [FromBody] Progreso_CursoUpdateDto dto)
        {
            var progreso = await _context.Set<Progreso_Curso>().FindAsync(progresoId);
            if (progreso == null)
                return NotFound(new { message = "Progreso no encontrado" });

            progreso.PorcentajeCompletacion = dto.PorcentajeCompletacion;
            progreso.Completado = dto.Completado;

            if (dto.Completado && progreso.FechaCompletacion == null)
            {
                progreso.FechaCompletacion = DateTime.UtcNow;
                progreso.PorcentajeCompletacion = 100;
            }

            _context.Set<Progreso_Curso>().Update(progreso);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [GET] api/desarrollo/estadisticas/{colaboradorId} - Estadísticas de desarrollo
        [HttpGet("estadisticas/{colaboradorId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetEstadisticasDesarrollo(int colaboradorId)
        {
            var totalCursos = await _context.Set<Progreso_Curso>()
                .Where(p => p.ColaboradorID == colaboradorId)
                .CountAsync();

            var cursosCompletados = await _context.Set<Progreso_Curso>()
                .Where(p => p.ColaboradorID == colaboradorId && p.Completado)
                .CountAsync();

            var horasCompletadas = await _context.Set<Progreso_Curso>()
                .Include(p => p.Curso)
                .Where(p => p.ColaboradorID == colaboradorId && p.Completado)
                .SumAsync(p => p.Curso != null ? p.Curso.DuracionHoras : 0);

            var promedioCompletacion = totalCursos > 0
                ? await _context.Set<Progreso_Curso>()
                    .Where(p => p.ColaboradorID == colaboradorId)
                    .AverageAsync(p => p.PorcentajeCompletacion)
                : 0;

            return Ok(new
            {
                TotalCursos = totalCursos,
                CursosCompletados = cursosCompletados,
                HorasCompletadas = horasCompletadas,
                PromedioCompletacion = Math.Round(promedioCompletacion, 2)
            });
        }
    }
}
