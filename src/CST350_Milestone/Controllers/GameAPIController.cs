using CST350_Milestone.Models;
using CST350_Milestone.Services.Business;
using Microsoft.AspNetCore.Mvc;

namespace CST350_Milestone.Controllers
{
    // Define the API controller
    [ApiController]
    // Set the base route for the API
    [Route("api/")]
    public class GameAPIController : ControllerBase
    {
        // HTTP GET endpoint to show all saved games
        // Route: /api/showSavedGames
        [HttpGet("showSavedGames")]
        public ActionResult<IEnumerable<SavedGameModel>> Index()
        {
            // Instantiate the Business Layer to handle game collection logic
            GameCollection gameCollection = new GameCollection();

            // Retrieve the list of all saved games from the business layer
            List<SavedGameModel> listSavedGame = gameCollection.GetAllSavedGame();

            // Return the list of saved games to the client
            return listSavedGame;
        }

        // HTTP GET endpoint to show a specific saved game by its ID
        // Route: /api/showSavedGames/{gameId}
        [HttpGet("showSavedGames/{gameId}")]
        public ActionResult<SavedGameModel> ShowOneSavedGame(int gameId)
        {
            // Instantiate the Business Layer to handle game collection logic
            GameCollection gameCollection = new GameCollection();

            // Retrieve the saved game with the specified ID from the business layer
            SavedGameModel savedGame = gameCollection.GetSavedGameById(gameId);

            // Return the details of the specific saved game to the client
            return savedGame;
        }

        // HTTP GET endpoint to delete a specific saved game by its ID
        // Route: /api/deleteOneGame/{gameId}
        [HttpGet("deleteOneGame/{gameId}")]
        public ActionResult<bool> DeleteOneGame(int gameId)
        {
            // Instantiate the Business Layer to handle game collection logic
            GameCollection gameCollection = new GameCollection();

            // Attempt to delete the saved game with the specified ID
            bool isDelete = gameCollection.DeleteGameById(gameId);

            // Return the result of the deletion operation (true if successful, false otherwise)
            return isDelete;
        }
    }
}
