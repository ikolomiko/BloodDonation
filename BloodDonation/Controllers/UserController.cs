using Microsoft.AspNetCore.Mvc;
using BloodDonation.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using BloodDonation.Web.Models.User;
using BloodDonation.Types.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [NonAction]
        private IActionResult? CheckPrivileges()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("BloodDonation_User_Username")))
            {
                return RedirectToAction("Login");
            }
            if (HttpContext.Session.GetInt32("BloodDonation_User_UserTypeId") != (int)UserType.Admin)
            {
                return RedirectToAction("NotAuthorized");
            }

            return null;
        }

        [AllowAnonymous]
        public IActionResult NotAuthorized()
        {
            return View();
        }

        [HttpGet]
        public IActionResult List()
        {
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }

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
                    string? hospitalName = item.HospitalId == 0 ? null : item.HospitalName;
                    model.Add(new ListViewModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Username = item.Username,
                        UserType = item.UserType,
                        BloodGroupName = item.UserType == UserType.Donor ? item.BloodGroup.GetName() : null,
                        HospitalName = item.UserType == UserType.Hospital ? item.HospitalName : null,
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
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }

            AddViewModel model = new AddViewModel();
            model.BloodGroupSelectList = GetBloodGroupSelectList();
            model.HospitalSelectList = GetHospitalSelectList();
            model.UserTypeSelectList = GetUserTypeSelectList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(AddViewModel model)
        {
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }

            model.BloodGroupSelectList ??= GetBloodGroupSelectList();
            model.HospitalSelectList ??= GetHospitalSelectList();
            model.UserTypeSelectList ??= GetUserTypeSelectList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _userService.GetByUsername(model.Username);
            if (user != null)
            {
                ViewBag.ErrorMessage = "Bu kullanıcı adına sahip başka bir kullanıcı mevcut. Lütfen farklı bir kullanıcı adı seçiniz.";
                return View(model);
            }

            user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                BloodGroupId = model.BloodGroupId ?? 0,
                Username = model.Username,
                Password = model.Password,
                UserType = (UserType)model.UserTypeId,
                HospitalId = model.HospitalId ?? 0
            };

            try
            {
                _userService.Add(user);
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at UserController::Add " + ex.Message;
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

        [NonAction]
        private List<SelectListItem> GetUserTypeSelectList()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();
            try
            {
                resultList.Add(new SelectListItem
                {
                    Value = ((int)UserType.Hospital).ToString(),
                    Text = UserType.Hospital.GetName()
                });
                resultList.Add(new SelectListItem
                {
                    Value = ((int)UserType.Donor).ToString(),
                    Text = UserType.Donor.GetName()
                });
            }
            catch
            {
                resultList = new List<SelectListItem>();
            }
            Console.WriteLine("UserTypeList size: " + resultList.Count);
            return resultList;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Signup()
        {
            SignupViewModel model = new SignupViewModel();
            model.BloodGroupSelectList = GetBloodGroupSelectList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Signup(SignupViewModel model)
        {
            model.BloodGroupSelectList ??= GetBloodGroupSelectList();

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Bilgileri kontrol ediniz.";
                return View(model);
            }

            var user = _userService.GetByUsername(model.Username);
            if (user != null)
            {
                ViewBag.Error = "Bu kullanıcı adına sahip başka bir kullanıcı var. Lütfen başka bir kullanıcı adı seçiniz.";
                return View(model);
            }

            user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                BloodGroupId = model.BloodGroupId,
                Username = model.Username,
                Password = model.Password,
                UserType = UserType.Donor,
                HospitalId = 0,
            };

            try
            {
                _userService.Add(user);
                return Redirect("/User/Login#signup-successful");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at UserController::Signup " + ex.Message;
                return View(model);
            }

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

            var user = _userService.GetByUsername(model.Username);
            if (user == null)
            {
                ViewBag.Error = "Kullanıcı bulunamadı.";
                return View(model);
            }

            user = _userService.GetByUserNameAndPassword(model.Username, model.Password);
            if (user == null)
            {
                ViewBag.Error = "Hatalı parola.";
                return View(model);
            }

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("BloodDonation_User_Username")))
            {
                HttpContext.Session.SetString("BloodDonation_User_FirstName", user.FirstName ?? string.Empty);
                HttpContext.Session.SetString("BloodDonation_User_LastName", user.LastName ?? string.Empty);
                HttpContext.Session.SetInt32("BloodDonation_User_BloodGroupId", user.BloodGroupId);
                HttpContext.Session.SetString("BloodDonation_User_BloodGroupName", user.BloodGroup.GetName());
                HttpContext.Session.SetString("BloodDonation_User_Username", user.Username);
                HttpContext.Session.SetInt32("BloodDonation_User_UserTypeId", user.UserTypeId);
                if (user.UserType == UserType.Hospital && user.HospitalId != null)
                {
                    HttpContext.Session.SetInt32("BloodDonation_User_HospitalId", user.HospitalId.Value);
                    HttpContext.Session.SetString("BloodDonation_User_HospitalName", _hospitalService.GetById(user.HospitalId.Value)?.Name ?? "Geçersiz Hastane");
                }
                else
                {
                    HttpContext.Session.SetInt32("BloodDonation_User_HospitalId", -1);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }

            try
            {
                var user = _userService.GetByUsername(id);
                if (user == null)
                {
                    return View("Error");
                }

                _userService.Delete(user);
                return RedirectToAction("List");
            }
            catch
            {
                ViewBag.ErrorMessage = "Not Saved.";
                return View("Error");
            }
        }

        public IActionResult Edit(string id)
        {
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }

            AddViewModel model = new AddViewModel();
            try
            {
                var result = _userService.GetByUsername(id);

                model.BloodGroupSelectList = GetBloodGroupSelectList();
                model.HospitalSelectList = GetHospitalSelectList();
                model.UserTypeSelectList = GetUserTypeSelectList();

                if (result == null)
                {
                    return View("Error");
                }

                model.FirstName = result.FirstName;
                model.LastName = result.LastName;
                model.BloodGroupId = result.BloodGroupId;
                model.Username = result.Username;
                model.Password = result.Password;
                model.UserTypeId = result.UserTypeId;
                model.HospitalId = result.HospitalId ?? 0;

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at UserController::Edit " + ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult Edit(AddViewModel model)
        {
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }
            model.BloodGroupSelectList ??= GetBloodGroupSelectList();
            model.HospitalSelectList ??= GetHospitalSelectList();
            model.UserTypeSelectList ??= GetUserTypeSelectList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = _userService.GetByUsername(model.Username);
                if (user == null)
                {
                    return View("Error");
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.BloodGroupId = model.BloodGroupId ?? 0;
                user.Password = string.IsNullOrEmpty(model.Password) ? user.Password : model.Password;
                user.UserType = (UserType)model.UserTypeId;
                user.HospitalId = model.HospitalId ?? 0;

                _userService.Update(user);
                return RedirectToAction("List");
            }
            catch
            {
                ViewBag.ErrorMessage = "Not Saved.";
                return View(model);
            }
        }
    }
}
