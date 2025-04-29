using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarcFavoriteGames.Pages
{
    public class AccessDeniedModel : PageModel
    {
        public void OnGet()
        {
            // Check the unauthorized access attempt (optional for security)
            Console.WriteLine($"Unauthorized access attempt at {DateTime.Now} by user: {User.Identity?.Name}");
        }
    }
}
