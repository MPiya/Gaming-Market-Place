using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using F2Play.DataAccess.Repository.IRepository;
using F2Play.DataAccess.DbInitializer;
using F2Play.DataAccess.Repository;
using F2Play.DataAccess.Data;
using F2Play.Utility;
using Serilog.Formatting.Compact;
using Serilog;
using Infrastructure.LogConfiguration;

var builder = WebApplication.CreateBuilder(args);

//loogging
var loggerConfiguration = new LoggerConfiguration()
//usingCustom Method
.ConfigureLogging();
// Read settings from appsettings.json
var logSettings = builder.Configuration.GetSection("Logging");
loggerConfiguration.WriteTo.File(new CompactJsonFormatter(), logSettings["LogPath"], rollingInterval: RollingInterval.Day);
Log.Logger = loggerConfiguration.CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

//This one for localDB
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer
(builder.Configuration.GetConnectionString("ApplicationDbContextConnection")));

//// This is for Azure DB
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer
//(builder.Configuration.GetConnectionString("DefaultConnection")));

// Accesss to configure
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

//This override Path default from Indentity
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Stripe
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
SeedDatabase();


app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{Area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();


void SeedDatabase()
{
	using (var scope = app.Services.CreateScope())
	{
		var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		dbInitializer.Initialize();
	}
}