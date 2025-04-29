using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Azure.Identity;

namespace MarcFavoriteGames.Pages
{
    public class ShowGlobalModel : PageModel
    {
        public bool ShowUserLogin { get; set; } = true;

        
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {

            string userRole = context.HttpContext.Session.GetString("role");

            if (string.IsNullOrEmpty(userRole))
            {
                ShowUserLogin = true;
            }

            else if(userRole== "user")
            {
                ShowUserLogin = false;
                HttpContext.Session.SetString("HelloUser", "Hello " + userRole);

            }

            else if (userRole == "admin")
            {
                ShowUserLogin = true;  // Show admin panel
                HttpContext.Session.SetString("HelloUser", "Hello Admin");
            }
            base.OnPageHandlerExecuting(context);
        }
    }
}