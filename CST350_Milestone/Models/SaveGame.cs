namespace CST350_Milestone.Models
{
    public class SaveGame
    {

        // *fix properties*!!
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DataSaved { get; set; }
        public string GameData { get; set; }


        // *Fix Constructor!!*
        public SaveGame(int id, int userId, string dataSaved, string gameData)
        {
            Id = id;
            UserId = userId;
            DataSaved = dataSaved;
            GameData = gameData;
        }
    }
}
