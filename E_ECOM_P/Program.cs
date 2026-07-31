using E_COM_DataAccess.Data;
using E_COM_DataAccess.Repository;
using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using E_ECOM_P.Services;
using E_ECOM_P.Services.ServiceInterfaces;
using E_EOM_Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("constr") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
//builder.Services.AddScoped<IcategoryRepository,CategoryRepository>();   //added to access in controller but not good for reusability//
//builder.Services.AddScoped<ICoverTypeRepository, CoverTypeRepository>();//coz if there are 1000 models 1000 times condition to be written in program.cs
//so we should make single interface to acess models in it and add that interface in program.cs for reusability//
builder.Services.AddScoped<IUnitofWork,UnitofWork>();

//builder.Services.AddDefaultIdentity
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
//This makes services like UserManager, SignInManager, and RoleManager available for use.^


builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Social login providers (Google/Facebook) - reads credentials from configuration
// (appsettings.json locally, or User Secrets / environment variables in production).
// Only registers a provider if real credentials have been filled in, so the app
// doesn't break or show broken buttons when they're left as placeholders.
var googleClientId = builder.Configuration["GoogleAuth:ClientId"];
var googleClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
var facebookAppId = builder.Configuration["FacebookAuth:AppId"];
var facebookAppSecret = builder.Configuration["FacebookAuth:AppSecret"];

var authBuilder = builder.Services.AddAuthentication();

if (!string.IsNullOrWhiteSpace(googleClientId) && !googleClientId.StartsWith("REPLACE_WITH"))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
    });
}

if (!string.IsNullOrWhiteSpace(facebookAppId) && !facebookAppId.StartsWith("REPLACE_WITH"))
{
    authBuilder.AddFacebook(options =>
    {
        options.AppId = facebookAppId;
        options.AppSecret = facebookAppSecret;
    });
}

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<StripeSetting>
    (builder.Configuration.GetSection("StripeSetting"));


//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//builder.Services.AddTransient<IEmailservice, EmailService>();

builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));
builder.Services.AddTransient<ISmsService, SmsService>();
builder.Services.AddTransient<IVoiceservice, VoiceService>();











var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InitializeAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSetting")["SecretKey"];

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
