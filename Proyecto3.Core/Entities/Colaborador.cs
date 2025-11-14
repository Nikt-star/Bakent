using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto3.Core.Entities
{
    public class Colaborador
    {
        public int ColaboradorID { get; set; }
        public string NombreCompleto { get; set; }
        public string RolActual { get; set; }
        public string CuentaProyecto { get; set; }
        public bool DisponibilidadMovilidad { get; set; }

        // Propiedades de Navegación (Relaciones Muchos-a-Muchos y Uno-a-Muchos)
        public ICollection<HabilidadColaborador> Habilidades { get; set; }
        public ICollection<Certificacion> Certificaciones { get; set; }

        public Colaborador()
        {
            Habilidades = new HashSet<HabilidadColaborador>();
            Certificaciones = new HashSet<Certificacion>();
        }
    }
}
