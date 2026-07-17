using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SistemaControlDeCalidad.Models;

namespace SistemaControlDeCalidad.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Redirige al controlador de Calidad
            return RedirectToAction("Index", "Calidad");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}