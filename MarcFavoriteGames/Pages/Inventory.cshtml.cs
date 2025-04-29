using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using static MarcFavoriteGames.Pages.mmanagementModel;

namespace MarcFavoriteGames.Pages
{
    [Authorize(Roles = "Admin")]
    public class InventoryModel : PageModel
    {
        
        private readonly string _connectionString;
                        
        public InventoryModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("strcon");

        }

        public class Game // Store Game List Data
        {
            public string GameID { get; set; }
            public string GameName { get; set; }
            public string Genre { get; set; }
            public string DirectorName { get; set; }
            public string CompanyName { get; set; }
            public string ReleaseDate { get; set; }
            public string Language { get; set; }
            public string Releases { get; set; }
            public string Price { get; set; }
            public string Ratings { get; set; }
            public string GameDescription { get; set; }
            public string RentalStock { get; set; }
            public string CurrentStock { get; set; }
            public string GameImage { get; set; }
        }

        //Storage For inputs
        [BindProperty] public string GameID { get; set; }
        [BindProperty] public string GameName { get; set; }
        [BindProperty] public IFormFile GameImage { get; set; }
        [BindProperty] public List<string> Genre { get; set; }
        [BindProperty] public string HiddenImagePath { get; set; }
        [BindProperty] public string GameImagePath { get; set; }
        [BindProperty] public string DirectorName { get; set; }
        [BindProperty] public string CompanyName { get; set; }
        [BindProperty] public string ReleaseDate { get; set; }
        [BindProperty] public string Language { get; set; }
        [BindProperty] public string Releases { get; set; }
        [BindProperty] public string Price { get; set; }
        [BindProperty] public string Ratings { get; set; }
        [BindProperty] public string RentalStock { get; set; }
        [BindProperty] public string CurrentStock { get; set; }
        [BindProperty] public string GameDescription { get; set; }
        public List<Game> InventoryList { get; set; } = new();

        //Load the Game List
        public void OnGet()
        {
            LoadGames();
        }

