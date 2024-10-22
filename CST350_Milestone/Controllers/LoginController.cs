using Microsoft.AspNetCore.Mvc;
using CST350_Milestone.Filter;
using CST350_Milestone.Models;
using CST350_Milestone.Services.Business;
using System.Text;
namespace CST350_Milestone.Controllers
{
    public class LoginController : Controller
    {
        // Instantiate the UserCollection class and create an object of named users
        static UserCollection users = new UserCollection();

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Controller that will be called to process the user login
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        public IActionResult ProcessLogin(string username, string password)
        {
            // Declare and Initialize 
            int result = -1;
            string userJson = "";

            // Create a new instance of 'UserModel' with propertires 'Id, Username, and PasswordHash'
            // This represents the user data provided during the login attempt
            UserModel userData = users.GetUserByUsername(username);

            // Is there a match
            result = users.CheckCredentials(username, password);

            // We know the result will be 0 if the cred check failed
            if (result >= 0)
            {
                // Serialize the 'userData' object to JSON string
                userJson = ServiceStack.Text.JsonSerializer.SerializeToString(userData);

                // Store the 'userData in the session with the key "User"
                HttpContext.Session.SetString("User", userJson);

                // Return the LoginSUccess view passing the userData as a model
                return View("LoginSuccess", userData);
            }
            // There is no need for an else
            return View("LoginFailure");

        }
        /// <summary>
        /// The annotation "SessionCheckFilter" triggers a request to run the filter
        /// </summary>
        /// <returns></returns>
        [SessionCheckFilter]
        public IActionResult StartGame()
        {
            return View("StartGame");
        }

        /// <summary>
        /// Log user out and remove session
        /// </summary>
        /// <returns></returns>
        [SessionCheckFilter]
        public IActionResult Logout()
        {
            //Remove the ssion
            HttpContext.Session.Remove("User");
            return View("Login");
        }

        /// <summary>
        /// Show the Registration Form with all the checkboxes
        /// </summary>
        /// <returns></returns>
        public IActionResult Register()
        {
            // Invokes the constructor we created
            // which has the list of all the checkboxes
            return View(new RegisterViewModel());
        }

        /// <summary>
        /// This action method takes the inputs from the registraction view and will process the results
        /// </summary>
        /// <param name="registerViewModel"></param>
        /// <returns></returns>
        public IActionResult ProcessRegister(RegisterViewModel registerViewModel)
        {
            //Create a new instance of UserModel to store the new user's information
            UserModel user = new UserModel();

            // Set the username, firstname, lastname, email, state, age of the new user from the RegisterViewModel passed from the form 
            user.UserName = registerViewModel.UserName;
            user.FirstName = registerViewModel.FirstName;
            user.LastName = registerViewModel.LastName;
            user.Email = registerViewModel.Email;
            user.State = registerViewModel.State;
            user.Age = registerViewModel.Age;

            // Set the password or the user by calling the SetPassword method
            user.SetPassword(registerViewModel.Password);

            // iNITIALIZE THE USER'S gROUPS AS AN EMPTY STRING
            user.Sex = "";

            // Initialize a sringBuilder to concatenate the group names efficiently
            StringBuilder sexBuilder = new StringBuilder();

            // Loop through each group in the RegisterViewModel.Groups list
            // (groups selected by the user)
            foreach (var sex in registerViewModel.Sex)
            {
                // Check if the group is selected by the user (IsSelected is true)
                if (sex.IsSelected)
                {
                    // Append the group name followed by a comma to the StringBuilder
                    sexBuilder.Append(sex.GenderOption).Append(",");
                }
            }
            // Remove the trailing comma from the Groups string (if any)
            // So the format is clean
            user.Sex = sexBuilder.ToString().TrimEnd(',');
            // Add the new user to the users collection, typically this is saving the user to a database of in-memory list
            users.AddUser(user);
            // After processing the registration, return the "Index" view to display the login page
            return View("Index");
        }
    }
}
