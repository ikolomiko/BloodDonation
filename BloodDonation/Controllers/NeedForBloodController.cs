using BloodDonation.Business.Interfaces;
using BloodDonation.Types.Entity;
using BloodDonation.Web.Models.NeedForBlood;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BloodDonation.Web.Controllers
{
    public class NeedForBloodController : Controller
    {
        private readonly INeedForBloodService _needForBloodService;
        private readonly IBloodGroupService _bloodGroupService;
        private readonly IHospitalService _hospitalService;

        public NeedForBloodController(INeedForBloodService needForBloodService, IBloodGroupService bloodGroupService, IHospitalService hospitalService)
        {
            _needForBloodService = needForBloodService;
            _bloodGroupService = bloodGroupService;
            _hospitalService = hospitalService;
        }

        public IActionResult ListAll()
        {
            // TODO: Check for user type and auth

            List<ListAllViewModel> model = new List<ListAllViewModel>();

            try
            {
                var needForBloodList = _needForBloodService.GetAll();
                if (needForBloodList == null || needForBloodList.IsNullOrEmpty())
                {
                    throw new Exception("Liste boş geldi");
                }
                foreach (var item in needForBloodList)
                {
                    ListAllViewModel needForBlood = new ListAllViewModel()
                    {
                        Id = item.Id,
                        BloodGroupName = item.BloodGroup.GetName(),
                        Amount = item.AmountNeeded,
                        HospitalName = _hospitalService.GetById(item.HospitalId).Name
                    };
                    model.Add(needForBlood);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error at NeedForBloodController::ListAll() " + ex.Message;
                Console.WriteLine(ex.StackTrace);
            }

            return View(model);
        }
    }
}
