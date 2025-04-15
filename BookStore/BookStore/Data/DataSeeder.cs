using BookStore.Models;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Data
{
    public static class DataSeeder
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public static readonly string[] Roles = { User, Admin };

        public static async void SeedRolesAndAdmin(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            foreach (var role in Roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var admin = new User { UserName = "Mohamed_Abdo", Email = "Moh123@gmail.com" };
            if(await userManager.FindByEmailAsync(admin.Email) == null)
            {
                var result = await userManager.CreateAsync(admin, "Moh1234@");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
