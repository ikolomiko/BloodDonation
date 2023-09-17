using BloodDonation.Business.Interfaces;
using BloodDonation.Types.Data;
using BloodDonation.Types.Entity;

namespace BloodDonation.Business.Services
{
    public class HospitalService : IHospitalService
    {
        private readonly BloodDonationDbContext _dbContext;

        public HospitalService(BloodDonationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public List<Hospital> GetAll()
        {
            var result = _dbContext.Hospital.ToList();
            Console.WriteLine("count: "+result.Count);
            return result;
        }

        public Hospital GetById(int id)
        {
            return _dbContext.Hospital.First(x => x.Id == id);
        }

        public int Add(Hospital hospital)
        {
            _dbContext.Hospital.Add(hospital);
            return _dbContext.SaveChanges();
        }

        public int Update(Hospital hospital)
        {
            _dbContext.Hospital.Update(hospital);
            return _dbContext.SaveChanges();
        }
    }
}
