using BloodDonation.Types.Entity;

namespace BloodDonation.Business.Interfaces
{
    public interface INeedForBloodService
    {
        List<NeedForBlood> GetAll();
        List<NeedForBlood> GetAllByHospitalId(int hospitalId);
        NeedForBlood? GetById(int id);
        NeedForBlood? GetByHospitalIdAndBloodGroup(int hospitalId, BloodGroup bloodGroup);
        int Add(NeedForBlood needForBlood);
        int Update(NeedForBlood needForBlood);
        List<Hospital> GetHospitalListByBloodId(int bloodId);
    }
}
