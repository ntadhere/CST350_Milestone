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
        private List<UserModel>? _users;
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
        // added this?
        private int user;

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

        /// <summary>
        /// This method will be used to verify a login attempt.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int CheckCredentials(string username, string password)
        {
            string query = "";

            using (SqlConnection  connection = new SqlConnection(conn))
            {
                connection.Open();

                query = "SELECT * FROM UserAccount WHERE Username = @Username AND MyPassword = @password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(@"Username", username);
                    command.Parameters.AddWithValue(@"Password", password);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if any records were returned (meaning a user with
                        // matching credentials exist)
                        // ADD MORE!!!
                        if (reader.Read())
                        {
                            UserModel userModel = new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserName = reader.GetString(reader.GetOrdinal("username")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("MyPassword")),
                                // ADD THE REST!!!
                            };
                            return user;
                        }
                        UserModel users2 = new UserModel
                        {
                            Id = 0
                        };
                        return 0;    // recheck
                    }
                }
            }
        }

        public void DeleteUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a user by Id and return it
        /// </summary>
        /// <returns></returns>
        public List<UserModel> GetAllUsers()
        {
            // Declare and Initialize
            string query = "";

            // Create a new transport list
                List<UserModel > userList = new List<UserModel>();

            // Step 1: create a connection to the db
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // open the connection to database
                connection.Open();
                // Define the SQL query to select all users
                query = ("SELECT * FROM UserAccount");
                // Step 2: Create a SQL Command
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Step 3: Execute the command and obtain a SQLDataReader object
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if any records are returned 
                        while (reader.Read())
                        {
                            // Create a UserModel object to store the User's details
                            UserModel user = new UserModel();

                            // Populate the object
                            // ADD THE REST!!!
                            user.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            user.UserName = reader["Username"].ToString();
                            user.PasswordHash = reader["MyPassword"].ToString();
                            
                            // Add each user to the list
                            userList.Add(user);
                        }
                    }
                }
            }
                return userList;
        }

        /// <summary>
        /// Get a user by Id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserModel GetUserById(int id)
        {
            // Declare and Initialize
            string query = "";

            // Find the matching id number
            // 1. Instantiate the SqlConnection class and pass conn
            using(SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();
                // Define the SQL query to select a single users based on id
                query = string.Format("SELECT * FROM UserAccount WHERE Id = {0}", id);
                // Create a SQL command object using the query & open connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Execute the command and obtain a SqlDataReader object to
                    // read the results set returned by the query
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // check if any records were returned (meaning a user with 
                        // matching crednetials exists)
                        if (reader.Read())
                        {
                            // we know there is at least one row
                            // Create a UserModel object to store the user's
                            // details from the database
                            UserModel user = new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserName = reader.GetString(reader.GetOrdinal("username")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("MyPassword"))
                                // ADD THE REST!!!!
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
            // End get user by Id

        }
    }
}
