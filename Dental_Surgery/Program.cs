using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.DataAccess.Repo;
using Dental.Service;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Identity;
using Dental_Surgery.DataAccess.Repo;
using Dental_Surgery.Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Load Configuration (before reading settings)
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Check if running in Production (Azure)
        string connectionString;

        if (builder.Environment.IsDevelopment())
        {
            // Use Local Database Connection String
            connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine("Using Local Development Database.");
        }
        else
        {
            // Use Azure Key Vault Connection String
            var keyVaultUrl = builder.Configuration["KeyVaultUrl"];

            if (string.IsNullOrEmpty(keyVaultUrl))
            {
                throw new InvalidOperationException("KeyVaultUrl is not set in environment variables.");
            }

            Console.WriteLine($"Using Azure Key Vault at: {keyVaultUrl}");

            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            var secret = await client.GetSecretAsync("dentalconnection");
            connectionString = secret.Value.Value;

            Console.WriteLine("Using Azure SQL Database from Key Vault.");
        }

        // Register DbContext
        builder.Services.AddDbContext<AppDBContext>(options =>
            options.UseSqlServer(connectionString, sqlServerOptions =>
                sqlServerOptions.MigrationsAssembly("Dental_Surgery")
            ));

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        // Register Services
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddScoped<IAppointmentRepo, AppointmentRepo>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Email Service
        builder.Services.AddTransient<IEmailService, EmailService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        await app.CreateRolesAsync(builder.Configuration);

        app.UseAuthentication();
        app.UseAuthorization();

        //Login page is the first thing user sees if not logged in
        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/" && !context.User.Identity.IsAuthenticated)
            {
                context.Response.Redirect("/Login");
                return;
            }
            await next();
        });

        app.MapRazorPages();
        app.MapBlazorHub();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            await AppDBContextInitializer.SeedAsync(services);
        }

        app.Run();
    }
}

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> CreateRolesAsync(this WebApplication app, IConfiguration configuration)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));
        var roles = configuration.GetSection("Roles").Get<List<string>>();

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        return app;

    }
}




