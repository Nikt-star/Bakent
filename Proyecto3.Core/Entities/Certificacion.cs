using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto3.Core.Entities
{
    public class Certificacion
    {
        public int CertificacionID { get; set; }
        public int ColaboradorID { get; set; } // Foreign Key (FK)
        public string NombreCertificacion { get; set; }
        public DateTime? FechaObtencion { get; set; } // Nullable
        public string Institucion { get; set; }

        // Propiedad de Navegación para la FK (Uno a Muchos)
        public Colaborador? Colaborador { get; set; }
    }
}
