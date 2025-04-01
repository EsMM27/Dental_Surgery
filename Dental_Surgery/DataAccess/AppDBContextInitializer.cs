using Dental.DataAccess;
using Dental.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

public class AppDBContextInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //await context.Database.MigrateAsync();

            const string userName = "admin@admin.com";
            const string userNameNormal = "ADMIN@ADMIN.COM";
            const string emailNormal = "ADMIN@ADMIN.COM";
            const string email = "admin@admin.com";
            const string password = "Admin@123";
            const string roleName = "Admin";

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new IdentityUser { UserName = userName, Email = email, NormalizedUserName = userNameNormal, NormalizedEmail = emailNormal };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Ensure the user is saved before adding to role
                    user = await userManager.FindByNameAsync(userName);
                    if (user != null)
                    {
                        await userManager.AddToRoleAsync(user, roleName);
                    }
                }
                else
                {
                    throw new Exception("Failed to create user");
                }
            }
            else
            {
                // User already exists, add to role
                Console.WriteLine("User already exists");
				if (!await userManager.IsInRoleAsync(user, roleName))
				{
					await userManager.AddToRoleAsync(user, roleName);
				}
			}
        }
    }
}
