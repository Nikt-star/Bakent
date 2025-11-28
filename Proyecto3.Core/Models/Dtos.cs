using System;
using System.Collections.Generic;

namespace Proyecto3.Core.Models
{
    public class HabilidadCreateDto
    {
        public int? SkillID { get; set; }
        public string? SkillNombre { get; set; }
        public string NivelDominio { get; set; }
        public DateTime? FechaUltimaEvaluacion { get; set; }
        public string? TipoSkill { get; set; }
    }

    public class CertificacionCreateDto
    {
        public string NombreCertificacion { get; set; }
        public DateTime? FechaObtencion { get; set; }
        public string Institucion { get; set; }
    }

    public class ColaboradorCreateDto
    {
        public string NombreCompleto { get; set; }
        public string RolActual { get; set; }
        public string CuentaProyecto { get; set; }
        public bool DisponibilidadMovilidad { get; set; }

        public List<HabilidadCreateDto>? Habilidades { get; set; }
        public List<CertificacionCreateDto>? Certificaciones { get; set; }
    }

    public class ColaboradorUpdateDto
    {
        public List<HabilidadCreateDto>? Habilidades { get; set; }
        public List<CertificacionCreateDto>? Certificaciones { get; set; }
    }

    public class SkillCreateDto
    {
        public string NombreSkill { get; set; }
        public string? TipoSkill { get; set; }
    }

    // Perfil DTOs
    public class PerfilCreateDto
    {
        public int ColaboradorID { get; set; }
        public string Resumen { get; set; }
        public string? FotoUrl { get; set; }
        public string? LinkedIn { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
    }

    public class PerfilUpdateDto
    {
        public string? Resumen { get; set; }
        public string? FotoUrl { get; set; }
        public string? LinkedIn { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
    }

    // Vacante Aplicación DTOs
    public class Vacante_AplicacionCreateDto
    {
        public int VacanteID { get; set; }
        public int ColaboradorID { get; set; }
        public string? Interes { get; set; }
    }

    public class Vacante_AplicacionUpdateDto
    {
        public string Estado { get; set; }
        public string? Interes { get; set; }
        public string? Notas { get; set; }
    }

    // Notificación DTOs
    public class NotificacionCreateDto
    {
        public int ColaboradorID { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; }
    }

    // Punto DTOs
    public class PuntoDto
    {
        public int ColaboradorID { get; set; }
        public int Cantidad { get; set; }
        public string Razon { get; set; }
    }

    // Curso DTOs
    public class CursoCreateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int DuracionHoras { get; set; }
    }

    // Progreso Curso DTOs
    public class Progreso_CursoCreateDto
    {
        public int ColaboradorID { get; set; }
        public int CursoID { get; set; }
    }

    public class Progreso_CursoUpdateDto
    {
        public decimal PorcentajeCompletacion { get; set; }
        public bool Completado { get; set; }
    }

    // Log de Puntos DTOs
    public class LogPuntosDto
    {
        public int TotalPuntos { get; set; }
        public List<PuntoRegistroDto> Historial { get; set; } = new List<PuntoRegistroDto>();
    }

    public class PuntoRegistroDto
    {
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
        public string Razon { get; set; }
    }

    // Dashboard Skills DTOs
    public class DashboardSkillsDto
    {
        public string Area { get; set; }
        public string Rol { get; set; }
        public int Nivel { get; set; }
        public int Cantidad { get; set; }
        public List<SkillDetallDTO> Skills { get; set; }
    }

    public class SkillDetallDTO
    {
        public string Nombre { get; set; }
        public int CantidadColaboradores { get; set; }
        public string NivelPromedio { get; set; }
    }

    // Brecha de Skills DTOs
    public class BrechaSkillsDto
    {
        public string Rol { get; set; }
        public string Area { get; set; }
        public List<SkillBrechaDto> SkillsCriticas { get; set; }
    }

    public class SkillBrechaDto
    {
        public string Nombre { get; set; }
        public int Demanda { get; set; }
        public int Disponibilidad { get; set; }
        public int Brecha { get; set; }
        public string Severidad { get; set; } // "Crítica", "Alta", "Media", "Baja"
    }
}
