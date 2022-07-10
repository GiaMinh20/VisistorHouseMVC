using Microsoft.AspNetCore.Mvc;

namespace VisistorHouseMVC.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
