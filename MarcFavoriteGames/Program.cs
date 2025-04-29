using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using glibraryDB.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
var connectionString = builder.Configuration.GetConnectionString("strcon");

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication("MyAuthScheme") // Authentication cookie style
    .AddCookie("MyAuthScheme", options =>
    {
        options.LoginPath = "/admin";          // Redirect to login if not authenticated
        options.AccessDeniedPath = "/AccessDenied"; // Redirect here if access is denied
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // session will timeout after 30 min.
        options.SlidingExpiration = true;
    });

var app = builder.Build();
app.UseRouting();
app.UseAuthentication();  // Auth middleware must come first
app.UseAuthorization();   // Ensure role-based access logic works
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.MapGet("/", () => Results.Redirect("/Home")); Direct Routing just in case

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapRazorPages();

app.UseSession();
app.Run();
