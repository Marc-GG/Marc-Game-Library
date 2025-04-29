using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using static MarcFavoriteGames.Pages.mmanagementModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MarcFavoriteGames.Pages
{
    [Authorize(Roles = "Admin")]
    public class mmanagementModel : PageModel
    {
        private readonly string _connectionString;

        public mmanagementModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("strcon");

        }

        public class Member // Store Member List Data - also Old 
        {
            public string MemberID { get; set; }
            public string FullName { get; set; }
            public string DOB { get; set; }
            public string ContactNo { get; set; }
            public string Email { get; set; }
            public string State { get; set; }
            public string City { get; set; }
            public string Pincode { get; set; }
            public string FullAddress { get; set; }
            public string AccountStatus { get; set; }
        }

        [BindProperty] public string MemberID { get; set; }
        [BindProperty] public string FullName { get; set; }
        [BindProperty] public string DOB { get; set; }
        [BindProperty] public string ContactNo { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string State { get; set; }
        [BindProperty] public string City { get; set; }
        [BindProperty] public string Pincode { get; set; }
        [BindProperty] public string FullAddress { get; set; }
        [BindProperty] public string AccountStatus { get; set; }

        public List<Member> MemberList { get; set; } = new();
        [BindProperty] public string SearchTerm { get; set; }
        [BindProperty] public int PageSize { get; set; } = 10;
        [BindProperty] public int PageNumber { get; set; } = 1;
        public List<int> PageSizeOptions { get; } = new() { 10, 25, 50, 100 };

        public void OnGet()
        {
            LoadMembers();
        }

        public void OnPostSearchMember()
        {
            if (!string.IsNullOrEmpty(MemberID))
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                string query = "SELECT full_name, dob, contact_no, email, state, city, pincode, full_address, account_status " +
                               "FROM member_master_tbl WHERE member_id = @MemberID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MemberID", MemberID);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    FullName = reader["full_name"].ToString();
                    DOB = reader["dob"].ToString();
                    ContactNo = reader["contact_no"].ToString();
                    Email = reader["email"].ToString();
                    State = reader["state"].ToString();
                    City = reader["city"].ToString();
                    Pincode = reader["pincode"].ToString();
                    FullAddress = reader["full_address"].ToString();
                    AccountStatus = reader["account_status"].ToString();
                }
            }
            LoadMembers(); // ensures member list is loaded and not cleared by the GO button.
        }

        public void OnPostUpdateStatus(string status)
        {
            if (!string.IsNullOrEmpty(MemberID))
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();
                string query = "UPDATE member_master_tbl SET account_status = @Status WHERE member_id = @MemberID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@MemberID", MemberID);
                command.ExecuteNonQuery();

                OnPostSearchMember();
            }
        }
    
        // For searching but old function
        public void OnPostSearchList() // Old function not needed anymore
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                LoadMembers(); // Load full list if search is empty
                return;
            }

            // Search for either MemberID or FullName
            string query = @"
                SELECT member_id, full_name, dob, contact_no, email, state, city, pincode, full_address, account_status 
                FROM member_master_tbl 
                WHERE (LTRIM(RTRIM(member_id)) LIKE LTRIM(RTRIM(@SearchQuery))) 
                   OR (full_name LIKE @SearchTerm)
                ORDER BY full_name";
            using var command = new SqlCommand(query, connection);

            // Directly use the search term for both cases
            command.Parameters.AddWithValue("@SearchQuery", "%" + SearchTerm.Trim() + "%"); // Enables partial matching for ID
            command.Parameters.AddWithValue("@SearchTerm", "%" + SearchTerm.Trim() + "%");  // Enables partial matching for name

            using var reader = command.ExecuteReader();

            MemberList.Clear(); // Clear before adding new results

            while (reader.Read())
            {
                MemberList.Add(new Member
                {
                    MemberID = reader["member_id"].ToString(),
                    FullName = reader["full_name"].ToString(),
                    DOB = reader["dob"].ToString(),
                    ContactNo = reader["contact_no"].ToString(),
                    Email = reader["email"].ToString(),
                    State = reader["state"].ToString(),
                    City = reader["city"].ToString(),
                    Pincode = reader["pincode"].ToString(),
                    FullAddress = reader["full_address"].ToString(),
                    AccountStatus = reader["account_status"].ToString()
                });
            }
        }

        // above is an old function
        void clearForm() //To Clear the Member Details 
        
        {
            
            MemberID = "";
            FullName = "";
            DOB = "";
            ContactNo = "";
            State = "";
            City = "";
            Pincode = "";
            FullAddress = "";
            AccountStatus = "";
        }

        public void OnPostDeleteMember() 
        {
            if (string.IsNullOrWhiteSpace(MemberID))
            {
                TempData["ErrorMessage"] = "Member ID cannot be Blank!";
            }

            else
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        string query = "DELETE from member_master_tbl WHERE member_id = @MemberID";

                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@MemberID", MemberID?.Trim());

                            int affectedRows = cmd.ExecuteNonQuery(); // Check if the conection is successful variable.

                            connection.Close();

                            if (affectedRows > 0)
                            {
                                TempData["SuccessMessage"] = "Member deleted successfully.";
                                clearForm(); //Clears Member Details
                                MemberList.Clear(); // Refresh the list
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Member not found.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting the member.";
                    // Log the error for better debugging
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        //Not needed
        public void OnPostPreviousPage()
        {
            if (PageNumber > 1)
                PageNumber--;
            LoadMembers();
        }
        //Not Needed
        public void OnPostNextPage()
        {
            PageNumber++;
            LoadMembers();
        }

        private void LoadMembers()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string query = "SELECT member_id, full_name, dob, contact_no, email, state, city, pincode, full_address, account_status " +
                           "FROM member_master_tbl ORDER BY full_name ";
     
                //@OFFSET @Offset Rows fetch next @PageSize Rows only - this was for the old paginator

            using var command = new SqlCommand(query, connection);

            //not needed anymore datatables has a pagenator and a search engine

            //command.Parameters.AddWithValue("@SearchTerm", "%" + SearchTerm + "%");
            //command.Parameters.AddWithValue("@Offset", (PageNumber - 1) * PageSize);
            //command.Parameters.AddWithValue("@PageSize", PageSize);
            using var reader = command.ExecuteReader();

            //MemberList.Clear();
            while (reader.Read())
            {
                MemberList.Add(new Member
                {
                    MemberID = reader["member_id"].ToString(),
                    FullName = reader["full_name"].ToString(),
                    DOB = reader["dob"].ToString(),
                    ContactNo = reader["contact_no"].ToString(),
                    Email = reader["email"].ToString(),
                    State = reader["state"].ToString(),
                    City = reader["city"].ToString(),
                    Pincode = reader["pincode"].ToString(),
                    FullAddress = reader["full_address"].ToString(),
                    AccountStatus = reader["account_status"].ToString()
                });
            }
        }
    }
}
