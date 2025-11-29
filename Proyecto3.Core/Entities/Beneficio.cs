using System;

namespace Proyecto3.Core.Entities
{
    public class Beneficio
    {
        public int BeneficioID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; } // "Salud", "Financiero", "Laboral", "Otros"
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
