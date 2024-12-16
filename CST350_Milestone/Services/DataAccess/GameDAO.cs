using System.Data.SqlClient;

namespace CST350_Milestone.Services.DataAccess
{
    public class GameDAO
    {
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MilestoneUser;Integrated Security=True;Connect Timeout=30;";

        /// <summary>
        /// Save game data to the database
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="gameDataJson">The serialized game data</param>
        /// <returns></returns>
        public bool SaveGame(int userId, string gameDataJson)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(conn))
                {
                    string query = "INSERT INTO Games (UserId, DateSaved, GameData) VALUES (@UserId, @DateSaved, @GameData)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@DateSaved", DateTime.Now);
                        command.Parameters.AddWithValue("@GameData", gameDataJson);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game: {ex.Message}");
                return false;
            }
        }
    }
}
