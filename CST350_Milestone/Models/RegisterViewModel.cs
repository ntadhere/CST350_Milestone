using System.ComponentModel.DataAnnotations; // Required for data annotations
using System.Collections.Generic;

namespace CST350_Milestone.Models
{
    public class SexViewModel
    {
        public bool IsSelected { get; set; }
        public string GenderOption { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces.")] public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain letters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Age must be greater than 0.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "State can only contain letters.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        // Password must be at least 8 characters long, contain at least one uppercase letter and one special character
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&?*])(?=.*\d).{8,}$",
ErrorMessage = "Password must have:<br/>- At least 8 characters long.<br/>- At least one uppercase letter.<br/>- At least one symbol (?!@#$%^&*).")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public List<SexViewModel> Sex { get; set; }

        // Constructor
        public RegisterViewModel()
        {
            // Initialize properties with default values
            FirstName = "";
            LastName = "";
            Age = 0;
            State = "";
            Email = "";
            UserName = "";
            Password = "";

            // Initialize gender selection checkboxes
            Sex = new List<SexViewModel>
            {
                new SexViewModel{GenderOption = "Male", IsSelected = false},
                new SexViewModel{GenderOption = "Female", IsSelected = false},
                new SexViewModel{GenderOption = "Prefer not to say", IsSelected = false}
            };
        }
    }
}
