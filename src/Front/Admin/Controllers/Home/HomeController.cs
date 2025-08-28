using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers.Home
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
