using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MunicipalServicesMVC.Models;

namespace MunicipalServicesMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Landing page (will show our 3-card menu in Views/Home/Index.cshtml)
        public IActionResult Index()
        {
            return View();
        }

        // Leave this as-is (template view)
        public IActionResult Privacy()
        {
            return View();
        }

        // Standard error page (leave as-is)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
