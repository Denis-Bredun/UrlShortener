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
    public class IdentitySeeder : IIdentitySeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public IdentitySeeder(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedRolesAsync()
        {
            string[] roles = { "User", "Admin" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                        throw new RoleCreationException(string.Join("; ", result.Errors));
                }
            }
        }

        public async Task SeedAdminUserAsync(string email, string password, string username)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return;

            var adminUser = new IdentityUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, password);
            if (!result.Succeeded)
                throw new UserCreationException(string.Join("; ", result.Errors));

            var addRoleResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
            if (!addRoleResult.Succeeded)
                throw new RoleAssignmentException(string.Join("; ", addRoleResult.Errors));
        }
    }

}
