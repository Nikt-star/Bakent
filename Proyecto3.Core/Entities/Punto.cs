using System;

namespace Proyecto3.Core.Entities
{
    public class Punto
    {
        public int PuntoID { get; set; }
        public int ColaboradorID { get; set; }
        public int Cantidad { get; set; }
        public string Razon { get; set; } // "Perfil Completado", "Certificacion", "Curso Completado", "Aplicacion Vacante"
        public DateTime FechaOtorgamiento { get; set; }
        public int TotalAcumulado { get; set; }

        // Navegación
        public Colaborador? Colaborador { get; set; }
    }
}
