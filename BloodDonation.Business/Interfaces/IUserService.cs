﻿using BloodDonation.Business.Model;
using BloodDonation.Types.Entity;

namespace BloodDonation.Business.Interfaces
{
    public interface IUserService
    {
        List<UserWithDetail> GetAll();
        List<UserWithDetail> GetAllWithDetailForOtherUser();
        List<UserWithDetail> GetAllWithDetailForHospitalUser();
        User? GetByUsername(string username);
        User? GetByUserNameAndPassword(string username, string password);
        int Add(User user);
        int Update(User user);
        int Delete(User user);
    }
}
