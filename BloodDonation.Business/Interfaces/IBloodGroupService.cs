using BloodDonation.Types.Entity;

namespace BloodDonation.Business.Interfaces
{
    public interface IBloodGroupService
    {
        List<BloodGroup> GetAll();
        BloodGroup GetById(int id);
    }
}
