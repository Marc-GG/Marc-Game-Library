using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace MarcFavoriteGames.Pages
{
    
    public class userloginModel(IConfiguration configuration) : ShowGlobalModel
    {

        private readonly IConfiguration _configuration = configuration;

        public string Message { get; set; } = "";

        //Public Strings that Will store form input data 
        [BindProperty] public required string UserID { get; set; }
        [BindProperty] public required string UserPW { get; set; }

        //User Login
        public async Task<IActionResult> OnPostAsync()
        {
            string connectionString = _configuration.GetConnectionString("strcon");

            if (string.IsNullOrWhiteSpace(UserID) || string.IsNullOrWhiteSpace(UserPW)) // This is here so that it does not load the alert message without any login attempt.
            {
                TempData["ErrorMessage"] = "Please enter both username and password.";
                return Page();
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = "SELECT member_id FROM member_master_tbl WHERE member_id = @UserID AND password = @UserPW";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Sets user inputs as data rather than executable code. TO prevent SQL injection / hacking
                        
                        cmd.Parameters.Add("@UserID", System.Data.SqlDbType.NVarChar).Value = UserID?.Trim(); //define parameter types more explicitly
                        cmd.Parameters.Add("@UserPW", System.Data.SqlDbType.NVarChar).Value = UserPW?.Trim(); //define parameter types more explicitly
                        
                        //Old Code
                        //cmd.Parameters.AddWithValue("@UserID", UserID?.Trim());
                        //cmd.Parameters.AddWithValue("@UserPW", UserPW?.Trim());

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                var claims = new List<Claim> //member id is the only one necessary to validate the user existence.
                                {
                                    new Claim(ClaimTypes.Name, dr["member_id"].ToString()),
                                    new Claim(ClaimTypes.Role, "User")
                                };

                                var identity = new ClaimsIdentity(claims, "MyAuthScheme");
                                var principal = new ClaimsPrincipal(identity); //incase more claims are added

                                await HttpContext.SignInAsync("MyAuthScheme", principal, new AuthenticationProperties
                                {
                                    IsPersistent = true, //Should try to keep authentication even if browser is closed
                                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                                });

                                TempData["SuccessMessage"] = $"Hello, {UserID}!";
                                return RedirectToPage("/Home");

                                //HttpContext.Session.SetString("username", dr["member_id"].ToString());  //Old Session Code
                                ////HttpContext.Session.SetString("fullname", dr["full_name"].ToString());
                                //HttpContext.Session.SetString("role", "user");
                                ////HttpContext.Session.SetString("status", dr["account_status"].ToString());


                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Invalid Credentials";
                                return Page();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while logging in. Details: " + ex.Message; // FIXED: Show error details for debugging
                return Page();
            }

        }

        //Personal Functions


    }
}
