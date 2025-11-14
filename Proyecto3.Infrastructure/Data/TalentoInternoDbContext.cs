// Proyecto3.Infrastructure/Data/TalentoInternoDbContext.cs

using Microsoft.EntityFrameworkCore;
using Proyecto3.Core.Entities; // Importa las entidades que acabas de crear

namespace Proyecto3.Infrastructure.Data
{
    public class TalentoInternoDbContext : DbContext
    {
        // Constructor: Recibe las opciones de conexión (configuradas en Program.cs)
        public TalentoInternoDbContext(DbContextOptions<TalentoInternoDbContext> options)
            : base(options)
        {
        }

        // --- 1. Definición de Tablas (DbSet) ---

        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Certificacion> Certificaciones { get; set; }
        public DbSet<PerfilesVacante> PerfilesVacante { get; set; }

        // Tablas de Unión
        public DbSet<HabilidadColaborador> HabilidadesColaboradores { get; set; }
        public DbSet<RequisitoVacante> RequisitosVacante { get; set; }

        // --- 2. Mapeo Avanzado (Claves Compuestas/Relaciones) ---

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de Clave Compuesta para HabilidadColaborador
            // (Relación Muchos-a-Muchos entre Colaborador y Skill)
            modelBuilder.Entity<HabilidadColaborador>()
                .HasKey(hc => new { hc.ColaboradorID, hc.SkillID });

            // Configuración de las FKs y sus restricciones para HabilidadColaborador
            modelBuilder.Entity<HabilidadColaborador>()
                .HasOne(hc => hc.Colaborador)
                .WithMany(c => c.Habilidades)
                .HasForeignKey(hc => hc.ColaboradorID);

            modelBuilder.Entity<HabilidadColaborador>()
                .HasOne(hc => hc.Skill)
                .WithMany(s => s.HabilidadesColaboradores)
                .HasForeignKey(hc => hc.SkillID);


            // Configuración de Clave Única para RequisitoVacante
            // Si la usaste como tabla de unión con clave compuesta, usa esta línea:
            /*
            modelBuilder.Entity<RequisitoVacante>()
                .HasKey(rv => new { rv.VacanteID, rv.SkillID });
            */
            // Pero si usaste RequisitoID como PK simple (como en tu entidad):
            modelBuilder.Entity<RequisitoVacante>().HasKey(rv => rv.RequisitoID);

            base.OnModelCreating(modelBuilder);
        }
    }
}