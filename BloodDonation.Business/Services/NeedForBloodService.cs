using BloodDonation.Business.Interfaces;
using BloodDonation.Types.Data;
using BloodDonation.Types.Entity;

namespace BloodDonation.Business.Services
{
    public class NeedForBloodService : INeedForBloodService
    {
        private readonly BloodDonationDbContext _dbContext;

        public NeedForBloodService(BloodDonationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public List<NeedForBlood> GetAll()
        {
            return _dbContext.NeedForBlood.ToList();
        }
        public List<NeedForBlood> GetAllByHospitalId(int hospitalId)
        {
            return _dbContext.NeedForBlood.Where(x => x.HospitalId == hospitalId).ToList();
        }

        public NeedForBlood? GetById(int id)
        {
            return _dbContext.NeedForBlood.FirstOrDefault(x => x.Id == id);
        }

        public NeedForBlood? GetByHospitalIdAndBloodGroup(int hospitalId, BloodGroup bloodGroup)
        {
            return _dbContext.NeedForBlood.FirstOrDefault(x => x.HospitalId == hospitalId && x.BloodGroupId == (int)bloodGroup);
        }

        public int Add(NeedForBlood needForBlood)
        {
            _dbContext.NeedForBlood.Add(needForBlood);
            return _dbContext.SaveChanges();
        }

        public int Update(NeedForBlood needForBlood)
        {
            _dbContext.NeedForBlood.Update(needForBlood);
            return _dbContext.SaveChanges();
        }

        public List<Hospital> GetHospitalListByBloodId(int bloodId)
        {
            var query = from hospital in _dbContext.Set<Hospital>()
                        join needForBlood in _dbContext.Set<NeedForBlood>()
                        on hospital.Id equals needForBlood.HospitalId
                        where needForBlood.BloodGroupId == bloodId && needForBlood.AmountNeeded > 0
                        select hospital;
            return query.ToList();
        }
    }
}
