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

        /// <summary>
        /// add the new user to the db
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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

        public UserModel CheckCredentials(string username, string password)
        {
            // Open a new SQL connection using the connection string 
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the db
                connection.Open();
                query = "SELECT * FROM Player WHERE Username = @Username AND MyPassword = @MyPassword";

                // Create a SQL command object
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //Add the username parameter
                    command.Parameters.AddWithValue(@"Username", username);
                    command.Parameters.AddWithValue(@"MyPassword", password);

                    // Execute the command and object the SqlDataReader
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if any records are returned (meaning a user with matching credentials exists)
                        if (reader.Read())
                        {
                            // Create a UserModel object to store the user's details from the database
                            UserModel user = new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Sex = reader.GetString(reader.GetOrdinal("Sex")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                                State = reader.GetString(reader.GetOrdinal("State")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("MyPassword"))
                            };
                            // Return the user's ID if the credentials are valid
                            return user;
                        }
                        // Return 0 if no matching user is found in the database
                        // Create a UserModel object to store the user's details from the database
                        UserModel user2 = new UserModel
                        {
                            Id = 0
                        };
                        return user2;
                    }
                }
            }
        } //End Check Credentials

        /// <summary>
        /// Deletes a user from the UserAccount table in the database based on the provided user ID.
        /// </summary>
        /// <param name="id">The unique identifier (ID) of the user to be deleted from the database.</param>
        /// <returns>Returns true if the user was successfully deleted; otherwise, false.</returns>
        public bool DeleteUser(int id)
        {
            // Result flag to determine if the deletion was successful
            bool isDeleted = false;

            // Establish a new SQL connection using the provided connection string.
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database.
                connection.Open();

                // Define the SQL query to delete a user from the UserAccount table based on the user's ID.
                // Using string formatting here is risky (SQL Injection), so it's better to use a parameterized query.
                string query = "DELETE FROM Player WHERE Id = @Id";

                // Create a SQL command object using the query and open connection.
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add a parameter to safely pass the user ID, preventing SQL injection attacks.
                    command.Parameters.AddWithValue("@Id", id);

                    // Execute the DELETE query and return the number of rows affected (i.e., the number of users deleted).
                    int rowsAffected = command.ExecuteNonQuery();

                    // If one or more rows were affected, the deletion was successful.
                    if (rowsAffected > 0)
                    {
                        isDeleted = true;
                    }
                }
            }
            // Return whether the deletion was successful (true) or not (false).
            return isDeleted;

        } //End of DeleteUser

        /// <summary>
        /// Return a list of all the users in the db
        /// </summary>
        /// <returns></returns>
        public List<UserModel> GetAllUsers()
        {
            // List to store the retrieved users
            List<UserModel> users = new List<UserModel>();

            // SQL query to select all users
            string query = "SELECT Id, FirstName, LastName, Sex, Age, State, Email, Username, MyPassword FROM Player";

            // Using a SQL connection
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();

                // SQL command to execute the query
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Execute the command and obtain a reader
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Read all the rows from the result set
                        while (reader.Read())
                        {
                            // Create a new UserModel for each row
                            UserModel user = new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Sex = reader.GetString(reader.GetOrdinal("Sex")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                                State = reader.GetString(reader.GetOrdinal("State")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("MyPassword"))
                            };

                            // Add the user to the list
                            users.Add(user);
                        }
                    }
                }
            }

            // Return the list of users
            return users;

        } // End GetAllUser method

        /// <summary>
        /// Get user by username and return it
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public UserModel GetUserByUsername(string username)
        {
            // Find the matching id number
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();

                // Define the SQL query using a parameter placeholder (@Username)
                string query = "SELECT * FROM Player WHERE Username = @Username";

                // Create a SQL command object using the query and open connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add the username as a parameter to prevent SQL injection
                    command.Parameters.AddWithValue("@Username", username);

                    // Execute the command and obtain a SqlDataReader object to read the result set returned by the query
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if any records are returned (meaning a user with matching credentials exists)
                        if (reader.Read())
                        {
                            // Create a UserModel object to store the user's details from the database
                            UserModel user = new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Sex = reader.GetString(reader.GetOrdinal("Sex")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                                State = reader.GetString(reader.GetOrdinal("State")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("MyPassword"))
                            };
                            // Return the user's ID if the credential are valid
                            return user;
                        }
                        // Return 0 if no matching user is found in the database
                        // Create a UserModel object to store the user's details from the database
                        UserModel user2 = new UserModel
                        {
                            Id = 0
                        };
                        return user2;
                    }
                }
            }

        }// End of GetUserByUsername

        /// <summary>
        /// get user by user id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserModel GetUserById(int id)
        {
            // Find the matching id number
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();

                // Define the SQL query to select a single users based on id
                query = string.Format("SELECT * FROM Player WHERE Id = {0}", id);

                // Create a SQL command object using the query and open connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Execute the command and obtain a SqlDataReader object to read the result set returned by the query
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if any records are returned (meaning a user with matching credentials exists)
                        if (reader.Read())
                        {
                            // Create a UserModel object to store the user's details from the database
                            UserModel user = new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Sex = reader.GetString(reader.GetOrdinal("Sex")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                                State = reader.GetString(reader.GetOrdinal("State")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("MyPassword"))
                            };
                            // Return the user's ID if the credential are valid
                            return user;
                        }
                        // Return 0 if no matching user is found in the database
                        // Create a UserModel object to store the user's details from the database
                        UserModel user2 = new UserModel
                        {
                            Id = 0
                        };
                        return user2;
                    }
                }
            }
        } // End Get User by ID
    }
}
