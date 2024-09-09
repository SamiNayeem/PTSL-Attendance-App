using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PTSLAttendanceManager.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Enforce issuer validation
            ValidateAudience = false, // Enforce audience validation
            ValidateLifetime = true, // Validate the expiration and not-before times
            ValidateIssuerSigningKey = true, // Validate the signature of the token
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Define the valid issuer
            ValidAudience = builder.Configuration["Jwt:Audience"], // Define the valid audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Key used to validate the token signature
        };
    });

// Add services to the container, such as controllers and other services
builder.Services.AddControllers();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Enable the authentication middleware
app.UseAuthorization();  // Enable the authorization middleware

app.MapControllers(); // Ensure all controllers are mapped

// Optionally define a default route for MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
