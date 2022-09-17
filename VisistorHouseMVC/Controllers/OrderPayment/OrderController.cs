using Microsoft.AspNetCore.Mvc;

namespace VisistorHouseMVC.Controllers.OrderPayment
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}