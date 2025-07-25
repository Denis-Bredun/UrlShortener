using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;
using UrlShortener.Infrastructure.Exceptions;

namespace UrlShortener.Infrastructure.Services
{  
    public class IdentitySeeder(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager) : IIdentitySeeder
    {
        public async Task SeedRolesAsync()
        {
            string[] roles = { "User", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                        throw new RoleCreationException(string.Join("; ", result.Errors));
                }
            }
        }

        public async Task SeedAdminUserAsync(string email, string password, string username)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return;

            var adminUser = new IdentityUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, password);
            if (!result.Succeeded)
                throw new UserCreationException(string.Join("; ", result.Errors));

            var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (!addRoleResult.Succeeded)
                throw new RoleAssignmentException(string.Join("; ", addRoleResult.Errors));
        }
    }

}
