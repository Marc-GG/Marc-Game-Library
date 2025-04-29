using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Pages.Shared
{
    public class HeaderActionsModel : PageModel
    {

        public IActionResult OnGetViewGames()
        {
            HttpContext.Session.Remove("role"); // Clear role before navigation
            return RedirectToPage("/Inventory"); 
        }

        public IActionResult OnGetUserLogin()
        {
            return RedirectToPage("/userlogin"); 
        }

        public IActionResult OnGetSignUp()
        {
            return RedirectToPage("/UserSignup");
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.SignOutAsync("MyAuthScheme");  // Completely clear session data
            TempData.Clear();             // Clear any TempData
            return RedirectToPage("/Home"); // Redirect after logout
        }
        public IActionResult OnGetHelloUser()
        {
            return Page(); 
        }
    }
}