using Microsoft.AspNetCore.Mvc;

namespace PTSLAttendanceManager.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
