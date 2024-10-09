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
    }
}
