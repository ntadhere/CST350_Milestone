using CST350_Milestone.Interface;
using CST350_Milestone.Models;
using System.Data.SqlClient;

namespace CST350_Milestone.Services.DataAccess
{
    public class UserDAO : IUserManager
    {
        // Define the connection string for MSSQL
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MilestoneUser;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // Define the connection string for MySQL
        static string serverName = "localhost";
        static string username = "root";
        static string password = "root";
        static string dbName = "milestoneuser";
        static string port = "3306";
        // Declare and Initiallize
        string query = "";


        public int AddUser(UserModel user)
        {
            int result = -1;

            // Establish a new SQL connection using the provided connection string.
            // This connection uses SQL Server Authentication (username and password).
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database.
                connection.Open();

                // Define the SQL query to insert a new user into the UserAccount table.
                // The query inserts values for Username, MyPassword (hashed password), and GroupName.
                // SELECT SCOPE_IDENTITY() is used to retrieve the ID of the newly inserted record.
                string query = @"INSERT INTO Player (FirstName, LastName, Sex, Age, State, Email, Username, MyPassword)
                         VALUES (@FirstName, @LastName, @Sex, @Age, @State, @Email, @Username, @MyPassword);
                         SELECT SCOPE_IDENTITY();";

                // Create a SQL command object, passing the query and the open connection.
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the command to securely pass the user input (Username, PasswordHash, and Groups).
                    // This prevents SQL injection by ensuring the input values are safely escaped.
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Sex", user.Sex);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@State", user.State);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@MyPassword", user.PasswordHash);

                    // Execute the query and retrieve the identity value (ID) of the newly inserted user using ExecuteScalar.
                    object resultObj = command.ExecuteScalar();

                    // Check if the result is not null and can be successfully parsed to an integer.
                    // This indicates the ID of the newly inserted user.
                    if (resultObj != null && int.TryParse(resultObj.ToString(), out result))
                    {
                        // Return the newly inserted user's ID.
                        return result;
                    }
                    else
                    {
                        // If the ID retrieval fails, inform the user and suggest re-entering the details.
                        Console.WriteLine("Failed to retrieve the inserted ID. Please re-enter the details.");

                        // Return 0 to indicate failure to retrieve the inserted ID.
                        // You can handle this case differently depending on your application's error handling approach.
                        return 0;
                    }
                }
            }
        } //End AddUser method

        public int CheckCredentials(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        public List<UserModel> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public UserModel GetUserById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
