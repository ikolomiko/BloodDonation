using BloodDonation.Types.Entity;

namespace BloodDonation.Business.Interfaces
{
    public interface IHospitalService
    {
        List<Hospital> GetAll();
        Hospital GetById(int id);
        int Add(Hospital hospital);
        int Update(Hospital hospital);
    }
}
