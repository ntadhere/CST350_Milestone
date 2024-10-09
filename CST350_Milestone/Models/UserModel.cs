

using Microsoft.AspNetCore.Identity;

namespace CST350_Milestone.Models
{
    public class UserModel
    {
        // Class level properties
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        // We may do not need salt
        //public byte[] Salt { get; set; }

        public void SetPassword(string password)
        {
            // Delare and Initialize
            IPasswordHasher<UserModel> hasher = new PasswordHasher<UserModel>();

            // Set password hash using hasher method
            PasswordHash = hasher.HashPassword(this, password);
        }
    }
}
