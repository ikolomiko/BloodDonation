using BloodDonation.Types.Entity;
using BloodDonation.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BloodDonation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            int userTypeId = HttpContext.Session.GetInt32("BloodDonation_User_UserTypeId") ?? -1;
            if (userTypeId == -1)
            {
                return RedirectToAction("Login", "User");
            }

            var userType = (UserType)userTypeId;
            if (userType == UserType.Admin)
            {
                return RedirectToAction("List", "Hospital"); // Admin profili
            }
            else if (userType == UserType.Donor)
            {
                return RedirectToAction("ListAll", "NeedForBlood"); // Donör profili
            }
            else if (userType == UserType.Hospital)
            {
                return RedirectToAction("List", "NeedForBlood"); // Hastane kullanıcısı profili
            }

            return Error();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}