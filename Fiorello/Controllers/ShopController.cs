using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
