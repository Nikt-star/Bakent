using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto3.Core.Entities
{
    public class RequisitoVacante
    {
        // Clave Principal Compuesta (Se define en el DbContext)
        public int RequisitoID { get; set; } // Clave primaria de la tabla
        public int VacanteID { get; set; }
        public int SkillID { get; set; }

        public string NivelMinimoRequerido { get; set; } // Ej: 'Básico', 'Intermedio', etc.

        // Propiedades de Navegación (Foreign Keys)
        public PerfilesVacante Vacante { get; set; }
        public Skill Skill { get; set; }
    }
}
