using System;
using System.Collections.Generic;

namespace Proyecto3.Core.Entities
{
    public class Perfil
    {
        public int PerfilID { get; set; }
        public int ColaboradorID { get; set; }
        public string Resumen { get; set; }
        public string? FotoUrl { get; set; }
        public string? LinkedIn { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool PerfilCompleto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        // Navegación
        public Colaborador? Colaborador { get; set; }
    }
}
