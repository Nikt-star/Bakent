using Proyecto3.Core.Entities;
using Proyecto3.Core.Interfaces;
using Proyecto3.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proyecto3.Core.Models;
using System.Linq;
using System;

namespace Proyecto3.Infrastructure.Services
{
    public class ColaboradorService : IColaboradorService
    {
        private readonly TalentoInternoDbContext _context;

        public ColaboradorService(TalentoInternoDbContext context)
        {
            _context = context;
        }

        public async Task<Colaborador> CreateColaboradorAsync(Colaborador colaborador)
        {
            // Procesar habilidades: pueden venir con SkillID o con Skill.NombreSkill
            if (colaborador.Habilidades != null)
            {
                var processed = new List<HabilidadColaborador>();
                foreach (var hc in colaborador.Habilidades)
                {
                    Skill? skill = null;

                    if (hc.SkillID != 0)
                    {
                        skill = await _context.Skills.FindAsync(hc.SkillID);
                    }

                    if (skill == null && hc.Skill != null && !string.IsNullOrWhiteSpace(hc.Skill.NombreSkill))
                    {
                        // Buscar por nombre (case-insensitive)
                        skill = await _context.Skills
                            .FirstOrDefaultAsync(s => s.NombreSkill.ToLower() == hc.Skill.NombreSkill.ToLower());

                        if (skill == null)
                        {
                            // Crear nueva skill si no existe
                            skill = new Skill
                            {
                                NombreSkill = hc.Skill.NombreSkill,
                                TipoSkill = hc.Skill.TipoSkill ?? "Técnico"
                            };
                            _context.Skills.Add(skill);
                            await _context.SaveChangesAsync();
                        }
                    }

                    // Construir la habilidad a guardar
                    var newHc = new HabilidadColaborador
                    {
                        SkillID = skill != null ? skill.SkillID : hc.SkillID,
                        NivelDominio = hc.NivelDominio,
                        FechaUltimaEvaluacion = hc.FechaUltimaEvaluacion,
                        Skill = skill
                    };

                    processed.Add(newHc);
                }

                colaborador.Habilidades = processed;
            }

            // Asegurar que las certificaciones no traigan referencias circulares
            if (colaborador.Certificaciones != null)
            {
                foreach (var cert in colaborador.Certificaciones)
                {
                    cert.Colaborador = null;
                }
            }

            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();
            return colaborador;
        }

        public async Task<IEnumerable<Colaborador>> GetAllColaboradoresAsync()
        {
            return await _context.Colaboradores.ToListAsync();
        }

        public async Task<Colaborador> GetColaboradorDetailsAsync(int id)
        {
            var col = await _context.Colaboradores
                .Include(c => c.Habilidades)
                    .ThenInclude(h => h.Skill)
                .Include(c => c.Certificaciones)
                .FirstOrDefaultAsync(c => c.ColaboradorID == id);

            if (col == null) throw new KeyNotFoundException();
            return col;
        }

        public async Task UpdateColaboradorAsync(Colaborador colaborador)
        {
            _context.Entry(colaborador).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteColaboradorAsync(int id)
        {
            var col = await _context.Colaboradores.FindAsync(id);
            if (col != null)
            {
                _context.Colaboradores.Remove(col);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Colaborador> CreateFromDtoAsync(ColaboradorCreateDto dto)
        {
            var colaborador = new Colaborador
            {
                NombreCompleto = dto.NombreCompleto,
                RolActual = dto.RolActual,
                CuentaProyecto = dto.CuentaProyecto,
                DisponibilidadMovilidad = dto.DisponibilidadMovilidad,
                Habilidades = new List<HabilidadColaborador>(),
                Certificaciones = new List<Certificacion>()
            };

            if (dto.Habilidades != null)
            {
                foreach (var h in dto.Habilidades)
                {
                    Skill? skill = null;
                    if (h.SkillID.HasValue && h.SkillID.Value != 0)
                    {
                        skill = await _context.Skills.FindAsync(h.SkillID.Value);
                    }
                    if (skill == null && !string.IsNullOrWhiteSpace(h.SkillNombre))
                    {
                        skill = await _context.Skills.FirstOrDefaultAsync(s => s.NombreSkill.ToLower() == h.SkillNombre.ToLower());
                        if (skill == null)
                        {
                            skill = new Skill { NombreSkill = h.SkillNombre, TipoSkill = h.TipoSkill ?? "Técnico" };
                            _context.Skills.Add(skill);
                            await _context.SaveChangesAsync();
                        }
                    }

                    colaborador.Habilidades.Add(new HabilidadColaborador
                    {
                        SkillID = skill != null ? skill.SkillID : (h.SkillID ?? 0),
                        NivelDominio = h.NivelDominio,
                        FechaUltimaEvaluacion = h.FechaUltimaEvaluacion,
                        Skill = skill
                    });
                }
            }

            if (dto.Certificaciones != null)
            {
                foreach (var c in dto.Certificaciones)
                {
                    colaborador.Certificaciones.Add(new Certificacion
                    {
                        NombreCertificacion = c.NombreCertificacion,
                        FechaObtencion = c.FechaObtencion,
                        Institucion = c.Institucion
                    });
                }
            }

            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();
            return colaborador;
        }

        public async Task AddHabilidadesAndCertsAsync(int colaboradorId, ColaboradorUpdateDto dto)
        {
            var colaborador = await _context.Colaboradores
                .Include(c => c.Habilidades)
                .Include(c => c.Certificaciones)
                .FirstOrDefaultAsync(c => c.ColaboradorID == colaboradorId);

            if (colaborador == null) throw new KeyNotFoundException();

            if (dto.Habilidades != null)
            {
                foreach (var h in dto.Habilidades)
                {
                    Skill? skill = null;
                    if (h.SkillID.HasValue && h.SkillID.Value != 0)
                    {
                        skill = await _context.Skills.FindAsync(h.SkillID.Value);
                    }
                    if (skill == null && !string.IsNullOrWhiteSpace(h.SkillNombre))
                    {
                        skill = await _context.Skills.FirstOrDefaultAsync(s => s.NombreSkill.ToLower() == h.SkillNombre.ToLower());
                        if (skill == null)
                        {
                            skill = new Skill { NombreSkill = h.SkillNombre, TipoSkill = h.TipoSkill ?? "Técnico" };
                            _context.Skills.Add(skill);
                            await _context.SaveChangesAsync();
                        }
                    }

                    var newH = new HabilidadColaborador
                    {
                        SkillID = skill != null ? skill.SkillID : (h.SkillID ?? 0),
                        NivelDominio = h.NivelDominio,
                        FechaUltimaEvaluacion = h.FechaUltimaEvaluacion,
                        Skill = skill
                    };

                    colaborador.Habilidades.Add(newH);
                }
            }

            if (dto.Certificaciones != null)
            {
                foreach (var c in dto.Certificaciones)
                {
                    colaborador.Certificaciones.Add(new Certificacion
                    {
                        NombreCertificacion = c.NombreCertificacion,
                        FechaObtencion = c.FechaObtencion,
                        Institucion = c.Institucion
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
