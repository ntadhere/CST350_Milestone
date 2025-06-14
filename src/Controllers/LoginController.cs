﻿using Microsoft.AspNetCore.Mvc;
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
            string userJson = "";

            // Create a new instance of 'UserModel' with propertires 'Id, Username, and PasswordHash'
            // This represents the user data provided during the login attempt
            UserModel userData = users.GetUserByUsername(username);

            // We know the result will be 0 if the cred check failed
            if (users.CheckCredentials(username, password).Id > 0)
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


        [SessionCheckFilter]
        public IActionResult StartGame()
        {
            // Display form to input Size and Difficulty
            return View("StartGame");
        }

        [HttpPost]
        [SessionCheckFilter]
        public IActionResult StartGame(int size, int difficulty)
        {
            if (ModelState.IsValid)
            {
                // Save Size and Difficulty to session
                HttpContext.Session.SetInt32("BoardSize", size);
                HttpContext.Session.SetInt32("Difficulty", difficulty);

                // Generate a unique game session ID and store it in session
                var gameSessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("GameSessionId", gameSessionId);

                // Redirect to the game page with the session-based ID
                return RedirectToAction("Index", "Game", new { gameSessionId });
            }

            return View();
        }


        [SessionCheckFilter]
        public IActionResult Logout()
        {
            // Clear the session for the current user
            HttpContext.Session.Remove("User");

            // Optionally clear other session data if needed
            HttpContext.Session.Clear();

            // Redirect to login page
            return RedirectToAction("Index");
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
            user.Username = registerViewModel.UserName;
            user.PasswordHash = registerViewModel.Password;
            user.FirstName = registerViewModel.FirstName;
            user.LastName = registerViewModel.LastName;
            user.Email = registerViewModel.Email;
            user.State = registerViewModel.State;
            user.Age = registerViewModel.Age;

            // Set the password or the user by calling the SetPassword method
            //user.SetPassword(registerViewModel.Password);

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
            if (users.AddUser(user) > 0)
            {
                // Automatically log in the new user by storing them in session
                HttpContext.Session.SetObjectAsJson("User", user);

                // Redirect to success page or main application
                return View("RegisterSuccess", user);
            }
            else
            {
                // Registration failure
                return View("RegisterFailure");
            }

        }
    }
}
