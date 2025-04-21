using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
