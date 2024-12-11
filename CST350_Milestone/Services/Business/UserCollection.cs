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
            // GenerateUserData();
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
        /// Deletes a user from the database by their ID.
        /// </summary>
        /// <param name="id">The unique identifier (ID) of the user to be deleted.</param>
        /// <returns>Returns true if the user was successfully deleted, otherwise false.</returns>
        public bool DeleteUser(int id)
        {
            // Instantiate the DataAccess Layer to handle database operations.
            UserDAO userDAO = new UserDAO();

            // Pass the user ID to the DataAccess layer for deletion and return the result.
            return userDAO.DeleteUser(id);
        }

        /// <summary>
        /// Retrieves all users from the database as a list of UserModel objects.
        /// </summary>
        /// <returns>Returns a list of all UserModel objects representing the users in the database.</returns>
        public List<UserModel> GetAllUsers()
        {
            // Instantiate the DataAccess Layer to handle database operations.
            UserDAO userDAO = new UserDAO();

            // Retrieve the list of all users from the DataAccess layer and return it.
            return userDAO.GetAllUsers();
        }

        /// <summary>
        /// Retrieves a user by their ID from the database.
        /// </summary>
        /// <param name="id">The unique identifier (ID) of the user to retrieve.</param>
        /// <returns>Returns the UserModel object representing the user with the specified ID, or null if no user is found.</returns>
        public UserModel GetUserById(int id)
        {
            // Instantiate the DataAccess Layer to handle database operations.
            UserDAO userDAO = new UserDAO();

            // Retrieve the user by their ID from the DataAccess layer and return it.
            return userDAO.GetUserById(id);
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

        /// <summary>
        /// Save the game state to the database
        /// </summary>
        /// <param name="userId">The ID of the user saving the game</param>
        /// <param name="gameDataJson">The serialized game state</param>
        /// <returns></returns>
        public bool SaveGame(int userId, string gameDataJson)
        {
            try
            {
                // Call the DAO to save the game
                return _users.SaveGame(userId, gameDataJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game: {ex.Message}");
                return false;
            }
        }

    }
}
