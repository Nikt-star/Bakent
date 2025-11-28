using System;

namespace Proyecto3.Core.Entities
{
    public class Curso
    {
        public int CursoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int DuracionHoras { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
