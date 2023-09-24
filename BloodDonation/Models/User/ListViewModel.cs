using BloodDonation.Types.Entity;

namespace BloodDonation.Web.Models.User
{
    public class ListViewModel
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Username { get; set; } = string.Empty;

        public UserType UserType { get; set; }

        public string? BloodGroupName { get; set; }

        public string? HospitalName { get; set; }
    }
}
