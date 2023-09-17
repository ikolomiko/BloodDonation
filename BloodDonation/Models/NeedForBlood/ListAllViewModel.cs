namespace BloodDonation.Web.Models.NeedForBlood
{
    public class ListAllViewModel
    {
        public int Id { get; set; }

        public string BloodGroupName { get; set; } = string.Empty;

        public string HospitalName { get; set; } = string.Empty;

        public int Amount { get; set; }
    }
}
