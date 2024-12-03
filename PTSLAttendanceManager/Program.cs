using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PTSLAttendanceManager.Data;
using PTSLAttendanceManager.Jobs;
using PTSLAttendanceManager.Models;
using PTSLAttendanceManager.Models.Entity;
using PTSLAttendanceManager.Services;
using Quartz;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure DbContext with SQL Server
//string connectionStringtest= "Server=103.95.97.156;Database=PTSLAttendance;User Id=sa;Password=ptsl@sql2022;  TrustServerCertificate=True;";
string encryptionKey = "sT9y3X7kfE&ZDg6q";
//string encryptedConnectionString = EncryptoEngine.Encrypt(connectionStringtest, encryptionKey);
//Console.WriteLine("abcd");


string connectionString = EncryptoEngine.Decrypt(builder.Configuration.GetConnectionString("DefaultConnection"), encryptionKey);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add HttpClient service
builder.Services.AddHttpClient();

// Add Firebase service
builder.Services.AddSingleton<FirebaseService>(); // Consider changing to AddScoped or AddTransient if needed

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });

// Fetch the CRON expression from the database
string cronExpression = "0 59 23 ? * 7,1-4"; // Default CRON if not found

using (var tempScope = builder.Services.BuildServiceProvider().CreateScope())
{
    var context = tempScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var scheduleTime = await context.Set<ScheduleTime>().FirstOrDefaultAsync();
    if (scheduleTime != null && !string.IsNullOrWhiteSpace(scheduleTime.JobTime))
    {
        cronExpression = scheduleTime.JobTime;
    }
}

//Add Quartz services with the fetched CRON expression
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

// Define the job and trigger
var jobKey = new JobKey("CheckOutUpdateJob");
q.AddJob<CheckOutUpdateJob>(opts => opts.WithIdentity(jobKey));

q.AddTrigger(opts => opts
    .ForJob(jobKey)
    .WithIdentity("CheckOutUpdateTrigger")
    .WithCronSchedule(cronExpression)); // Use the dynamic CRON expression
});

//// Add Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
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
