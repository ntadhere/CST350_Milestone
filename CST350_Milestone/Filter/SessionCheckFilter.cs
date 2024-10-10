using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CST350_Milestone.Filter
{
    // Define a class named "SessionCheckFIlter" that inherits from 
    // "ActionFilterAttribute"
    public class SessionCheckFilter: ActionFilterAttribute
    {
        // This code defines an action filter (SessionCheckFilter) that 
        // checks whether a user is logged in by verifying the 
        // "User" session variable.
        // Override the "OnActionExecutin" method, which executes
        // before and action method is called.
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Check if there is a session variable named "User" and if it's null (not set or expired)
            if(context.HttpContext.Session.GetString("User") == null)
            {
                // If the session variable "User" is null, redirect the user
                // to the "/User/Index" page
                context.Result = new RedirectResult("/Login/Index");
            }
        } //End override
    }
}
