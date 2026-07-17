using SistemaControlDeCalidad.Models;

namespace SistemaControlDeCalidad.ViewModels
{
    public class DashboardCalidadViewModel
    {
        public int TotalInspecciones { get; set; }
        public int TotalAprobadas { get; set; }
        public int TotalRechazadas { get; set; }
        public decimal PorcentajeDefectosGlobal { get; set; }
        public decimal PromedioDefectos { get; set; }
        public List<InspeccionCalidad> UltimasInspecciones { get; set; } = new();
        public string? MejorInspector { get; set; }
        public List<InspeccionCalidad> InspeccionesRecientes { get; set; } = new();

        public decimal TasaAprobacion => TotalInspecciones > 0
            ? (decimal)TotalAprobadas / TotalInspecciones * 100
            : 0;
    }
}
