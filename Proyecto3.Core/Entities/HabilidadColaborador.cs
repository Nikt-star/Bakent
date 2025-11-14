using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto3.Core.Entities
{
    public class HabilidadColaborador
    {
        // Clave Principal Compuesta (Se define en el DbContext)
        public int ColaboradorID { get; set; }
        public int SkillID { get; set; }

        public string NivelDominio { get; set; } // Ej: 'Básico', 'Intermedio', etc.
        public DateTime? FechaUltimaEvaluacion { get; set; } // Nullable

        // Propiedades de Navegación (Foreign Keys) - hacer opcionales para evitar validación obligatoria al bindear
        public Colaborador? Colaborador { get; set; }
        public Skill? Skill { get; set; }
    }
}
