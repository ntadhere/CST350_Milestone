using Microsoft.AspNetCore.Identity;
using CST350_Milestone.Interface;
using CST350_Milestone.Models;
using System.Security.Cryptography;
using System.Text;
using PasswordVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;
using CST350_Milestone.Services.DataAccess;

namespace CST350_Milestone.Services.Business
{
    public class UserCollection : IUserManager
    {
        // This is an in-memory list of users. Later this will be a db connection.
        // The _ prefix indicates a private field
        private List<UserModel>? _users;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public UserCollection() 
        {
            _users = new List<UserModel>();
            // Create some user account
            GenerateUserData();
        }

        /// <summary>
        /// Create two or more new ussers in the GenerateUserData method. Use the AddUser method to ensure new user accounts gets a valid Id number
        /// </summary>
        private void GenerateUserData()
        {
            // Create the first user
            UserModel user1 = new UserModel();
            user1.Username = "Dorothy";
            user1.SetPassword("Nguyen");
            user1.FirstName = "Dao";
            user1.LastName = "Nguyen";
            user1.Sex = "Female";
            user1.Age = 20;
            user1.State = "North Carolina";
            user1.Email = "dnguyen54@my.gcu.edu";

            AddUser(user1);
        }

        /// <summary>
        /// Generate an Id number automatically for the new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddUser(UserModel user)
        {
            // Instantiate the DataAccess Layer to pass the data up and down the N-Layer Architecture
            UserDAO userDAO = new UserDAO();

            // Then return it back up to the controller
            return userDAO.AddUser(user);
        }

        /// <summary>
        /// Verifies the login credentials by checking if the username and password match an existing user.
        /// The password is hashed and compared to the stored hash.
        /// </summary>
        /// <param name="username">The username of the user attempting to log in.</param>
        /// <param name="password">The plain-text password to be verified.</param>
        /// <returns>Returns the UserModel object if credentials are valid, otherwise returns null.</returns>
        public UserModel CheckCredentials(string username, string password)
        {
            // Instantiate the DataAccess Layer to handle database operations.
            UserDAO userDAO = new UserDAO();

            // Verify the username and password with the DataAccess layer and return the user details if valid.
            return userDAO.CheckCredentials(username, password);
        }

        /// <summary>
        /// Retrieves a user by their username from the database.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>Returns the UserModel object representing the user with the specified username, or null if no user is found.</returns>
        public UserModel GetUserByUsername(string username)
        {
            // Instantiate the DataAccess Layer to handle database operations.
            UserDAO userDAO = new UserDAO();

            // Retrieve the user by their username from the DataAccess layer and return it.
            return userDAO.GetUserByUsername(username);
        }

        /// <summary>
        /// Method to delete a user
        /// </summary>
        /// <param name="user"></param>
        public void DeleteUser(UserModel user)
        {
            _users.Remove(user);
        }

        /// <summary>
        /// Return all users in list
        /// </summary>
        /// <returns></returns>
        public List<UserModel> GetAllUsers()
        {
            return _users;
        }

        /// <summary>
        /// Given an id Number, Find the user with the matching id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserModel GetUserById(int id)
        {
            // Iterate through the model and find the user
            return _users.Find(x => x.Id == id);
        }


        /// <summary>
        /// Find an existing user and update that user
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(UserModel user)
        {
            // Declare and Initialize
            int userId = -1;
            UserModel findUser = new UserModel();


            // Find mathing id number
            // findUser object will equal null if not found
            findUser = GetUserById(user.Id);

            // Update the existing user
            if(findUser != null)
            {
                // Get the index of the user to update
                userId = _users.IndexOf(findUser);
                // Update the user at this Id
                _users[userId] = user;
            }
        }

        public bool VerifyPassword(string password, UserModel user)
        {
            // Declare and Initaialize
            IPasswordHasher<UserModel> hasher = new PasswordHasher<UserModel>();

            // Check if password matches the password hash
            PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            // Check and return result
            if (result == PasswordVerificationResult.Success)
            {
                return true;
            }
            return false;
        }

        public bool DeleteUser(int id)
        {
            throw new NotImplementedException();
        }
    }
}
