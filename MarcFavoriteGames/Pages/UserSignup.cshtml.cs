using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
namespace MarcFavoriteGames.Pages
{
    public class UserSignupModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public string Message { get; set; } = "";
        public UserSignupModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Public Strings that Will store form input data 
        [BindProperty] public string FullID { get; set; }
        [BindProperty] public string Dob { get; set; }
        [BindProperty] public string Con { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string State { get; set; }
        [BindProperty] public List<SelectListItem> StateList { get; set; } = new()
{
    new SelectListItem { Value = "AL", Text = "Alabama" },
    new SelectListItem { Value = "AK", Text = "Alaska" },
    new SelectListItem { Value = "AZ", Text = "Arizona" },
    new SelectListItem { Value = "AR", Text = "Arkansas" },
    new SelectListItem { Value = "CA", Text = "California" },
    new SelectListItem { Value = "CO", Text = "Colorado" },
    new SelectListItem { Value = "CT", Text = "Connecticut" },
    new SelectListItem { Value = "DE", Text = "Delaware" },
    new SelectListItem { Value = "FL", Text = "Florida" },
    new SelectListItem { Value = "GA", Text = "Georgia" },
    new SelectListItem { Value = "HI", Text = "Hawaii" },
    new SelectListItem { Value = "ID", Text = "Idaho" },
    new SelectListItem { Value = "IL", Text = "Illinois" },
    new SelectListItem { Value = "IN", Text = "Indiana" },
    new SelectListItem { Value = "IA", Text = "Iowa" },
    new SelectListItem { Value = "KS", Text = "Kansas" },
    new SelectListItem { Value = "KY", Text = "Kentucky" },
    new SelectListItem { Value = "LA", Text = "Louisiana" },
    new SelectListItem { Value = "ME", Text = "Maine" },
    new SelectListItem { Value = "MD", Text = "Maryland" },
    new SelectListItem { Value = "MA", Text = "Massachusetts" },
    new SelectListItem { Value = "MI", Text = "Michigan" },
    new SelectListItem { Value = "MN", Text = "Minnesota" },
    new SelectListItem { Value = "MS", Text = "Mississippi" },
    new SelectListItem { Value = "MO", Text = "Missouri" },
    new SelectListItem { Value = "MT", Text = "Montana" },
    new SelectListItem { Value = "NE", Text = "Nebraska" },
    new SelectListItem { Value = "NV", Text = "Nevada" },
    new SelectListItem { Value = "NH", Text = "New Hampshire" },
    new SelectListItem { Value = "NJ", Text = "New Jersey" },
    new SelectListItem { Value = "NM", Text = "New Mexico" },
    new SelectListItem { Value = "NY", Text = "New York" },
    new SelectListItem { Value = "NC", Text = "North Carolina" },
    new SelectListItem { Value = "ND", Text = "North Dakota" },
    new SelectListItem { Value = "OH", Text = "Ohio" },
    new SelectListItem { Value = "OK", Text = "Oklahoma" },
    new SelectListItem { Value = "OR", Text = "Oregon" },
    new SelectListItem { Value = "PA", Text = "Pennsylvania" },
    new SelectListItem { Value = "RI", Text = "Rhode Island" },
    new SelectListItem { Value = "SC", Text = "South Carolina" },
    new SelectListItem { Value = "SD", Text = "South Dakota" },
    new SelectListItem { Value = "TN", Text = "Tennessee" },
    new SelectListItem { Value = "TX", Text = "Texas" },
    new SelectListItem { Value = "UT", Text = "Utah" },
    new SelectListItem { Value = "VT", Text = "Vermont" },
    new SelectListItem { Value = "VA", Text = "Virginia" },
    new SelectListItem { Value = "WA", Text = "Washington" },
    new SelectListItem { Value = "WV", Text = "West Virginia" },
    new SelectListItem { Value = "WI", Text = "Wisconsin" },
    new SelectListItem { Value = "WY", Text = "Wyoming" }
};

        [BindProperty] public string City { get; set; }
        [BindProperty] public string Pincode { get; set; }
        [BindProperty] public string FullAddress { get; set; }
        [BindProperty] public string memberID { get; set; }
        [BindProperty] public string PwID { get; set; }


        public IActionResult newUserSignUp(bool? clicked)
        {
            Console.WriteLine("OnPost() method was triggered! Clicked value: " + clicked);

            // Get the connection string
            string connectionString = _configuration.GetConnectionString("strcon");
            
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand(@"INSERT INTO member_master_tbl
                        (full_name, dob, contact_no, email, state, city, pincode, full_address, member_id, password, account_status) 
                        values (@full_name, @dob, @contact_no, @email, @state, @city, @pincode, @full_address, @member_id, @password, @account_status)", con);

                        cmd.Parameters.AddWithValue("@full_name", FullID?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@dob", Dob?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@contact_no", Con?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@email", Email?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@state", State ?? "");
                        cmd.Parameters.AddWithValue("@city", City?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@pincode", Pincode?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@full_address", FullAddress?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@member_id", memberID?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@password", PwID?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@account_status", "pending");

                        cmd.ExecuteNonQuery();
                    }

                    TempData["SuccessMessage"] = "Sign Up Successful. Go to User Login to Login";
                    return RedirectToPage("/userlogin");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database connection failed: " + ex.Message);
                    TempData["ErrorMessage"] = "Error: " + ex.Message;
                    return Page();
                }
            
        }

        //user defined method
        bool checkUserExists()
        {
            string connectionString = _configuration.GetConnectionString("strcon");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM member_master_tbl WHERE member_id = @memberID", con);
                    cmd.Parameters.AddWithValue("@memberID", memberID?.Trim());

                    int count = (int)cmd.ExecuteScalar(); // Get the count of matching records

                    return count > 0; // Return true if at least one user exists, false otherwise
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking user existence: " + ex.Message);
                return false; // Return false in case of an error
            }
        }

        public IActionResult OnPost(bool? clicked)
        {
            // Check if user exists first
            if (checkUserExists())
            {
                TempData["ErrorMessage"] = "User Already Exists with this User ID!";
                return Page(); // return to the same page with an error message
            }

            // If user doesn't exist, proceed with signup
            return newUserSignUp(clicked); // Ensure newUserSignUp properly handles redirect
        }



    }
}