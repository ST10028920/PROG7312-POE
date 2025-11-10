using Microsoft.AspNetCore.Mvc;

namespace MunicipalServicesMVC.Controllers
{
    public class ChatController : Controller
    {
        // GET: /Chat
        public IActionResult Index()
        {
            return View();
        }
    }
}
