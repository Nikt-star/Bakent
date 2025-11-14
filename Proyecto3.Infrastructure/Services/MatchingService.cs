// Proyecto3.Infrastructure/Services/MatchingService.cs

using Proyecto3.Core.Entities;
using Proyecto3.Core.Interfaces;
using Proyecto3.Core.Models;
using Proyecto3.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto3.Infrastructure.Services
{
    public class MatchingService : IMatchingService
    {
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly IVacanteRepository _vacanteRepository;

        public MatchingService(IColaboradorRepository colaboradorRepository, IVacanteRepository vacanteRepository)
        {
            _colaboradorRepository = colaboradorRepository;
            _vacanteRepository = vacanteRepository;
        }

        // Mapeo de Niveles de Dominio a Puntuación Numérica
        private static int GetPuntuacion(string nivel) => nivel switch
        {
            "Básico" => 1,
            "Intermedio" => 2,
            "Avanzado" => 3,
            "Experto" => 4,
            _ => 0,
        };

        // UC-M1 y UC-B1: Algoritmo de Matching
        public async Task<IEnumerable<MatchResult>> FindCandidatesForVacancyAsync(int vacanteId)
        {
            // 1. Obtener la vacante y sus requisitos (usa el método especializado)
            var vacante = await _vacanteRepository.GetByIdWithRequisitosAsync(vacanteId);
            if (vacante == null) return Enumerable.Empty<MatchResult>();

            // 2. Obtener todos los colaboradores y sus habilidades (usa el método especializado)
            var colaboradores = await _colaboradorRepository.GetAllWithHabilidadesAsync();

            var results = new List<MatchResult>();

            foreach (var colaborador in colaboradores)
            {
                double skillsCumplidos = 0;
                double totalSkillsRequeridos = vacante.RequisitosVacante.Count;
                var skillsFaltantes = new List<string>();

                foreach (var requisito in vacante.RequisitosVacante)
                {
                    var habilidadColaborador = colaborador.Habilidades
                        .FirstOrDefault(h => h.SkillID == requisito.SkillID);

                    if (habilidadColaborador != null)
                    {
                        int puntajeColaborador = GetPuntuacion(habilidadColaborador.NivelDominio);
                        int puntajeRequerido = GetPuntuacion(requisito.NivelMinimoRequerido);

                        // Si el puntaje del colaborador es igual o superior al requerido, lo cumple.
                        if (puntajeColaborador >= puntajeRequerido)
                        {
                            skillsCumplidos++;
                        }
                        else
                        {
                            skillsFaltantes.Add($"{requisito.Skill.NombreSkill} (Tiene: {habilidadColaborador.NivelDominio}, Requiere: {requisito.NivelMinimoRequerido})");
                        }
                    }
                    else
                    {
                        // No tiene la habilidad registrada
                        skillsFaltantes.Add($"{requisito.Skill.NombreSkill} (No registrada)");
                    }
                }

                // Cálculo del Porcentaje de Match
                double porcentajeMatch = (totalSkillsRequeridos > 0)
                    ? (skillsCumplidos / totalSkillsRequeridos) * 100
                    : 0;

                // Solo si cumple un 50% o más (Regla de negocio simple)
                if (porcentajeMatch >= 50)
                {
                    results.Add(new MatchResult
                    {
                        ColaboradorID = colaborador.ColaboradorID,
                        NombreCompleto = colaborador.NombreCompleto,
                        PorcentajeMatch = Math.Round(porcentajeMatch, 2),
                        SkillsFaltantes = skillsFaltantes
                    });
                }
            }

            // Devolver la lista ordenada por mayor porcentaje de match
            return results.OrderByDescending(r => r.PorcentajeMatch);
        }
    }
}