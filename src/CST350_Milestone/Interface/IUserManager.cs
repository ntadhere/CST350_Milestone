using CST350_Milestone.Models;

namespace CST350_Milestone.Interface
{
    public interface IUserManager
    {
        //Return all users stored in the system
        public List<UserModel> GetAllUsers();
        // Given Id number, find the matching user
        public UserModel GetUserById(int id);
        // Given username, find the matching user

        //public UserModel GetUserByUsername(string username);
        // Add a new user to the lish / db. Using during registration
        public int AddUser(UserModel user);
        // Remove the user who matches
        public bool DeleteUser(int id);
        // Find the user with matching Id and replace it
        public UserModel CheckCredentials(string username, string password);
    }
}
