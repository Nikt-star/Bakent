using System;
using System.Collections.Generic;

namespace Proyecto3.Core.Entities
{
    public class Vacante_Aplicacion
    {
        public int AplicacionID { get; set; }
        public int VacanteID { get; set; }
        public int ColaboradorID { get; set; }
        public DateTime FechaAplicacion { get; set; }
        public string Estado { get; set; } // "Aplicado", "Interesado", "En_Evaluacion", "Rechazado", "Seleccionado"
        public string? Interes { get; set; } // "Alto", "Medio", "Bajo"
        public string? Notas { get; set; }

        // Navegación
        public PerfilesVacante? Vacante { get; set; }
        public Colaborador? Colaborador { get; set; }
    }
}
