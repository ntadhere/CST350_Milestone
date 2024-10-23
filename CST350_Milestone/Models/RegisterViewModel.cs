namespace CST350_Milestone.Models
{
    public class SexViewModel
    {
        public bool IsSelected { get; set; }
        public string GenderOption { get; set; }
    }
    public class RegisterViewModel
    {

        // Properties for Entry Screen
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<SexViewModel> Sex { get; set; }

        public RegisterViewModel()
        {
            // Declare and Init
            FirstName = "";
            LastName = "";
            Age = 0;
            State = "";
            Email = "";
            UserName = "";
            Password = "";
            // Create the selection we want for checkboxes
            Sex = new List<SexViewModel>
            {
                new SexViewModel{GenderOption = "Male", IsSelected = false},
                new SexViewModel{GenderOption = "Female", IsSelected = false},
                new SexViewModel{GenderOption = "Prefer not to say", IsSelected = false},
            };
        }
    }
}
