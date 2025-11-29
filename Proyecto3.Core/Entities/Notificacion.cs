using System;

namespace Proyecto3.Core.Entities
{
    public class Notificacion
    {
        public int NotificacionID { get; set; }
        public int ColaboradorID { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; } // "Vacante", "Certificacion", "Puntos", "Desarrollo", "Otra"
        public bool Leida { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaLectura { get; set; }

        // Navegación
        public Colaborador? Colaborador { get; set; }
    }
}
