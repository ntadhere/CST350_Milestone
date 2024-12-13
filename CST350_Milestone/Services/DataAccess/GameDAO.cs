using CST350_Milestone.Models;
using System.Data.SqlClient;

namespace CST350_Milestone.Services.DataAccess
{
    // Milestone 4
    public class GameDAO
    {
        // public or static?**
        public string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GameDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"; // DON'T FORGET STRING CONNECTION GOES HERE!!!

        public GameDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Save game
        public void saveGame(GameModel game)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                // RE-CHECK QUERY!!!***
                // Add or AddWithValue?
                string query = "INSERT INTO Games (UserId, DateSaved, GameData) VALUES (@UserId, @DateSaved, @GameData)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", game.UserId);
                command.Parameters.AddWithValue("@DateSaved", game.DateSaved);

                if (game.GameData == null )
                {
                    command.Parameters.AddWithValue("@GameData", DBNull.Value);    // GameData? DBNull?**
                }
                else
                {
                    command.Parameters.AddWithValue("@GameData", game.GameData);
                }


                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Load Game
        public GameModel LoadGame(int userId)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                // RE-CHECK QUERY!!!***
                string query = "SELECT TOP 1 * FROM Games WHERE UserId = @UserId ORDER BY DateSaved DESC";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();

                var reader = command.ExecuteReader();
                if(reader.Read())
                {
                    return new GameModel
                    {
                        Id = (int)reader["Id"],
                        UserId = (int)reader["UserId"],
                        DateSaved = (DateTime)reader["DateSaved"],
                        GameData = reader["GameData"] == DBNull.Value ? null : (string)reader["GameData"]   // string
                    };
                }
                return null;
            }
        }

    }
}
