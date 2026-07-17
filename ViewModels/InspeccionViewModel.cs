using SistemaControlDeCalidad.Models;

namespace SistemaControlDeCalidad.ViewModels
{
    public class InspeccionViewModel
    {
        public InspeccionCalidad? Inspeccion { get; set; }
        public string? NivelCalidadDescripcion { get; set; }
        public string? ClaseColor { get; set; }

        public void MapearDesdeInspeccion(InspeccionCalidad inspeccion)
        {
            Inspeccion = inspeccion;
            NivelCalidadDescripcion = inspeccion.NivelCalidad;

            ClaseColor = inspeccion.NivelCalidad switch
            {
                "Excelente" => "success",
                "Bueno" => "info",
                "Regular" => "warning",
                "Crítico" => "danger",
                _ => "secondary"
            };
        }
    }
}
