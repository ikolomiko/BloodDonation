using Microsoft.AspNetCore.Mvc;
using BloodDonation.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using BloodDonation.Web.Models.User;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using BloodDonation.Types.Entity;
using BloodDonation.Business.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace BloodDonation.Web.Controllers
{
    public class UserController : Controller
    {
        IUserService _userService;
        IBloodGroupService _bloodGroupService;
        IHospitalService _hospitalService;

        public UserController(IUserService userService, IBloodGroupService bloodGroupService, IHospitalService hospitalService)
        {
            _userService = userService;
            _bloodGroupService = bloodGroupService;
            _hospitalService = hospitalService;
        }

        [AllowAnonymous]
        public IActionResult NotAuthorized()
        {
            return View();
        }

        [HttpGet]
        public IActionResult List()
        {
            var model = new List<ListViewModel>();

            try
            {
                var userList = _userService.GetAll();
                if (userList == null || userList.Count == 0)
                {
                    throw new Exception("Liste boş geldi");
                }

                foreach (var item in userList)
                {
                    model.Add(new ListViewModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Username = item.Username,
                        UserTypeName = item.UserType.ToString(),
                        BloodGroupName = item.BloodGroup.GetName(),
                        HospitalName = item.HospitalId == 0 ? null : item.HospitalName
                    });
                }

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at UserController::List() " + ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            AddViewModel model = new AddViewModel();
            model.BloodGroupSelectList = GetBloodGroupSelectList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(AddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.BloodGroupSelectList = GetBloodGroupSelectList();
                return View(model);
            }
            var existUser = _userService.GetByUserName(model.Username);
            if (existUser != null)
            {
                ViewBag.ErrorMessage = "Mevcut UserName. Farklı bir username girebilir misiniz ?";
                model.BloodGroupSelectList = GetBloodGroupSelectList();
                return View(model);
            }
            model.BloodGroupSelectList = GetBloodGroupSelectList();

            User user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                BloodGroupId = model.BloodGroupId,
                Username = model.Username,
                Password = model.Password,
                UserType = (UserType)model.UserTypeId
            };

            try
            {
                _userService.Add(user);
                return RedirectToAction("Index");
                //return RedirectToAction(nameof(UserController.Login));
            }
            catch
            {
                ViewBag.ErrorMessage = "Not Saved.";
                return View(model);
            }
        }

        [NonAction]
        private List<SelectListItem> GetBloodGroupSelectList()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();
            try
            {
                resultList = _bloodGroupService.GetAll().Select(r => new SelectListItem() { Value = ((int)r).ToString(), Text = r.GetName() }).ToList();
            }
            catch
            {
                resultList = new List<SelectListItem>();
            }
            return resultList;
        }

        [NonAction]
        private List<SelectListItem> GetHospitalSelectList()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();
            try
            {
                resultList = _hospitalService.GetAll().OrderBy(r => r.Name).Select(r => new SelectListItem() { Value = r.Id.ToString(), Text = r.Name }).ToList();
            }
            catch
            {
                resultList = new List<SelectListItem>();
            }
            return resultList;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Bilgileri kontrol ediniz.";
                return View(model);
            }

            var user = _userService.GetByUserNameAndPassword(model.Username, model.Password);
            if (user == null)
            {
                ViewBag.Error = "Kullanıcı bulunamadı.";
                return View(model);
            }

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("BloodDonation_User_Username")))
            {
                HttpContext.Session.SetString("BloodDonation_User_FirstName", user.FirstName ?? string.Empty);
                HttpContext.Session.SetString("BloodDonation_User_LastName", user.LastName ?? string.Empty);
                HttpContext.Session.SetInt32("BloodDonation_User_BloodGroupId", user.BloodGroupId);
                HttpContext.Session.SetString("BloodDonation_User_Username", user.Username);
                HttpContext.Session.SetInt32("BloodDonation_User_UserTypeId", user.UserTypeId);
                if (user.UserType == UserType.Hospital && user.HospitalId != null)
                {
                    HttpContext.Session.SetInt32("BloodDonation_User_HospitalId", user.HospitalId.Value);
                    HttpContext.Session.SetString("BloodDonation_User_HospitalName", _hospitalService.GetById(user.HospitalId.Value).Name);
                }
                else
                {
                    HttpContext.Session.SetInt32("BloodDonation_User_HospitalId", -1);
                }
            }

            ViewData["Username"] = user.Username;
            
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
