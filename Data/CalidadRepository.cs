using SistemaControlDeCalidad.Models;
using System.Collections.Generic;
using System.Linq;

namespace SistemaControlDeCalidad.Data
{
    public class CalidadRepository
    {
        private List<InspeccionCalidad> _inspecciones = new();
        private int _nextId = 1;

        public CalidadRepository()
        {
            InicializarDatos();
        }

        private void InicializarDatos()
        {
            var productos = new[] { "ProductoA", "ProductoB", "ProductoC", "ProductoD" };
            var inspectores = new[] { "Juan Pérez", "María Gómez", "Carlos Ruiz", "Ana Martínez" };
            var defectos = new[] { "Dimensional", "Superficial", "Funcional", "Estético" };
            var random = new Random();

            for (int i = 0; i < 35; i++)
            {
                var producto = productos[random.Next(productos.Length)];
                var cantidad = random.Next(50, 1000);
                var defectuosas = random.Next(0, (int)(cantidad * 0.15m)); // hasta 15% defectuosas
                var porcentaje = cantidad > 0 ? (decimal)defectuosas / cantidad * 100 : 0;
                var resultado = porcentaje <= 5 ? "Aprobado" : "Rechazado";
                var nivelCalidad = ObtenerNivelCalidad(porcentaje);

                _inspecciones.Add(new InspeccionCalidad
                {
                    Id = _nextId++,
                    NumeroInspeccion = $"INSP-{DateTime.Now.Year}-{i + 1:D4}",
                    Lote = $"LOTE-{DateTime.Now.Year}-{(char)('A' + random.Next(5))}-{random.Next(100, 999)}",
                    Producto = producto,
                    Inspector = inspectores[random.Next(inspectores.Length)],
                    CantidadInspeccionada = cantidad,
                    CantidadDefectuosa = defectuosas,
                    Resultado = resultado,
                    TipoDefecto = defectuosas > 0 ? defectos[random.Next(defectos.Length)] : null,
                    FechaInspeccion = DateTime.Now.AddDays(-random.Next(30)),
                    NivelCalidad = nivelCalidad,
                    PorcentajeDefectos = porcentaje
                });
            }
        }

        private string ObtenerNivelCalidad(decimal porcentaje)
        {
            if (porcentaje < 1) return "Excelente";
            if (porcentaje < 3) return "Bueno";
            if (porcentaje <= 5) return "Regular";
            return "Crítico";
        }

        // Operaciones CRUD
        public List<InspeccionCalidad> ObtenerTodas() => _inspecciones.ToList();

        public InspeccionCalidad? Buscar(int id) => _inspecciones.FirstOrDefault(i => i.Id == id);

        public List<InspeccionCalidad> BuscarPorTexto(string texto)
        {
            return _inspecciones.Where(i =>
                i.NumeroInspeccion.Contains(texto) ||
                i.Lote.Contains(texto) ||
                i.Producto.Contains(texto) ||
                i.Inspector.Contains(texto)).ToList();
        }

        public List<InspeccionCalidad> ObtenerAprobadas() =>
            _inspecciones.Where(i => i.Resultado == "Aprobado").ToList();

        public List<InspeccionCalidad> ObtenerRechazadas() =>
            _inspecciones.Where(i => i.Resultado == "Rechazado").ToList();

        public List<InspeccionCalidad> Filtrar(string? producto, string? resultado)
        {
            var query = _inspecciones.AsQueryable();

            if (!string.IsNullOrEmpty(producto))
                query = query.Where(i => i.Producto == producto);

            if (!string.IsNullOrEmpty(resultado))
                query = query.Where(i => i.Resultado == resultado);

            return query.ToList();
        }

        public void Agregar(InspeccionCalidad inspeccion)
        {
            // Validar regla de negocio: defectuosas <= inspeccionadas
            if (inspeccion.CantidadDefectuosa > inspeccion.CantidadInspeccionada)
                throw new InvalidOperationException("La cantidad defectuosa no puede ser mayor a la inspeccionada");

            // Calcular porcentaje y nivel de calidad
            inspeccion.PorcentajeDefectos = inspeccion.CantidadInspeccionada > 0
                ? (decimal)inspeccion.CantidadDefectuosa / inspeccion.CantidadInspeccionada * 100
                : 0;

            inspeccion.NivelCalidad = ObtenerNivelCalidad(inspeccion.PorcentajeDefectos);

            // Regla del 5% máximo
            if (inspeccion.PorcentajeDefectos > 5 && inspeccion.Resultado == "Aprobado")
                throw new InvalidOperationException("No se puede aprobar con más del 5% de defectos");

            // Validar código único
            if (_inspecciones.Any(i => i.NumeroInspeccion == inspeccion.NumeroInspeccion))
                throw new InvalidOperationException("El número de inspección ya existe");

            inspeccion.Id = _nextId++;
            _inspecciones.Add(inspeccion);
        }

        public void Actualizar(InspeccionCalidad inspeccion)
        {
            var existente = Buscar(inspeccion.Id);
            if (existente == null) return;

            // Validar reglas de negocio
            if (inspeccion.CantidadDefectuosa > inspeccion.CantidadInspeccionada)
                throw new InvalidOperationException("La cantidad defectuosa no puede ser mayor a la inspeccionada");

            inspeccion.PorcentajeDefectos = inspeccion.CantidadInspeccionada > 0
                ? (decimal)inspeccion.CantidadDefectuosa / inspeccion.CantidadInspeccionada * 100
                : 0;

            inspeccion.NivelCalidad = ObtenerNivelCalidad(inspeccion.PorcentajeDefectos);

            if (inspeccion.PorcentajeDefectos > 5 && inspeccion.Resultado == "Aprobado")
                throw new InvalidOperationException("No se puede aprobar con más del 5% de defectos");

            existente.NumeroInspeccion = inspeccion.NumeroInspeccion;
            existente.Lote = inspeccion.Lote;
            existente.Producto = inspeccion.Producto;
            existente.Inspector = inspeccion.Inspector;
            existente.CantidadInspeccionada = inspeccion.CantidadInspeccionada;
            existente.CantidadDefectuosa = inspeccion.CantidadDefectuosa;
            existente.Resultado = inspeccion.Resultado;
            existente.TipoDefecto = inspeccion.TipoDefecto;
            existente.FechaInspeccion = inspeccion.FechaInspeccion;
            existente.PorcentajeDefectos = inspeccion.PorcentajeDefectos;
            existente.NivelCalidad = inspeccion.NivelCalidad;
        }

        public void Eliminar(int id)
        {
            var inspeccion = Buscar(id);
            if (inspeccion == null) return;

            // No permitir eliminar inspecciones aprobadas
            if (inspeccion.Resultado == "Aprobado")
                throw new InvalidOperationException("No se pueden eliminar inspecciones aprobadas");

            _inspecciones.Remove(inspeccion);
        }

        public decimal CalcularPorcentajeDefectos()
        {
            var total = _inspecciones.Sum(i => i.CantidadInspeccionada);
            if (total == 0) return 0;
            var defectuosas = _inspecciones.Sum(i => i.CantidadDefectuosa);
            return (decimal)defectuosas / total * 100;
        }

        public List<string> ObtenerProductos() =>
            _inspecciones.Select(i => i.Producto).Distinct().OrderBy(p => p).ToList();

        public List<string> ObtenerInspectores() =>
            _inspecciones.Select(i => i.Inspector).Distinct().OrderBy(i => i).ToList();
    }
}
