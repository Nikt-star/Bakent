using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Proyecto3.Core.Entities
{
    public class PerfilesVacante
    {
        [Key]
        public int VacanteID { get; set; }
        public string NombrePerfil { get; set; }
        public string NivelDeseado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaUrgencia { get; set; }
        public bool Activa { get; set; }

        // Propiedad de Navegación (Relación con RequisitosVacante)
        public ICollection<RequisitoVacante> RequisitosVacante { get; set; }

        public PerfilesVacante()
        {
            RequisitosVacante = new HashSet<RequisitoVacante>();
        }
    }
}
