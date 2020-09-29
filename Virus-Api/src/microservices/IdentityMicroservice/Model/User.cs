using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityMicroservice.Model
{
    public class User
    {
        public static readonly string DocumentName = "users";
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [SwaggerSchema("Email-ID of User")]
        [Required]
        public string Email { get; set; }
        [SwaggerSchema("Password of User")]
        [Required]
        public string Password { get; set; }
        public string Mobile { get; set; }
        public string RoleName { get; set; }
        [SwaggerSchema("Working company of user")]
        [Required]
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        [SwaggerSchema("Working status of user")]
        public int IsActive { get; set; }
        public string Token { get; set; }
        [SwaggerSchema("User when get registered.")]
        public DateTime WhenEntered { get; set; } = DateTime.UtcNow;
        [SwaggerSchema("User when get registered by whome ?")]
        public Guid? EnteredBy { get; set; }
        [SwaggerSchema("User details modified by whome ?")]
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
