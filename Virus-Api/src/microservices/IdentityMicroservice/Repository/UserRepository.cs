using IdentityMicroservice.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public User GetUser(string email) => user.Find(u => u.Email == email).FirstOrDefault();

        public void InsertUser(User users) => user.InsertOne(users);

    }
}
