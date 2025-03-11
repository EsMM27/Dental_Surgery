using Microsoft.EntityFrameworkCore;
using Dental.DataAccess;
using Dental.DataAccess.Repo;
using Dental.Service;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

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

builder.Services.AddScoped<IAppointmentRepo, AppointmentRepo>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();

app.Run();
