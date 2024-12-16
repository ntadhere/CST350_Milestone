using CST350_Milestone.Models;
using CST350_Milestone.Services.Business;
using Microsoft.AspNetCore.Mvc;

namespace CST350_Milestone.Controllers
{
    [ApiController]
    [Route("api/")]
    public class GameAPIController : ControllerBase
    {
        // No route specified since this is the default
        // /api/weatherapi
        [HttpGet("showSavedGames")]
        public ActionResult<IEnumerable<SavedGameModel>> Index()
        {
            // Intantiate the Business Layer
            GameCollection gameCollection = new GameCollection();

            // Get the List of saved games
            List<SavedGameModel> listSavedGame = gameCollection.GetAllSavedGame();

            return listSavedGame;
        }


        // HttpGet now defines the controller and Action Method Parameter
        [HttpGet("showSavedGames/{gameId}")]
        // Get /api/weatherapi/searchresults/xyz
        public ActionResult <SavedGameModel> ShowOneSavedGame(int gameId)
        {
            // Intantiate the Business Layer
            GameCollection gameCollection = new GameCollection();

            // Get the List of saved games
            SavedGameModel savedGame = gameCollection.GetSavedGameById(gameId);

            // return the list
            return savedGame;
        }
        
        // HttpGet now defines the controller and Action Method Parameter
        [HttpGet("deleteOneGame/{gameId}")]
        // Get /api/weatherapi/searchresults/xyz
        public ActionResult <bool> DeleteOneGame(int gameId)
        {
            // Intantiate the Business Layer
            GameCollection gameCollection = new GameCollection();

            // Get the List of saved games
            bool isDelete = gameCollection.DeleteGameById(gameId);

            // return the list
            return isDelete;
        }
    }
}
