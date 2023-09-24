
namespace BloodDonation.Web.Models.NeedForBlood
{
    public class ListViewModel
    {
        public int Id { get; set; }

        public string BloodGroupName { get; set; } = string.Empty;

        public Types.Entity.Hospital? Hospital { get; set; }

        public int Amount { get; set; }
    }
}
