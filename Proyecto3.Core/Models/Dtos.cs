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
}
