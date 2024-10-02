
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
        public int State { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        // We make do not need salt
        //public byte[] Salt { get; set; }

    }
}
