namespace BloodDonation.Web.Models.User
{
    public class ListViewModel
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Username { get; set; } = string.Empty;

        public string UserTypeName { get; set; } = string.Empty;

        public string? BloodGroupName { get; set; }

        public string? HospitalName { get; set; }
    }
}
