using BloodDonation.Business.Interfaces;
using BloodDonation.Types.Entity;

namespace BloodDonation.Business.Services
{
    public class BloodGroupService : IBloodGroupService
    {
        public List<BloodGroup> GetAll()
        {
            return Enum.GetValues(typeof(BloodGroup)).Cast<BloodGroup>().ToList();
        }

        public BloodGroup GetById(int id)
        {
            return (BloodGroup)id;
        }
    }
}
