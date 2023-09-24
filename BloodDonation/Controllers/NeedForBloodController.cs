using BloodDonation.Business.Interfaces;
using BloodDonation.Types.Entity;
using BloodDonation.Web.Models.NeedForBlood;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace BloodDonation.Web.Controllers
{
    public class NeedForBloodController : Controller
    {
        private readonly INeedForBloodService _needForBloodService;
        private readonly IBloodGroupService _bloodGroupService;
        private readonly IHospitalService _hospitalService;
        private readonly IUserService _userService;

        public NeedForBloodController(INeedForBloodService needForBloodService, IBloodGroupService bloodGroupService, IHospitalService hospitalService, IUserService userService)
        {
            _needForBloodService = needForBloodService;
            _bloodGroupService = bloodGroupService;
            _hospitalService = hospitalService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult List()
        {
            string username = HttpContext.Session.GetString("BloodDonation_User_Username") ?? string.Empty;
            var user = _userService.GetByUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return user.UserType switch
            {
                UserType.Admin => ListAll(),
                UserType.Hospital => ListForHospital(),
                UserType.Donor => ListForDonor(),
                _ => RedirectToAction("NotAuthorized", "User"),
            };
        }

        public IActionResult ListAll()
        {
            var model = new SortedDictionary<string, List<ListViewModel>>();

            try
            {
                var hospitals = _hospitalService.GetAll();
                if (hospitals == null || hospitals.IsNullOrEmpty())
                {
                    throw new Exception("Hastane listesi boş geldi");
                }
                foreach (var hospital in hospitals)
                {
                    var needForBloodList = _needForBloodService.GetAllByHospitalId(hospital.Id);
                    if (needForBloodList == null || needForBloodList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    var needForBloodModel = new List<ListViewModel>();
                    foreach (var item in needForBloodList)
                    {
                        if (item.AmountNeeded <= 0)
                        {
                            continue;
                        }

                        needForBloodModel.Add(new ListViewModel()
                        {
                            Id = item.Id,
                            BloodGroupName = item.BloodGroup.GetName(),
                            Amount = item.AmountNeeded
                        });
                    }
                    if (!needForBloodModel.IsNullOrEmpty())
                    {
                        model.Add(hospital.Name, needForBloodModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at NeedForBloodController::ListAll() " + ex.Message;
                Console.WriteLine(ex.StackTrace);
            }

            return View("ListAll", model);
        }

        public IActionResult ListForHospital()
        {
            var model = new List<ListViewModel>();

            try
            {
                int hospitalId = HttpContext.Session.GetInt32("BloodDonation_User_HospitalId") ?? -1;
                if (hospitalId == -1 || _hospitalService.GetById(hospitalId) == null)
                {
                    throw new Exception("Hastane bulunamadı");
                }

                var needForBloodList = _needForBloodService.GetAllByHospitalId(hospitalId);
                if (needForBloodList == null)
                {
                    throw new Exception("Kan ihtiyacı listesi null geldi");
                }

                foreach (var item in needForBloodList)
                {
                    if (item.AmountNeeded <= 0)
                    {
                        continue;
                    }
                    model.Add(new ListViewModel()
                    {
                        Id = item.Id,
                        Amount = item.AmountNeeded,
                        BloodGroupName = item.BloodGroup.GetName()
                    });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at NeedForBloodController::ListForHospital() " + ex.Message;
                Console.WriteLine(ex.StackTrace);
            }
            return View("ListForHospital", model);
        }

        public IActionResult ListForDonor()
        {
            var model = new List<ListViewModel>();

            try
            {
                int bloodGroupId = HttpContext.Session.GetInt32("BloodDonation_User_BloodGroupId") ?? -1;
                if (bloodGroupId == -1)
                {
                    throw new Exception("Kan grubu bulunamadı");
                }

                var needForBloodList = _needForBloodService.GetAllByBloodGroup((BloodGroup)bloodGroupId);
                if (needForBloodList == null)
                {
                    throw new Exception("Kan ihtiyacı listesi null geldi");
                }

                foreach (var item in needForBloodList)
                {
                    if (item.AmountNeeded <= 0)
                    {
                        continue;
                    }

                    var hospital = _hospitalService.GetById(item.HospitalId);
                    model.Add(new ListViewModel()
                    {
                        Id = item.Id,
                        Amount = item.AmountNeeded,
                        Hospital = hospital
                    });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at NeedForBloodController::ListForDonor() " + ex.Message;
                Console.WriteLine(ex.StackTrace);
            }
            return View("ListForDonor", model);
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

        [HttpGet]
        public IActionResult Add()
        {
            string username = HttpContext.Session.GetString("BloodDonation_User_Username") ?? string.Empty;
            var user = _userService.GetByUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (user.UserType == UserType.Donor)
            {
                return RedirectToAction("NotAuthorized", "User");
            }

            var model = new AddViewModel()
            {
                BloodGroupSelectList = GetBloodGroupSelectList(),
                HospitalSelectList = GetHospitalSelectList()
            };
            if (user.UserType == UserType.Hospital)
            {
                model.HospitalId = user.HospitalId ?? -1;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(AddViewModel model)
        {
            string username = HttpContext.Session.GetString("BloodDonation_User_Username") ?? string.Empty;
            var user = _userService.GetByUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (user.UserType == UserType.Donor)
            {
                return RedirectToAction("NotAuthorized", "User");
            }

            model.BloodGroupSelectList ??= GetBloodGroupSelectList();
            model.HospitalSelectList ??= GetHospitalSelectList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (user.UserType == UserType.Hospital && model.HospitalId != user.HospitalId)
            {
                return RedirectToAction("NotAuthorized", "User");
            }

            try
            {
                var existingNeed = _needForBloodService.GetByHospitalIdAndBloodGroup(model.HospitalId, (BloodGroup)model.BloodGroupId);
                if (existingNeed != null)
                {
                    existingNeed.AmountNeeded = model.Amount;
                    _needForBloodService.Update(existingNeed);
                    return RedirectToAction("List");
                }

                var need = new NeedForBlood()
                {
                    HospitalId = model.HospitalId,
                    AmountNeeded = model.Amount,
                    BloodGroupId = model.BloodGroupId
                };
                _needForBloodService.Add(need);
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at NeedForBloodController::Add " + ex.Message;
                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            string username = HttpContext.Session.GetString("BloodDonation_User_Username") ?? string.Empty;
            var user = _userService.GetByUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (user.UserType == UserType.Donor)
            {
                return RedirectToAction("NotAuthorized", "User");
            }

            AddViewModel model = new AddViewModel();
            try
            {
                var result = _needForBloodService.GetById(id);
                if (result == null)
                {
                    return View("Error");
                }

                if (user.UserType == UserType.Hospital && result.HospitalId != user.HospitalId)
                {
                    return RedirectToAction("NotAuthorized", "User");
                }

                model.Id = result.Id;
                model.BloodGroupId = result.BloodGroupId;
                model.HospitalId = result.HospitalId;
                model.Amount = result.AmountNeeded;
                model.HospitalSelectList = GetHospitalSelectList();
                model.BloodGroupSelectList = GetBloodGroupSelectList();
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at NeedForBloodController::Edit " + ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult Edit(AddViewModel model)
        {
            string username = HttpContext.Session.GetString("BloodDonation_User_Username") ?? string.Empty;
            var user = _userService.GetByUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (user.UserType == UserType.Donor)
            {
                return RedirectToAction("NotAuthorized", "User");
            }

            model.HospitalSelectList ??= GetHospitalSelectList();
            model.BloodGroupSelectList ??= GetBloodGroupSelectList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (user.UserType == UserType.Hospital && model.HospitalId != user.HospitalId)
            {
                return RedirectToAction("NotAuthorized", "User");
            }

            try
            {
                var need = _needForBloodService.GetById(model.Id);
                if (need == null)
                {
                    return View("Error");
                }

                need.BloodGroupId = model.BloodGroupId;
                need.HospitalId = model.HospitalId;
                need.AmountNeeded = model.Amount;

                _needForBloodService.Update(need);
                return RedirectToAction("List");
            }
            catch
            {
                ViewBag.ErrorMessage = "Not Saved.";
                return View(model);
            }
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            string username = HttpContext.Session.GetString("BloodDonation_User_Username") ?? string.Empty;
            var user = _userService.GetByUsername(username);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (user.UserType == UserType.Donor)
            {
                return RedirectToAction("NotAuthorized", "User");
            }

            try
            {
                var need = _needForBloodService.GetById(id);
                if (need == null)
                {
                    return View("Error");
                }

                if (user.UserType == UserType.Hospital && need.HospitalId != user.HospitalId)
                {
                    return RedirectToAction("NotAuthorized", "User");
                }

                _needForBloodService.Delete(need);
                return RedirectToAction("List");
            }
            catch
            {
                ViewBag.ErrorMessage = "Not Saved.";
                return View("Error");
            }
        }
    }
}
