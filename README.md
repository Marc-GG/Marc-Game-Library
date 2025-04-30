# Marc's Gaming Library
Goals of this project
1. Create a Online LIbrary System using Asp.Net  
2. Showcase relation between the webpage, the C# code, and the database  
3. Create basic CRUD applications on the website  
4. Create user profiles with limited permissions and a admin profile with access to everything
5. Create a inventory of games that can be used for rental purposes.

![Website](images/website.png)

## Setup
-Used Visual Studios 2022, Microsoft SQL Server Management Studio, and SQL Server 2022 configuration  
-Used ASP.NET Razor Pages  
-BootStrap and Jquery are added as libraries by default in Visual Studios 2022  
-Added libraries: Fontawesome, Datatables, and AJax

## Layout
-By default you are given a _Layout page which is the same as the Master page of older asp.net.  
-I have sectioned the webpage into 3 parts by Header, Main Body, and Footer.
  * The Header and footer styling stay the same on every page, but the middle content will always change depending on the page.
-Additional Settings
  * _ViewStart.cshtml must run Layout Page first

## Database Setup
-Above in the repository is a script of the tables I made in the Database.
-Created a profile on SQL management Studio  
 * Created several tables for receiving data
 * Set authentication to mandatory / No password
-Created a Connection String - This will link the code to the database.
 * In appsettings.json : strcon":"Server=localhost\\SQLEXPRESS;Database=glibraryDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"

## CRUD
-This web app shares a similar structure throughout its functionality.  
1. Object / Item is created, added to the database, displayed on the web app to be read.
2. Object can be updated or edited via the forms.
3. Object can be deleted.  
-Whether it be the users or the game, the CRUD functionality exists.

## User Signup
- My entire website follows the same format for the construction and connectivity of the process.  
  * Using Form method to encompass all fields and the button that triggers the process.
  * Stored in bounded property strings, or list of strings.
  * Function is ran > using connection string > con.Open() > Run sql command query
  * Parameter is created with the database placeholder, the stored string is passed into the database.
- Check if user already exists or not, if not, signup is successful, redirect to user login page.

## User Login / Admin Login
- By default give everyone restricted access.
- Release restrictions and allow users who are logged in to access certain pages using a session.
- Session timer can be edited and is currently set to 30 min.
- During the session, whether the page is reset or any changes occur, the page will save the current data.
* Uses Claims
  * Create a claim with using only the id and a role.
  * Create a identity which holds the claims and the authentication scheme.
  * Create a principal in case of more claims being created.

## Member Management
- On click of the Go button, fields are sent to string storages
  * Connection is created, query SELECT to get information from database, and by using reader.Read() it fills the forms with the data.
- Update Status just updates status.
- Delete Option - Just delets the Member ID. In deleting the primary key, all corresponding table elements are also deleted.
- LoadMembers() - Loads data into the tables. / OnGet function
- GridView and Datatables. (DataTables is used so that I dont have to rawcode a table and its manipulators.)

## Inventory Management
- Upon filling the fields you can add the game to the database, and it will show up on the gridview.
  * By entering a GameID and clicking the checkmark it will fill the necessary fields with details from the database.
  * You can also update and delete, but only if all fields are filled with data already form the database.
- Image and preview image - some Javascript used.

## Future Plans
The Project is still ongoing. I have not completed the rental system for the Library. The idea is that you can choose a game available in stock and rent it. The stock will change accordingly to what is rented. A user that is signed in will also have a profile page that they can edit and it will show the games they rented. The Admin user should also be able to delete or edit profile details.  

Some other things that need to be added:  
-Data filters. There should be restrictions on the names of items that are inputed.  
-Email filters. If an email is not real, it should not be able to be added.  
-Usernames should be a valid and restricted to a certain style.  
-Status updates should be tied with user restricitons.  
-Maybe a password strength modifier.  

This project was just implemented for personal showcase of my skills, but it is possible that I can create a fully funcitonal website.  
-Deployment through the Azure hosting  
-Use a Steam Api for me to add actual game codes.  
-Game Data Api from IGN api(Ratings, Releases, and etc).  
-Transforming the Library to a personal website with the library just being a small part of the website.  
-Add more functional pages. Maybe a Forums page.  

## Resources
-Bootstrap - https://getbootstrap.com/  
-Data tables - https://datatables.net/  
-Font Awesome - https://fontawesome.com/   
-Visual studios 2022 - https://visualstudio.microsoft.com/  
-SQL Studios Management  - https://www.microsoft.com/en-us/sql-server/sql-server-downloads  
-SQL server configuration management - https://learn.microsoft.com/en-us/sql/tools/configuration-manager/sql-server-configuration-manager?view=sql-server-ver16  
