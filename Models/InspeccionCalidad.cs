using System.ComponentModel.DataAnnotations;

namespace SistemaControlDeCalidad.Models
{
    public class InspeccionCalidad
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de inspección es obligatorio")]
        [StringLength(20, MinimumLength = 3)]
        [Display(Name = "Número de Inspección")]
        public string NumeroInspeccion { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Lote")]
        public string Lote { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Producto")]
        public string Producto { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Inspector")]
        public string Inspector { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Cantidad Inspeccionada")]
        public int CantidadInspeccionada { get; set; }

        [Required]
        [Range(0, 10000)]
        [Display(Name = "Cantidad Defectuosa")]
        public int CantidadDefectuosa { get; set; }

        [Required]
        [Display(Name = "Resultado")]
        public string Resultado { get; set; } = string.Empty;

        [Display(Name = "Tipo de Defecto")]
        public string? TipoDefecto { get; set; }

        [Display(Name = "Fecha Inspección")]
        public DateTime FechaInspeccion { get; set; } = DateTime.Now;

        [Display(Name = "Nivel de Calidad")]
        public string? NivelCalidad { get; set; }

        public decimal PorcentajeDefectos { get; set; }
    }
}
