using BloodDonation.Types.Entity;
using System.ComponentModel.DataAnnotations;

namespace BloodDonation.Web.Models.User
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Password { get; set; } = string.Empty;
    }
}
