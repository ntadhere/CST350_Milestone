using CST350_Milestone.Models;
using CST350_Milestone.Services.Business;
using Microsoft.AspNetCore.Mvc;

namespace CST350_Milestone.Controllers
{
    public class UserController : Controller
    {
        // Instantiate the UserCollection class and create an object of named users
        static UserCollection users = new UserCollection();

        public IActionResult Index()
        {
            return View();
        }

        
    }

}
