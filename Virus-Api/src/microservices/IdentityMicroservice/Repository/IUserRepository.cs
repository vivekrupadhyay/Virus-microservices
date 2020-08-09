using IdentityMicroservice.Model;
using System;
using System.Collections.Generic;

namespace IdentityMicroservice.Repository
{
    public interface IUserRepository
    {
        List<User> GetAllUser();
        User GetUser(Guid userid);
        User GetUser(string email);
        User Login(string email, string password,string companyCode);
        void InsertUser(User user);
        void UpdateUser(User users);
        void DeleteUser(Guid userid);
        void ChangePassword(string oldPassword, string newPassword, Guid userId);
    }
}
