using BloodDonation.Business.Interfaces;
using BloodDonation.Business.Model;
using BloodDonation.Types.Data;
using BloodDonation.Types.Entity;

namespace BloodDonation.Business.Services
{
    public class UserService : IUserService
    {
        private readonly BloodDonationDbContext _dbContext;

        public UserService(BloodDonationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public List<UserWithDetail> GetAll()
        {
            var query = from user in _dbContext.Set<User>()
                        join hospital in _dbContext.Set<Hospital>()
                        on user.HospitalId equals hospital.Id
                        orderby user.UserTypeId
                        select new UserWithDetail()
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            BloodGroupId = user.BloodGroupId,
                            Username = user.Username,
                            Password = user.Password,
                            UserTypeId = user.UserTypeId,
                            HospitalId = user.HospitalId,
                            HospitalName = hospital.Name
                        };
            return query.ToList();
        }

        public List<UserWithDetail> GetAllWithDetailForOtherUser()
        {
            var query = from user in _dbContext.Set<User>()
                        join hospital in _dbContext.Set<Hospital>()
                        on user.HospitalId equals hospital.Id
                        where user.UserTypeId == (byte) UserType.Donor
                        select new UserWithDetail()
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            BloodGroupId = user.BloodGroupId,
                            Username = user.Username,
                            Password = user.Password,
                            UserTypeId = user.UserTypeId,
                            HospitalId = user.HospitalId,
                            HospitalName = hospital.Name
                        };
            return query.ToList();

        }

        public List<UserWithDetail> GetAllWithDetailForHospitalUser()
        {
            var query = from user in _dbContext.Set<User>()
                        join hospital in _dbContext.Set<Hospital>()
                        on user.HospitalId equals hospital.Id
                        where user.UserTypeId == (byte) UserType.Hospital
                        select new UserWithDetail()
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            BloodGroupId = user.BloodGroupId,
                            Username = user.Username,
                            Password = user.Password,
                            UserTypeId = user.UserTypeId,
                            HospitalId = user.HospitalId,
                            HospitalName = hospital.Name
                        };
            return query.ToList();
        }

        public User? GetByUsername(string username)
        {
            return _dbContext.User.FirstOrDefault(x => x.Username == username);
        }

        public User? GetByUserNameAndPassword(string username, string password)
        {
            return _dbContext.User.FirstOrDefault(x => x.Username == username && x.Password == password);
        }

        public User? GetByUserName(string username)
        {
            return _dbContext.User.FirstOrDefault(x => x.Username == username);
        }

        public int Add(User user)
        {
            _dbContext.User.Add(user);
            return _dbContext.SaveChanges();
        }

        public int Update(User user)
        {
            _dbContext.User.Update(user);
            return _dbContext.SaveChanges();
        }
    }
}
