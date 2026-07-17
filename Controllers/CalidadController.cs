using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaControlDeCalidad.Data;
using SistemaControlDeCalidad.Models;
using SistemaControlDeCalidad.ViewModels;

namespace SistemaControlDeCalidad.Controllers
{
    public class CalidadController : Controller
    {
        private readonly CalidadRepository _repository;

        // Constructor con inyección de dependencias
        public CalidadController(CalidadRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var inspecciones = _repository.ObtenerTodas();
            return View(inspecciones);
        }

        public IActionResult Dashboard()
        {
            var todas = _repository.ObtenerTodas();
            var aprobadas = _repository.ObtenerAprobadas();
            var rechazadas = _repository.ObtenerRechazadas();

            // Calcular mejor inspector
            var mejorInspector = todas
                .GroupBy(i => i.Inspector)
                .Select(g => new { Inspector = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault()?.Inspector;

            var viewModel = new DashboardCalidadViewModel
            {
                TotalInspecciones = todas.Count,
                TotalAprobadas = aprobadas.Count,
                TotalRechazadas = rechazadas.Count,
                PorcentajeDefectosGlobal = _repository.CalcularPorcentajeDefectos(),
                PromedioDefectos = todas.Any() ? todas.Average(i => i.PorcentajeDefectos) : 0,
                UltimasInspecciones = todas.OrderByDescending(i => i.FechaInspeccion).Take(5).ToList(),
                MejorInspector = mejorInspector,
                InspeccionesRecientes = todas.OrderByDescending(i => i.Id).Take(5).ToList()
            };

            // Datos para ViewData
            ViewData["Aprobadas"] = aprobadas.Count;
            ViewData["Rechazadas"] = rechazadas.Count;
            ViewData["TotalInspecciones"] = todas.Count;
            ViewData["PromedioDefectos"] = viewModel.PromedioDefectos;
            ViewData["FechaServidor"] = DateTime.Now;

            // Convertir a SelectListItem
            ViewBag.Inspectores = _repository.ObtenerInspectores()
                .Select(i => new SelectListItem { Value = i, Text = i })
                .ToList();

            ViewBag.Productos = _repository.ObtenerProductos()
                .Select(p => new SelectListItem { Value = p, Text = p })
                .ToList();

            ViewBag.TiposDefecto = new[] { "Dimensional", "Superficial", "Funcional", "Estético" }
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToList();

            ViewBag.Resultados = new[] { "Aprobado", "Rechazado" }
                .Select(r => new SelectListItem { Value = r, Text = r })
                .ToList();

            return View(viewModel);
        }

        // GET: Calidad/Create
        public IActionResult Create()
        {
            ViewBag.Inspectores = _repository.ObtenerInspectores();
            ViewBag.Productos = _repository.ObtenerProductos();
            ViewBag.TiposDefecto = new[] { "Dimensional", "Superficial", "Funcional", "Estético" };
            ViewBag.Resultados = new[] { "Aprobado", "Rechazado" };

            return View(new InspeccionCalidad());
        }

        // POST: Calidad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InspeccionCalidad inspeccion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Agregar(inspeccion);
                    TempData["Success"] = "Inspección creada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.Inspectores = _repository.ObtenerInspectores();
            ViewBag.Productos = _repository.ObtenerProductos();
            ViewBag.TiposDefecto = new[] { "Dimensional", "Superficial", "Funcional", "Estético" };
            ViewBag.Resultados = new[] { "Aprobado", "Rechazado" };

            return View(inspeccion);
        }

        // GET: Calidad/Edit/{id}
        public IActionResult Edit(int id)
        {
            var inspeccion = _repository.Buscar(id);
            if (inspeccion == null)
                return NotFound();

            // Convertir a SelectListItem
            ViewBag.Inspectores = _repository.ObtenerInspectores()
                .Select(i => new SelectListItem { Value = i, Text = i })
                .ToList();

            ViewBag.Productos = _repository.ObtenerProductos()
                .Select(p => new SelectListItem { Value = p, Text = p })
                .ToList();

            ViewBag.TiposDefecto = new[] { "Dimensional", "Superficial", "Funcional", "Estético" }
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToList();

            ViewBag.Resultados = new[] { "Aprobado", "Rechazado" }
                .Select(r => new SelectListItem { Value = r, Text = r })
                .ToList();

            return View(inspeccion);
        }

        // POST: Calidad/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, InspeccionCalidad inspeccion)
        {
            if (id != inspeccion.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Actualizar(inspeccion);
                    TempData["Success"] = "Inspección actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Si hay error, recargar las listas como SelectListItem
            ViewBag.Inspectores = _repository.ObtenerInspectores()
                .Select(i => new SelectListItem { Value = i, Text = i })
                .ToList();

            ViewBag.Productos = _repository.ObtenerProductos()
                .Select(p => new SelectListItem { Value = p, Text = p })
                .ToList();

            ViewBag.TiposDefecto = new[] { "Dimensional", "Superficial", "Funcional", "Estético" }
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToList();

            ViewBag.Resultados = new[] { "Aprobado", "Rechazado" }
                .Select(r => new SelectListItem { Value = r, Text = r })
                .ToList();

            return View(inspeccion);
        }

        // GET: Calidad/Delete/{id}
        public IActionResult Delete(int id)
        {
            var inspeccion = _repository.Buscar(id);
            if (inspeccion == null)
                return NotFound();

            return View(inspeccion);
        }

        // POST: Calidad/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repository.Eliminar(id);
                TempData["Success"] = "Inspección eliminada exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Calidad/Details/{id}
        public IActionResult Details(int id)
        {
            var inspeccion = _repository.Buscar(id);
            if (inspeccion == null)
                return NotFound();

            var viewModel = new InspeccionViewModel();
            viewModel.MapearDesdeInspeccion(inspeccion);

            return View(viewModel);
        }

        // GET: Calidad/Search
        public IActionResult Search(string termino)
        {
            if (string.IsNullOrEmpty(termino))
                return RedirectToAction(nameof(Index));

            var resultados = _repository.BuscarPorTexto(termino);
            return View("Index", resultados);
        }

        // GET: Calidad/Filter
        public IActionResult Filter(string producto, string resultado)
        {
            var filtrados = _repository.Filtrar(producto, resultado);
            return View("Index", filtrados);
        }
    }
}