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

        // Entidades Principales
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Certificacion> Certificaciones { get; set; }
        public DbSet<PerfilesVacante> PerfilesVacante { get; set; }

        // Tablas de Unión
        public DbSet<HabilidadColaborador> HabilidadesColaboradores { get; set; }
        public DbSet<RequisitoVacante> RequisitosVacante { get; set; }

        // Nuevas Entidades para Funcionalidades Completas
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Vacante_Aplicacion> VacanteAplicaciones { get; set; }
        public DbSet<Beneficio> Beneficios { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Punto> Puntos { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Progreso_Curso> ProgresoCursos { get; set; }

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


            // Configuración de RequisitoVacante
            modelBuilder.Entity<RequisitoVacante>().HasKey(rv => rv.RequisitoID);

            // Configuración de Perfil (1-1 con Colaborador)
            modelBuilder.Entity<Perfil>()
                .HasOne(p => p.Colaborador)
                .WithMany()
                .HasForeignKey(p => p.ColaboradorID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de Vacante_Aplicacion: definir clave primaria y relaciones
            modelBuilder.Entity<Vacante_Aplicacion>()
                .HasKey(va => va.AplicacionID);

            modelBuilder.Entity<Vacante_Aplicacion>()
                .HasOne(va => va.Vacante)
                .WithMany()
                .HasForeignKey(va => va.VacanteID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vacante_Aplicacion>()
                .HasOne(va => va.Colaborador)
                .WithMany()
                .HasForeignKey(va => va.ColaboradorID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de Notificacion
            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Colaborador)
                .WithMany()
                .HasForeignKey(n => n.ColaboradorID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de Punto
            modelBuilder.Entity<Punto>()
                .HasOne(p => p.Colaborador)
                .WithMany()
                .HasForeignKey(p => p.ColaboradorID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de Progreso_Curso
            modelBuilder.Entity<Progreso_Curso>()
                .HasKey(pc => pc.ProgresoID);

            // Especificar precisión para evitar truncamiento en SQL Server
            modelBuilder.Entity<Progreso_Curso>()
                .Property(pc => pc.PorcentajeCompletacion)
                .HasPrecision(5, 2); // soporta 0.00 - 999.99; ajusta si necesitas otro rango

            modelBuilder.Entity<Progreso_Curso>()
                .HasOne(pc => pc.Colaborador)
                .WithMany()
                .HasForeignKey(pc => pc.ColaboradorID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Progreso_Curso>()
                .HasOne(pc => pc.Curso)
                .WithMany()
                .HasForeignKey(pc => pc.CursoID)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}