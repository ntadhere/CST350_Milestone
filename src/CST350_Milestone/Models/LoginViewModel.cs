using System.ComponentModel.DataAnnotations;

namespace CST350_Milestone.Models
{
    public class LoginViewModel
    {
        // Class level properties
        [Required]
        public string Username { get; set; }
        [Required] //this is called an "attribute"
        public string Password { get; set; }
    }
}
