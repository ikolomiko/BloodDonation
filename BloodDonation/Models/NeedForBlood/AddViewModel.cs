using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BloodDonation.Web.Models.NeedForBlood
{
    public class AddViewModel
    {
        public int Id { get; set; }

        [Required]
        public byte BloodGroupId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [Required]
        public int Amount { get; set; }

        public List<SelectListItem>? BloodGroupSelectList { get; set; }

        public List<SelectListItem>? HospitalSelectList { get; set; }
    }
}
