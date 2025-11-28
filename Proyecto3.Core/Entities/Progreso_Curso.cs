using System;

namespace Proyecto3.Core.Entities
{
    public class Progreso_Curso
    {
        public int ProgresoID { get; set; }
        public int ColaboradorID { get; set; }
        public int CursoID { get; set; }
        public decimal PorcentajeCompletacion { get; set; }
        public bool Completado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaCompletacion { get; set; }

        // Navegación
        public Colaborador? Colaborador { get; set; }
        public Curso? Curso { get; set; }
    }
}
