using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenterManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin görebilsin

    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
