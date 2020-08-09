using IdentityMicroservice.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IdentityMicroservice.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> user;
        public UserRepository(IMongoDatabase database)
        {
            user = database.GetCollection<User>(User.DocumentName);
        }
        public List<User> GetAllUser() => user.Find(FilterDefinition<User>.Empty).ToList();// user.AsQueryable<User>().Select(c => c).ToList();
        public User GetUser(Guid userId) => user.Find(u => u.Id == userId).FirstOrDefault();

        public User GetUser(string email) => user.Find(u => u.Email == email).FirstOrDefault();
        public User Login(string email, string password, string companyCode)
        {
            var loginDetails = user.Find(u => u.Email == email && u.Password == password && u.CompanyCode == companyCode).FirstOrDefault();
            return loginDetails;
        }

        public void InsertUser(User users) => user.InsertOne(users);

        public void UpdateUser(User users) =>
            user.UpdateOne(c => c.Id == users.Id, Builders<User>.Update
                .Set(c => c.FirstName, users.FirstName)
                .Set(c => c.LastName, users.LastName)
                .Set(c => c.Email, users.Email)
                .Set(c => c.Password, users.Password)
                .Set(c => c.Mobile, users.Mobile)
                .Set(c => c.CompanyCode, users.CompanyCode)
                .Set(c => c.CompanyName, users.CompanyName)
                .Set(c => c.IsActive, users.IsActive)
                .Set(c => c.ModifiedBy, users.Id)
                .Set(c => c.LastModified, DateTime.UtcNow));

        public void DeleteUser(Guid userid) =>
            user.DeleteOne(c => c.Id == userid);

        public void ChangePassword(string oldPassword, string newPassword, Guid userId)
        {
            var chekPassword = user.Find(u => u.Password == oldPassword && u.Id == userId);
            if (chekPassword != null)
            {
                user.UpdateOne(c => c.Id == userId, Builders<User>.Update.Set(c => c.Password, newPassword));
            }
        }


    }
}
