using System;
using System.Linq;
using System.Threading.Tasks;
using Proyecto3.Infrastructure.Data;
using Proyecto3.Core.Entities;
using System.Collections.Generic;

namespace Proyecto3.Infrastructure.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(TalentoInternoDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Evitar reseed
            if (context.Skills.Any() || context.Colaboradores.Any() || context.PerfilesVacante.Any())
            {
                return;
            }

            // 1. Skills
            var skillNet = new Skill { NombreSkill = "C#", TipoSkill = "Técnico" };
            var skillSql = new Skill { NombreSkill = "SQL Server", TipoSkill = "Técnico" };
            var skillAngular = new Skill { NombreSkill = "Angular", TipoSkill = "Técnico" };
            var skillComunicacion = new Skill { NombreSkill = "Comunicación", TipoSkill = "Blando" };

            context.Skills.AddRange(skillNet, skillSql, skillAngular, skillComunicacion);
            await context.SaveChangesAsync();

            // 2. Colaboradores con Habilidades y Certificaciones
            var colaborador1 = new Colaborador
            {
                NombreCompleto = "María Pérez",
                RolActual = "Desarrolladora Backend",
                CuentaProyecto = "marperez",
                DisponibilidadMovilidad = true,
                Habilidades = new List<HabilidadColaborador>
                {
                    new HabilidadColaborador { Skill = skillNet, NivelDominio = "Avanzado" },
                    new HabilidadColaborador { Skill = skillSql, NivelDominio = "Intermedio" }
                },
                Certificaciones = new List<Certificacion>
                {
                    new Certificacion { NombreCertificacion = "Azure Developer Associate", FechaObtencion = DateTime.UtcNow.AddYears(-1), Institucion = "Microsoft" }
                }
            };

            var colaborador2 = new Colaborador
            {
                NombreCompleto = "Juan García",
                RolActual = "Desarrollador Frontend",
                CuentaProyecto = "juang",
                DisponibilidadMovilidad = false,
                Habilidades = new List<HabilidadColaborador>
                {
                    new HabilidadColaborador { Skill = skillAngular, NivelDominio = "Intermedio" },
                    new HabilidadColaborador { Skill = skillComunicacion, NivelDominio = "Avanzado" }
                },
                Certificaciones = new List<Certificacion>()
            };

            context.Colaboradores.AddRange(colaborador1, colaborador2);
            await context.SaveChangesAsync();

            // 3. Vacante y Requisitos
            var vacante = new PerfilesVacante
            {
                NombrePerfil = "Desarrollador FullStack",
                NivelDeseado = "Intermedio",
                FechaInicio = DateTime.UtcNow,
                FechaUrgencia = DateTime.UtcNow.AddMonths(1),
                Activa = true,
                RequisitosVacante = new List<RequisitoVacante>
                {
                    new RequisitoVacante { Skill = skillNet, NivelMinimoRequerido = "Intermedio" },
                    new RequisitoVacante { Skill = skillAngular, NivelMinimoRequerido = "Básico" },
                    new RequisitoVacante { Skill = skillSql, NivelMinimoRequerido = "Intermedio" }
                }
            };

            context.PerfilesVacante.Add(vacante);
            await context.SaveChangesAsync();
        }
    }
}
