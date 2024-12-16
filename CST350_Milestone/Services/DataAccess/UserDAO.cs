using CST350_Milestone.Interface;
using CST350_Milestone.Models;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace CST350_Milestone.Services.DataAccess
{
    public class UserDAO //: IUserManager
    {
        // Define the connection string for MSSQL
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MilestoneUser;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // Define the connection string for MySQL
        static string serverName = "localhost";
        static string username = "root";
        static string password = "root";
        static string dbName = "MilestoneUser";

        static string port = "3306";
        // Declare and Initiallize
        string query = "";


        //==============================================================
        //==============================================================
        //======================= USER  SECTION ========================
        //==============================================================
        //==============================================================


        /// <summary>
        /// add the new user to the db
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddUser(UserModel user)
        {
            int result = -1;

            // Establish a new SQL connection using the provided connection string.
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();

                // Step 1: Check if the user already exists (based on Username or Email).
                // **Player replaced with MilestoneUser
                string checkQuery = @"SELECT COUNT(*) FROM MilestoneUser WHERE Username = @Username OR Email = @Email";

                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    // Add parameters for Username and Email to avoid SQL injection
                    checkCommand.Parameters.AddWithValue("@Username", user.Username);
                    checkCommand.Parameters.AddWithValue("@Email", user.Email);

                    // Execute the query and get the count of matching records
                    int userCount = (int)checkCommand.ExecuteScalar();

                    // If userCount > 0, the user already exists, so return 0 or an error code
                    if (userCount > 0)
                    {
                        Console.WriteLine("A user with this Username or Email already exists.");
                        return 0; // Return 0 to indicate that the user already exists
                    }
                }

                // Step 2: If the user does not exist, proceed with the insertion
                // Replaced Player with MilestoneUser
                string query = @"INSERT INTO MilestoneUser (FirstName, LastName, Sex, Age, State, Email, Username, MyPassword)
                  VALUES (@FirstName, @LastName, @Sex, @Age, @State, @Email, @Username, @MyPassword);
                  SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to securely pass user input values
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Sex", user.Sex);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@State", user.State);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@MyPassword", user.PasswordHash);

                    // Execute the query and retrieve the newly inserted user ID using ExecuteScalar
                    object resultObj = command.ExecuteScalar();

                    // Check if the result is not null and can be successfully parsed to an integer (the new user's ID)
                    if (resultObj != null && int.TryParse(resultObj.ToString(), out result))
                    {
                        return result; // Return the newly inserted user's ID
                    }
                    else
                    {
                        // If ID retrieval fails, inform the user
                        Console.WriteLine("Failed to retrieve the inserted ID. Please re-enter the details.");
                        return 0; // Return 0 to indicate failure
                    }
                }
            }
        } // End AddUser method




        public UserModel CheckCredentials(string username, string password)
        {
            // Open a new SQL connection using the connection string 
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the db
                connection.Open();
                // ** Relaced Player with MilestoneUser
                query = "SELECT * FROM MilestoneUser WHERE Username = @Username AND MyPassword = @MyPassword";

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
                // Replaced Player with MilestoneUser
                string query = "DELETE FROM MilestoneUser WHERE Id = @Id";

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
            // Replaced Player with MilestoneUser
            string query = "SELECT Id, FirstName, LastName, Sex, Age, State, Email, Username, MyPassword FROM MilestoneUser";

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
                // Replaced Player with MilestoneUser
                string query = "SELECT * FROM MilestoneUser WHERE Username = @Username";

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
                // Replaced Player with MilestoneUser
                query = string.Format("SELECT * FROM MilestoneUser WHERE Id = {0}", id);

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


        //==============================================================
        //==============================================================
        //=================== SAVE GAME SECTION ========================
        //==============================================================
        //==============================================================
        // Method to save the game state for a specific user in the database
        public bool SaveGameState(int userId, string gameDataJson)
        {
            try
            {
                // Establish a new SQL connection using the provided connection string.
                using (SqlConnection connection = new SqlConnection(conn))
                {
                    connection.Open(); // Open the database connection

                    // SQL query to insert a new game record into the Games table
                    string query = @"INSERT INTO Games (UserId, DateSaved, GameData)
                             VALUES (@UserId, GETDATE(), @GameData);
                             SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to securely pass user input values to the query
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@GameData", gameDataJson);

                        // Execute the query and check if any rows were affected
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Return true if at least one row was inserted
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the process
                Console.WriteLine($"Error saving game state: {ex.Message}");
                return false; // Return false if an error occurs
            }
        } // End SaveGameState method

        // Method to retrieve all saved games from the database
        public List<SavedGameModel> GetAllSavedGames()
        {
            // Create a new list to store all the saved games
            List<SavedGameModel> listSavedGames = new List<SavedGameModel>();

            // SQL query to select all records from the Games table
            string sqlStatement = "SELECT * FROM dbo.Games";

            using (SqlConnection connection = new SqlConnection(conn))
            {
                try
                {
                    connection.Open(); // Open the database connection

                    // Create a SQL command object with the query and connection
                    using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                    {
                        // Execute the query and retrieve the data using a SqlDataReader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Add each record to the list as a new SavedGameModel
                                listSavedGames.Add(new SavedGameModel(
                                    (int)reader[0],     // Game ID
                                    (string)reader[1], // User ID
                                    (DateTime)reader[2], // Date Saved
                                    (string)reader[3]  // Game Data
                                ));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during the process
                    Console.WriteLine($"Error fetching saved games: {ex.Message}");
                }
                // Return the list of saved games (empty if an error occurred)
                return listSavedGames;
            }
        }

        // Method to delete a specific game from the database using its ID
        public bool DeleteGameById(int id)
        {
            try
            {
                // Establish a new SQL connection using the provided connection string
                using (SqlConnection connection = new SqlConnection(conn))
                {
                    connection.Open(); // Open the database connection

                    // SQL query to delete a record from the Games table based on the ID
                    string query = "DELETE FROM Games WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameter to securely pass the game ID to the query
                        command.Parameters.AddWithValue("@Id", id);

                        // Execute the query and check if any rows were affected
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Return true if at least one row was deleted
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the process
                Console.WriteLine($"Error deleting game: {ex.Message}");
                return false; // Return false if an error occurs
            }
        }


        /// <summary>
        /// get user by user id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SavedGameModel GetSavedGameById(int id)
        {
            // Find the matching id number
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();

                // Define the SQL query to select a single users based on id
                // Replaced Player with MilestoneUser
                query = string.Format("SELECT * FROM Games WHERE Id = {0}", id);

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
                            SavedGameModel game = new SavedGameModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserId = reader.GetString(reader.GetOrdinal("UserId")),
                                DateSaved = reader.GetDateTime(reader.GetOrdinal("DateSaved")),
                                GameData = reader.GetString(reader.GetOrdinal("GameData"))
                            };
                            // Return the user's ID if the credential are valid
                            return game;
                        }
                        // Return 0 if no matching user is found in the database
                        // Create a UserModel object to store the user's details from the database
                        SavedGameModel game2 = new SavedGameModel
                        {
                            Id = 0
                        };
                        return game2;
                    }
                }
            }
        } // End Get User by ID

    }
}
