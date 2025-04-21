namespace CST350_Milestone.Models
{
    public class SavedGameModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime DateSaved { get; set; }
        public string GameData { get; set; }

        // Parameterless constructor
        public SavedGameModel()
        {
        }

        // Parameterized constructor
        public SavedGameModel(int id, string userId, DateTime dateSaved, string gameData)
        {
            Id = id;
            UserId = userId;
            DateSaved = dateSaved;
            GameData = gameData;
        }
    }
}
