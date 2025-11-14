using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Proyecto3.Core.Models
{
    public class MatchResult
    {
        public int ColaboradorID { get; set; }
        public string NombreCompleto { get; set; }
        public double PorcentajeMatch { get; set; } // % de cumplimiento de skills
        public List<string> SkillsFaltantes { get; set; } = new List<string>();
    }
}
