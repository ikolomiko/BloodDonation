namespace BloodDonation.Web.Models.Hospital
{
    public class ListViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}
