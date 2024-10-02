
namespace CST350_Milestone.Models
{
    public class UserModel
    {
        // Class level properties
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public byte[] Salt { get; set; }
        public string Groups { get; set; }

    }
}
