using BloodDonation.Types.Entity;

namespace BloodDonation.Business.Model
{
    public class UserWithDetail : User
    {
        public string HospitalName { get; set; } = string.Empty;
    }
}
