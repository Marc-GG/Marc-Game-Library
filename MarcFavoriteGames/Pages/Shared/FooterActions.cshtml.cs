using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Pages.Shared
{
    public class FooterActionsModel : PageModel
    {

        public IActionResult OnGetAdminLogin()
        {
            return RedirectToPage("/adminlogin");
        }

        public IActionResult OnGetGameInventory()
        {
            return RedirectToPage("/Inventory");
        }

        public IActionResult OnGetGameRental()
        {
            return RedirectToPage("/Rental");
        }

        public IActionResult OnGetMemberManagement()
        {
            return RedirectToPage("/mmanagement");
        }

        /*public IActionResult OnGetAuthorManagement() => RedirectToPage("/Author/Manage");
        public IActionResult OnGetPublisherManagement() => RedirectToPage("/Publisher/Manage");
        public IActionResult OnGetGameInventory() => RedirectToPage("/Inventory");
        public IActionResult OnGetGameRental() => RedirectToPage("/Rental");
        public IActionResult OnGetMemberManagement() => RedirectToPage("/mmanagement");
        */
    }
}