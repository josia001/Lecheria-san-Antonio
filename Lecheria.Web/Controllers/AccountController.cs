using Microsoft.AspNetCore.Mvc;

namespace Lecheria.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}