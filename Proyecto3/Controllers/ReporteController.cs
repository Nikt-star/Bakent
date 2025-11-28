// Proyecto3/Controllers/ReporteController.cs

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
    public class ReporteController : ControllerBase
    {
        private readonly TalentoInternoDbContext _context;

        public ReporteController(TalentoInternoDbContext context)
        {
            _context = context;
        }

        // ===== ENDPOINTS DE INVENTARIO DE SKILLS =====

        // [GET] api/reporte/inventario-skills - UC-I1: Visualización Interactiva del Inventario
        [HttpGet("inventario-skills")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetInventarioSkills()
        {
            var inventario = await _context.Set<HabilidadColaborador>()
                .Include(h => h.Skill)
                .Include(h => h.Colaborador)
                .GroupBy(h => new { h.Skill.NombreSkill, h.NivelDominio })
                .Select(g => new
                {
                    Skill = g.Key.NombreSkill,
                    Nivel = g.Key.NivelDominio,
                    Cantidad = g.Count(),
                    Colaboradores = g.Select(h => h.Colaborador != null ? h.Colaborador.NombreCompleto : "N/A").ToList()
                })
                .ToListAsync();

            return Ok(inventario);
        }

        // [GET] api/reporte/inventario-skills/por-area - Inventario por área/rol
        [HttpGet("inventario-skills/por-area")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetInventarioSkillsPorArea()
        {
            var inventario = await _context.Set<HabilidadColaborador>()
                .Include(h => h.Skill)
                .Include(h => h.Colaborador)
                .GroupBy(h => h.Colaborador != null ? h.Colaborador.RolActual : "N/A")
                .Select(g => new
                {
                    Area = g.Key,
                    TotalColaboradores = g.Select(h => h.ColaboradorID).Distinct().Count(),
                    Skills = g.GroupBy(h => h.Skill != null ? h.Skill.NombreSkill : "N/A")
                        .Select(sg => new
                        {
                            Skill = sg.Key,
                            Disponibles = sg.Count(),
                            NivelesDominio = sg.GroupBy(h => h.NivelDominio)
                                .Select(ng => new { Nivel = ng.Key, Cantidad = ng.Count() })
                                .ToList()
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(inventario);
        }

        // [GET] api/reporte/inventario-skills/por-nivel - Inventario por nivel
        [HttpGet("inventario-skills/por-nivel")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetInventarioSkillsPorNivel()
        {
            var nivelesValidos = new[] { "Básico", "Intermedio", "Avanzado", "Experto" };

            var inventario = await _context.Set<HabilidadColaborador>()
                .Include(h => h.Skill)
                .Include(h => h.Colaborador)
                .Where(h => nivelesValidos.Contains(h.NivelDominio))
                .GroupBy(h => h.NivelDominio)
                .Select(g => new
                {
                    Nivel = g.Key,
                    TotalColaboradores = g.Select(h => h.ColaboradorID).Distinct().Count(),
                    Skills = g.GroupBy(h => h.Skill != null ? h.Skill.NombreSkill : "N/A")
                        .Select(sg => new
                        {
                            Skill = sg.Key,
                            Cantidad = sg.Count()
                        })
                        .OrderByDescending(sg => sg.Cantidad)
                        .ToList()
                })
                .ToListAsync();

            return Ok(inventario);
        }

        // ===== ENDPOINTS DE BRECHAS DE SKILLS =====

        // [GET] api/reporte/brechas - UC-B1: Generar Reporte de Brechas de Skills Críticos
        [HttpGet("brechas")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetReporteBrechas()
        {
            // Obtener todos los requisitos de vacantes activas
            var requisitosVacantes = await _context.Set<RequisitoVacante>()
                .Include(r => r.Vacante)
                .Include(r => r.Skill)
                .Where(r => r.Vacante != null && r.Vacante.Activa)
                .GroupBy(r => r.Skill != null ? r.Skill.NombreSkill : "N/A")
                .Select(g => new
                {
                    Skill = g.Key,
                    Demanda = g.Count(),
                    NivelMinimo = g.First().NivelMinimoRequerido
                })
                .ToListAsync();

            var brechas = new List<dynamic>();

            foreach (var req in requisitosVacantes)
            {
                // Contar disponibilidad de colaboradores con ese skill en el nivel requerido
                var disponibles = await _context.Set<HabilidadColaborador>()
                    .Include(h => h.Skill)
                    .Where(h => h.Skill != null && h.Skill.NombreSkill == req.Skill && 
                                ObtenerPuntuacion(h.NivelDominio) >= ObtenerPuntuacion(req.NivelMinimo))
                    .CountAsync();

                var brecha = req.Demanda - disponibles;
                var severidad = brecha > req.Demanda * 0.5 ? "Crítica" : 
                               brecha > req.Demanda * 0.3 ? "Alta" : 
                               brecha > 0 ? "Media" : "Baja";

                brechas.Add(new
                {
                    Skill = req.Skill,
                    Demanda = req.Demanda,
                    Disponibilidad = disponibles,
                    Brecha = brecha,
                    Severidad = severidad,
                    NivelRequerido = req.NivelMinimo
                });
            }

            return Ok(brechas.OrderByDescending(b => b.Brecha).ToList());
        }

        // [GET] api/reporte/brechas/por-rol - Brechas por rol/área
        [HttpGet("brechas/por-rol")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetBrechasPorRol()
        {
            var roles = await _context.Colaboradores
                .Select(c => c.RolActual)
                .Distinct()
                .ToListAsync();

            var brechasPorRol = new List<dynamic>();

            foreach (var rol in roles)
            {
                var colaboradoresDelRol = await _context.Colaboradores
                    .Where(c => c.RolActual == rol)
                    .Select(c => c.ColaboradorID)
                    .ToListAsync();

                var skillsDelRol = await _context.Set<HabilidadColaborador>()
                    .Include(h => h.Skill)
                    .Where(h => colaboradoresDelRol.Contains(h.ColaboradorID))
                    .GroupBy(h => h.Skill != null ? h.Skill.NombreSkill : "N/A")
                    .Select(g => new { Skill = g.Key, Disponibilidad = g.Count() })
                    .ToListAsync();

                brechasPorRol.Add(new
                {
                    Rol = rol,
                    TotalColaboradores = colaboradoresDelRol.Count,
                    TotalSkillsUnicos = skillsDelRol.Count,
                    Skills = skillsDelRol
                });
            }

            return Ok(brechasPorRol);
        }

        // [GET] api/reporte/alertas - UC-B2: Generar Alertas de Brechas Críticas
        [HttpGet("alertas")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAlertas()
        {
            // Obtener brechas críticas
            var requisitosVacantes = await _context.Set<RequisitoVacante>()
                .Include(r => r.Vacante)
                .Include(r => r.Skill)
                .Where(r => r.Vacante != null && r.Vacante.Activa)
                .GroupBy(r => r.Skill != null ? r.Skill.NombreSkill : "N/A")
                .Select(g => new
                {
                    Skill = g.Key,
                    Demanda = g.Count(),
                    NivelMinimo = g.First().NivelMinimoRequerido
                })
                .ToListAsync();

            var alertas = new List<dynamic>();

            foreach (var req in requisitosVacantes)
            {
                var disponibles = await _context.Set<HabilidadColaborador>()
                    .Include(h => h.Skill)
                    .Where(h => h.Skill != null && h.Skill.NombreSkill == req.Skill && 
                                ObtenerPuntuacion(h.NivelDominio) >= ObtenerPuntuacion(req.NivelMinimo))
                    .CountAsync();

                var brecha = req.Demanda - disponibles;

                // Solo alertas críticas (más del 50% de brecha)
                if (brecha > req.Demanda * 0.5)
                {
                    alertas.Add(new
                    {
                        Skill = req.Skill,
                        Severidad = "CRÍTICA",
                        Mensaje = $"Brecha crítica en {req.Skill}: necesitamos {brecha} profesionales con nivel {req.NivelMinimo}",
                        Porcentaje = Math.Round((brecha / (double)req.Demanda) * 100, 2),
                        FechaAlerta = DateTime.UtcNow
                    });
                }
            }

            return Ok(alertas.OrderByDescending(a => a.Porcentaje).ToList());
        }

        // ===== ENDPOINTS DE EXPORTACIÓN =====

        // [GET] api/reporte/exportar-brechas-csv - Exportar reporte de brechas en CSV
        [HttpGet("exportar-brechas-csv")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ExportarBrechasCSV()
        {
            var requisitosVacantes = await _context.Set<RequisitoVacante>()
                .Include(r => r.Vacante)
                .Include(r => r.Skill)
                .Where(r => r.Vacante != null && r.Vacante.Activa)
                .GroupBy(r => r.Skill != null ? r.Skill.NombreSkill : "N/A")
                .Select(g => new
                {
                    Skill = g.Key,
                    Demanda = g.Count(),
                    NivelMinimo = g.First().NivelMinimoRequerido
                })
                .ToListAsync();

            var csv = "Skill,Demanda,Disponibilidad,Brecha,Severidad,Nivel_Requerido\n";

            foreach (var req in requisitosVacantes)
            {
                var disponibles = await _context.Set<HabilidadColaborador>()
                    .Include(h => h.Skill)
                    .Where(h => h.Skill != null && h.Skill.NombreSkill == req.Skill && 
                                ObtenerPuntuacion(h.NivelDominio) >= ObtenerPuntuacion(req.NivelMinimo))
                    .CountAsync();

                var brecha = req.Demanda - disponibles;
                var severidad = brecha > req.Demanda * 0.5 ? "Crítica" : 
                               brecha > req.Demanda * 0.3 ? "Alta" : 
                               brecha > 0 ? "Media" : "Baja";

                csv += $"{req.Skill},{req.Demanda},{disponibles},{brecha},{severidad},{req.NivelMinimo}\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "reporte_brechas.csv");
        }

        // [GET] api/reporte/dashboard - Dashboard resumen
        [HttpGet("dashboard")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetDashboard()
        {
            var totalColaboradores = await _context.Colaboradores.CountAsync();
            var totalSkills = await _context.Set<Skill>().CountAsync();
            var totalVacantes = await _context.Set<PerfilesVacante>().CountAsync();
            var vacantesActivas = await _context.Set<PerfilesVacante>().Where(v => v.Activa).CountAsync();
            var totalAplicaciones = await _context.Set<Vacante_Aplicacion>().CountAsync();
            var perfilesCompletos = await _context.Set<Perfil>().Where(p => p.PerfilCompleto).CountAsync();

            return Ok(new
            {
                TotalColaboradores = totalColaboradores,
                TotalSkills = totalSkills,
                TotalVacantes = totalVacantes,
                VacantesActivas = vacantesActivas,
                TotalAplicaciones = totalAplicaciones,
                PerfilesCompletos = perfilesCompletos,
                FechaReporte = DateTime.UtcNow
            });
        }

        // Método auxiliar para convertir niveles a puntuación
        private static int ObtenerPuntuacion(string nivel) => nivel switch
        {
            "Básico" => 1,
            "Intermedio" => 2,
            "Avanzado" => 3,
            "Experto" => 4,
            _ => 0,
        };
    }
}
