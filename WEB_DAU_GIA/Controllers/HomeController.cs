using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WEB_DAU_GIA.Models;

namespace WEB_DAU_GIA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

      
    }
}
