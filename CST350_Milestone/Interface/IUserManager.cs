using CST350_Milestone.Models;

namespace CST350_Milestone.Interface
{
    public interface IUserManager
    {
        //Return all users stored in the system
        public List<UserModel> GetAllUsers();
        // Given Id number, find the matching user
        public UserModel GetUserById(int id);
        // Add a new user to the lish / db. Using during registration
        public int AddUser(UserModel user);
        // Remove the user who matches
        public void DeleteUser(UserModel user);
        // Find the user with matching Id and replace it
        public int CheckCredentials(string username, string password);
    }
}
