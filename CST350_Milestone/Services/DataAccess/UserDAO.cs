//using MySql.Data.MySqlClient;
using CST350_Milestone.Interface;
using CST350_Milestone.Models;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace CST350_Milestone.Services.DataAccess
{
    public class UserDAO : IUserManager
    {
        // Define the connection for MSQL
        // change userauth?
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UserAuth;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        // Define the connection string for MySQL
        static string serverName = "localhost";
        static string username = "root";
        static string password = "root";
        static string dbName = "userauth";
        static string port = "3306";    // get correct port

        // set the string connection
        // String ineroplation
        static string connStr = $"server={serverName};user={username};database={dbName};port={port};password={password}";

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddUser(UserModel user)
        {
            // Declare and Initialize
            string query = "";
            int result = -1;

            using(SqlConnection connection = new SqlConnection(conn))
            {
                // open the connection to the database
                connection.Open();
                // Define the SQL querry with parameter placeholders to prevent SQL injection attacks
                query = "SELECT * FROM UserAccount WHERE Username = @Username AND MyPassworf = @password";

                // Create a SQL command object using the query and open connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parmeters to the command to safely pass user input values, avoiding SQL injection
                    command.Parameters.AddWithValue("@Username",  user.UserName);
                    command.Parameters.AddWithValue("@Password", user.PasswordHash);
                    command.Parameters.AddWithValue("@Groups", user.Email);
                   // command.Parameters.AddWithValue("@")

                   // Execute the query and retrieve the new inserted ID using ExecuteScalar
                   // which returns the first column of the first row in the result set
                   if (int.TryParse(command.ExecuteScalar().ToString(), out result))
                    {
                        return result;
                    }
                   else
                    {
                        return 0;
                    }
                }
            }
        }

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
