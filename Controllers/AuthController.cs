using Microsoft.AspNetCore.Mvc;

namespace article.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
