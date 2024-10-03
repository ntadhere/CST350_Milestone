using CST350_Milestone.Models;
using CST350_Milestone.Services.Business;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult ProcessLogin(LoginViewModel loginViewModel)
        {
            // Declare and Initialize 
            int result = -1;

            // Is there a match
            result = users.CheckCredentials(loginViewModel.Username, loginViewModel.Password);

            // We know the result will be 0 if the cred check failed
            if (result >= 0)
            {
                // Get the result and return it with the "UserModel"
                UserModel user = users.GetUserById(result);
                return View("LoginSuccess", user);
            }
            // There is no need for an else
            return View("LoginFailure");

        }
    }
}
