using Microsoft.AspNetCore.Identity;

namespace GameStoreWeb
{
    public class AppDbInitializer
    {
        public static async Task SeedRolesToDb(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                if (!await roleManager.RoleExistsAsync("Manager"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Manager"));
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }
            }
        }
    }
}
