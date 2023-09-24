using BloodDonation.Business.Interfaces;
using BloodDonation.Types.Entity;
using BloodDonation.Web.Models.Hospital;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers
{
    public class HospitalController : Controller
    {
        private readonly IHospitalService _hospitalService;

        public HospitalController(IHospitalService hospitalService)
        {
            _hospitalService = hospitalService;
        }

        [NonAction]
        private IActionResult? CheckPrivileges()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("BloodDonation_User_Username")))
            {
                return RedirectToAction("Login", "User");
            }
            if (HttpContext.Session.GetInt32("BloodDonation_User_UserTypeId") != (int)UserType.Admin)
            {
                return RedirectToAction("NotAuthorized", "User");
            }

            return null;
        }

        public IActionResult List()
        {
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }

            List<ListViewModel> model = new List<ListViewModel>();

            try
            {
                var hospitalList = _hospitalService.GetAll();
                if (hospitalList == null || hospitalList.Count == 1)
                {
                    throw new Exception("Liste boş geldi");
                }
                foreach (var item in hospitalList)
                {
                    if (item.Id == 0) continue;
                    ListViewModel hospital = new ListViewModel()
                    {
                        Address = item.Address,
                        Id = item.Id,
                        Name = item.Name,
                        Phone = item.Phone
                    };
                    model.Add(hospital);
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Error at HospitalController::List; " + e.Message;
            }
            return View(model);
        }

        public IActionResult Add()
        {
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }

            AddViewModel model = new AddViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(AddViewModel model)
        {
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Hospital hospital = new Hospital();
            hospital.Name = model.Name;
            hospital.Phone = model.Phone;
            hospital.Address = model.Address;

            try
            {
                _hospitalService.Add(hospital);
                return RedirectToAction("List");
            }
            catch
            {
                ViewBag.ErrorMessage = "Not Saved.";
                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }

            AddViewModel model = new AddViewModel();
            try
            {
                var result = _hospitalService.GetById(id);
                if (result == null)
                {
                    return View("Error");
                }
                model.Id = result.Id;
                model.Name = result.Name;
                model.Phone = result.Phone ?? string.Empty;
                model.Address = result.Address ?? string.Empty;
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at HospitalController::Edit " + ex.Message;
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var hospital = _hospitalService.GetById(model.Id);
                if (hospital == null)
                {
                    return View("Error");
                }

                hospital.Name = model.Name;
                hospital.Phone = model.Phone;
                hospital.Address = model.Address;

                _hospitalService.Update(hospital);
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
            if (CheckPrivileges() is var redirect && redirect != null)
            {
                return redirect;
            }

            try
            {
                var hospital = _hospitalService.GetById(id);
                if (hospital == null)
                {
                    return View("Error");
                }

                _hospitalService.Delete(hospital);
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
