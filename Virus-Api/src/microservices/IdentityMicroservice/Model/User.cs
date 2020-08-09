using Middleware;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityMicroservice.Model
{
    public class User
    {
        public static readonly string DocumentName = "users";
        
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Mobile { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int IsActive { get; set; }
        public DateTime WhenEntered { get; set; } = DateTime.UtcNow;
       
        public Guid? EnteredBy { get; set; }
        
        public Guid? ModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
       // public List<User> userlist { get; set; } = new List<User>();
        //public void SetPassword(string password, IEncryptor encryptor)
        //{
        //    Salt = encryptor.GetSalt(password);
        //    Password = encryptor.GetHash(password, Salt);
        //}

        //public bool ValidatePassword(string password, IEncryptor encryptor)
        //{
        //    var isValid = Password.Equals(encryptor.GetHash(password, Salt));
        //    return isValid;
        //}
    }
}
