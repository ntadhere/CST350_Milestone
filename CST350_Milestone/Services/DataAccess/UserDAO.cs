using CST350_Milestone.Interface;
using CST350_Milestone.Models;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace CST350_Milestone.Services.DataAccess
{
    public class UserDAO //: IUserManager
    {
        // Define the connection for MSQL
        private List<UserModel>? _users;
        // change userauth
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MilestoneUser;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        // Define the connection string for MySQL
        static string serverName = "localhost";
        static string username = "root";
        static string password = "root";
        static string dbName = "milestoneuser";
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
                query = "SELECT * FROM UserAccount WHERE Username = @Username AND MyPassword = @password";

                // Create a SQL command object using the query and open connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parmeters to the command to safely pass user input values, avoiding SQL injection
                    command.Parameters.AddWithValue("Id", user.Id);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Age", user.Age);  // age integer
                    command.Parameters.AddWithValue("@State", user.State);
                    command.Parameters.AddWithValue("@Groups", user.Email);
                    command.Parameters.AddWithValue("@Username",  user.UserName);
                    command.Parameters.AddWithValue("@Password", user.PasswordHash);

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

        /// <summary>
        /// This method will be used to verify a login attempt.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserModel CheckCredentials(string username, string password)
        {
            string query = "";

            using (SqlConnection  connection = new SqlConnection(conn))
            {
                connection.Open();

                // do user name & password exist
                query = "SELECT * FROM UserAccount WHERE Username = @Username AND MyPassword = @password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(@"Username", username);
                    command.Parameters.AddWithValue(@"Password", password);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if any records were returned (meaning a user with
                        // matching credentials exist)
                        if (reader.Read())
                        {
                            UserModel user = new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                                State = reader.GetString(reader.GetOrdinal("State")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                UserName = reader.GetString(reader.GetOrdinal("username")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("MyPassword")),
                                Sex = reader.GetString(reader.GetOrdinal("Sex"))
                            };
                            return user;
                        }
                        UserModel user2 = new UserModel
                        {
                            Id = 0
                        };
                        return user2;
                    }
                }
            }
        }
    }
}
