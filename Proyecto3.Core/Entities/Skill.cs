using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto3.Core.Entities
{
    public class Skill
    {
        public int SkillID { get; set; }
        public string NombreSkill { get; set; }
        public string? TipoSkill { get; set; } // 'Técnico', 'Blando', 'Otro' - opcional para binding desde cliente

        // Propiedades de Navegación
        public ICollection<HabilidadColaborador> HabilidadesColaboradores { get; set; }
        public ICollection<RequisitoVacante> RequisitosVacantes { get; set; }

        public Skill()
        {
            HabilidadesColaboradores = new HashSet<HabilidadColaborador>();
            RequisitosVacantes = new HashSet<RequisitoVacante>();
        }
    }
}