        //Check button
        public async Task OnPostCheckAsync()
        {
            if (!string.IsNullOrEmpty(GameID))
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"
                    SELECT 
                        game_id, game_name, genre, director_name, company_name, 
                        release_date, language, releases, 
                        ISNULL(price, '0') AS price,
                        ISNULL(ratings, '0') AS ratings,
                        ISNULL(rental_stock, '0') AS rental_stock,
                        ISNULL(current_stock, '0') AS current_stock,
                        ISNULL(game_img_link, 'images/game.png') AS game_img_link,
                        game_description
                    FROM game_master_tbl
                    WHERE game_id = @GameID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GameID", GameID);

                using var reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    GameID = reader["game_id"]?.ToString();
                    GameName = reader["game_name"]?.ToString();
                    Genre = reader["genre"]?.ToString()?.Split(',').ToList();
                    DirectorName = reader["director_name"]?.ToString();
                    CompanyName = reader["company_name"]?.ToString();
                    ReleaseDate = reader["release_date"]?.ToString();
                    Language = reader["language"]?.ToString();
                    Releases = reader["releases"]?.ToString();
                    Price = reader["price"]?.ToString() ?? "0";
                    Ratings = reader["ratings"]?.ToString() ?? "0";
                    GameDescription = reader["game_description"]?.ToString();
                    RentalStock = reader["rental_stock"]?.ToString() ?? "0";
                    CurrentStock = RentalStock;

                    ViewData["GameImageLink"] = reader["game_img_link"]?.ToString() ?? "/images/game.png";
                }
            }

            LoadGames();
            ModelState.Clear();
        }




        //Add button
        public async Task<IActionResult> OnPostAddGameAsync()
        {
            try
            {
                //Clear empty values
                GameID = string.IsNullOrWhiteSpace(GameID) ? null : GameID;
                GameName = string.IsNullOrWhiteSpace(GameName) ? null : GameName;
                DirectorName = string.IsNullOrWhiteSpace(DirectorName) ? null : DirectorName;
                CompanyName = string.IsNullOrWhiteSpace(CompanyName) ? null : CompanyName;
                ReleaseDate = string.IsNullOrWhiteSpace(ReleaseDate) ? null : ReleaseDate;
                GameDescription = string.IsNullOrWhiteSpace(GameDescription) ? null : GameDescription;
                // File Upload Handling - Saves the image in the correct place.
                if (GameImage != null && GameImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    string uniqueFileName = $"{Guid.NewGuid()}_{GameImage.FileName}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await GameImage.CopyToAsync(fileStream);
                    }

                    HiddenImagePath = $"images/{uniqueFileName}";
                }
                else
                {
                    HiddenImagePath = string.IsNullOrWhiteSpace(HiddenImagePath) ? "images/game.png" : HiddenImagePath;
                }


                //Validation
                if (string.IsNullOrWhiteSpace(GameID) ||
                    string.IsNullOrWhiteSpace(GameName) ||
                    string.IsNullOrWhiteSpace(DirectorName) ||
                    string.IsNullOrWhiteSpace(CompanyName) ||
                    string.IsNullOrWhiteSpace(ReleaseDate))
                {
                    TempData["ErrorMessage"] = "All required fields must be filled!";
                    LoadGames(); // ensures Games list is loaded and not cleared by the Check button.
                    return Page();
                }


                if (Genre == null || !Genre.Any())
                {
                    TempData["ErrorMessage"] = "Please select at least one genre.";
                    LoadGames(); // ensures Games list is loaded and not cleared by the Check button.
                    return Page();
                }

                string genres = string.Join(",", Genre ?? new List<string>());

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string checkQuery = "SELECT COUNT(1) FROM game_master_tbl WHERE game_id = @GameId";
                    using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@GameId", GameID);

                        con.Open();
                        int existingCount = (int)await cmd.ExecuteScalarAsync();

                        if (existingCount > 0)
                        {
                            TempData["ErrorMessage"] = "A game with this ID already exists!";
                            LoadGames(); // ensures Games list is loaded and not cleared by the Check button.
                            return Page();
                        }
                    }

                    string insertQuery = @"
                        INSERT INTO game_master_tbl 
                        (game_id, game_name, genre, director_name, company_name, release_date, language, releases, price, ratings, game_description, rental_stock, current_stock, game_img_link)
                        VALUES 
                        (@GameId, @GameName, @Genre, @DirectorName, @CompanyName, @ReleaseDate, @Language, @Releases, @Price, @Ratings, @GameDescription, @RentalStock, @CurrentStock, @GameImage)";


                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@GameId", GameID);
                        cmd.Parameters.AddWithValue("@GameName", GameName);
                        cmd.Parameters.AddWithValue("@Genre", genres);
                        cmd.Parameters.AddWithValue("@DirectorName", DirectorName);
                        cmd.Parameters.AddWithValue("@CompanyName", CompanyName);
                        cmd.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);
                        cmd.Parameters.AddWithValue("@Language", Language);
                        cmd.Parameters.AddWithValue("@Releases", Releases);
                        cmd.Parameters.AddWithValue("@Price", Price);
                        cmd.Parameters.AddWithValue("@Ratings", Ratings);
                        cmd.Parameters.AddWithValue("@GameDescription", GameDescription);
                        cmd.Parameters.AddWithValue("@RentalStock", RentalStock);
                        cmd.Parameters.AddWithValue("@CurrentStock", CurrentStock);
                        cmd.Parameters.AddWithValue("@GameImage", string.IsNullOrEmpty(HiddenImagePath) ? "images/game.png" : HiddenImagePath);

                        await cmd.ExecuteNonQueryAsync();
                    }

                    TempData["SuccessMessage"] = "Game added successfully!";
                }
                ModelState.Clear(); // Clears form fields to ensure no stale data lingers
                LoadGames(); // ensures Games list is loaded and not cleared by the Check button.
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                LoadGames(); // ensures Games list is loaded and not cleared by the Check button.
                return Page();
            }
        }

        //Update button
        public async Task<IActionResult> OnPostUpdateGameAsync()
        {
            try
            {
                // File Upload Handling - Saves the image in the correct place.
                if (GameImage != null && GameImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    string uniqueFileName = $"{Guid.NewGuid()}_{GameImage.FileName}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await GameImage.CopyToAsync(fileStream);
                    }

                    HiddenImagePath = $"images/{uniqueFileName}";
                }
                else
                {
                    HiddenImagePath = string.IsNullOrWhiteSpace(HiddenImagePath) ? "images/game.png" : HiddenImagePath;
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = @"
                        UPDATE game_master_tbl
                        SET 
                            game_name = @GameName,
                            genre = @Genre,
                            director_name = @DirectorName,
                            company_name = @CompanyName,
                            release_date = @ReleaseDate,
                            language = @Language,
                            releases = @Releases,
                            price = @Price,
                            ratings = @Ratings,
                            game_description = @GameDescription,
                            rental_stock = @RentalStock,
                            current_stock = @CurrentStock,
                            game_img_link = @GameImage
                        WHERE game_id = @GameID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@GameName", GameName);
                        cmd.Parameters.AddWithValue("@Genre", string.Join(",", Genre ?? new List<string>()));
                        cmd.Parameters.AddWithValue("@DirectorName", DirectorName);
                        cmd.Parameters.AddWithValue("@CompanyName", CompanyName);
                        cmd.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);
                        cmd.Parameters.AddWithValue("@Language", Language);
                        cmd.Parameters.AddWithValue("@Releases", Releases);
                        cmd.Parameters.AddWithValue("@Price", Price);
                        cmd.Parameters.AddWithValue("@Ratings", Ratings);
                        cmd.Parameters.AddWithValue("@GameDescription", GameDescription);
                        cmd.Parameters.AddWithValue("@RentalStock", RentalStock);
                        cmd.Parameters.AddWithValue("@CurrentStock", CurrentStock);
                        cmd.Parameters.AddWithValue("@GameImage", HiddenImagePath);
                        cmd.Parameters.AddWithValue("@GameID", GameID);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "Game details updated successfully!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Failed to update game details. Game ID may not exist.";
                        }
                    }
                }

                LoadGames(); // Refresh the list after update
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the game: " + ex.Message;
                return Page();
            }
        }



        void clearForm()
        {
            GameID = "";
            GameName = "";
            Genre = null;  // Set to null or a default value if it's a dropdown
            DirectorName = "";
            CompanyName = "";
            ReleaseDate = "";
            Language = "";
            Releases = "";
            Price = "";
            Ratings = "";
            GameDescription = "";
            RentalStock = "";
            GameImage = null; // If GameImage is an IFormFile, set it to null
        }


        //Delete button S
        public async Task<IActionResult> OnPostDeleteGameAsync()
        {
            if (string.IsNullOrWhiteSpace(GameID))
            {
                TempData["ErrorMessage"] = "Game ID cannot be Blank!";
                return Page(); //Return the page early to stop execution of the code
            }

            else
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        string query = "DELETE from game_master_tbl WHERE game_id = @GameID";

                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@GameID", GameID?.Trim());

                            int affectedRows = cmd.ExecuteNonQuery(); // Check if the conection is successful variable.

                            connection.Close();

                            if (affectedRows > 0)
                            {
                                TempData["SuccessMessage"] = "Game deleted successfully.";
                                clearForm(); //Clears Game Details
                                LoadGames(); // Refresh the list
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Game not found.";
                            }
                        }
                    }
                }
                catch (SqlException sqlEx) // Specific handling for SQL errors
                {
                    TempData["ErrorMessage"] = "Database error occurred while deleting the Game.";
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting the Game.";
                    // Log the error for better debugging
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            clearForm(); // Clears Game Details
            ModelState.Clear(); // Ensures the cleared form reflects on the page
            LoadGames(); // Refresh the list
            return Page();

        }

        //User functions
        void fillDeveloperCompanyValues()
        { 
        }

        void addNewGame()
        {
        }

        //Load Game LIst
        private void LoadGames()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string query = @"
                SELECT game_id, game_name, genre, director_name, company_name, 
                       release_date, language, releases, price, ratings, 
                       game_description, rental_stock, current_stock, game_img_link
                FROM game_master_tbl
                ORDER BY game_name";
            //"OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using var command = new SqlCommand(query, connection);
            //command.Parameters.AddWithValue("@SearchTerm", "%" + SearchTerm + "%");
            //command.Parameters.AddWithValue("@Offset", (PageNumber - 1) * PageSize);
            //command.Parameters.AddWithValue("@PageSize", PageSize);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                InventoryList.Add(new Game
                {
                    GameID = reader["game_id"].ToString(),
                    GameName = reader["game_name"].ToString(),
                    Genre = reader["genre"].ToString(),
                    DirectorName = reader["director_name"].ToString(),
                    CompanyName = reader["company_name"].ToString(),
                    ReleaseDate = reader["release_date"].ToString(),
                    Language = reader["language"].ToString(),
                    Releases = reader["releases"].ToString(),
                    Price = reader["price"].ToString(),
                    Ratings = reader["ratings"].ToString(),
                    GameDescription = reader["game_description"].ToString(),
                    RentalStock = reader["rental_stock"].ToString(),
                    CurrentStock = reader["current_stock"].ToString(), // Added this to change if rental stock changes - keep track of this
                    GameImage = reader["game_img_link"].ToString()
                });
            }
        }

    }
}
