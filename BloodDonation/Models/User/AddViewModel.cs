using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BloodDonation.Web.Models.User
{
    public class AddViewModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public byte BloodGroupId { get; set; }

        [Required]
        public byte UserTypeId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public int? HospitalId { get; set; }

        public List<SelectListItem>? BloodGroupSelectList { get; set; }
        public List<SelectListItem>? HospitalSelectList { get; set; }
    }
}
