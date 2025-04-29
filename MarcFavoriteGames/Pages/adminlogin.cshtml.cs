using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace MarcFavoriteGames.Pages
{
    public class adminloginModel(IConfiguration configuration) : PageModel
    {

        private readonly IConfiguration _configuration = configuration;

        public string Message { get; set; } = "";

        //Public Strings that Will store form input data 
        [BindProperty] public required string AdminID { get; set; }
        [BindProperty] public required string AdminPW { get; set; }


        //Admin Login
        public async Task<IActionResult> OnPostAsync()
        {
            string connectionString = _configuration.GetConnectionString("strcon");

            if (string.IsNullOrWhiteSpace(AdminID) || string.IsNullOrWhiteSpace(AdminPW)) // This is here so that it does not load the alert message without any login attempt.
            {
                TempData["ErrorMessage"] = "Please enter both username and password.";
                return Page();
            }
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = "SELECT username FROM admin_login_tbl WHERE username = @AdminID AND password = @AdminPW";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Sets user inputs as data rather than executable code. TO prevent SQL injection / hacking
                        //cmd.Parameters.AddWithValue("@AdminID", AdminID?.Trim()); // Old form 
                        //cmd.Parameters.AddWithValue("@AdminPW", AdminPW?.Trim()); //

                        cmd.Parameters.Add("@AdminID", System.Data.SqlDbType.NVarChar).Value = AdminID?.Trim(); //define parameter types more explicitly
                        cmd.Parameters.Add("@AdminPW", System.Data.SqlDbType.NVarChar).Value = AdminPW?.Trim(); //define parameter types more explicitly

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                //Old way of holding a session
                                // HttpContext.Session.SetString("username", dr["username"].ToString());
                                // HttpContext.Session.SetString("role", "admin");
                                // //HttpContext.Session.SetString("status", dr["account_status"].ToString()); - No need to check status admin should be always active

                                //HttpContext.Session.SetString("fullname", dr["full_name"].ToString()); //This is only here if necessary
                                // Create claims for authentication
                                var claims = new List<Claim>
                                {
                                    
                                    new Claim(ClaimTypes.Name, dr["username"].ToString()),
                                    new Claim(ClaimTypes.Role, "Admin") // For role-based security
                                };

                                var identity = new ClaimsIdentity(claims, "MyAuthScheme");
                                var principal = new ClaimsPrincipal(identity);

                                await HttpContext.SignInAsync("MyAuthScheme", principal, new AuthenticationProperties
                                { 
                                    IsPersistent = true,
                                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                                });

                                TempData["SuccessMessage"] = $"Hello, {AdminID}! Welcome back.";
                                return RedirectToPage("/Home");

                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again."); //Testing error handling
                                return Page();
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                // Log error for internal tracking
                Console.WriteLine($"Login Error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while logging in. Please try again later.";
                return Page();
            }

        }

        //Personal Functions

        

    }
  
}
