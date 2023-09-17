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
        public IActionResult List()
        {
            // TODO: Check login
            // TODO: Check user type id

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
            // TODO: Check login
            // TODO: Check user type id
            AddViewModel model = new AddViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(AddViewModel model)
        {
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
                return RedirectToAction(nameof(HospitalController.List));
            }
            catch
            {
                ViewBag.ErrorMessage = "Not Saved.";
                return View(model);
            }
        }
    }
}
