using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs;
using UrlShortener.Infrastructure.Exceptions;

namespace UrlShortener.Infrastructure.Services
{
    public class AuthService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration) : IAuthService
    {
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingEmail = await userManager.FindByEmailAsync(dto.Email);
            if (existingEmail != null)
                throw new UserCreationException("Email is already taken.");

            var existingUsername = await userManager.FindByNameAsync(dto.Username);
            if (existingUsername != null)
                throw new UserCreationException("Username is already taken.");

            var user = new IdentityUser
            {
                UserName = dto.Username,
                Email = dto.Email,
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new UserCreationException(string.Join("; ", result.Errors));

            var role = "User";
            var addRoleResult = await userManager.AddToRoleAsync(user, role);

            if (!addRoleResult.Succeeded)
                throw new RoleAssignmentException(string.Join("; ", addRoleResult.Errors));

            var token = await GenerateJwtTokenAsync(user);

            return new AuthResponseDto(token, user.UserName, role);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new InvalidCredentialsException();

            var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                throw new InvalidCredentialsException();

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault("User");

            var token = await GenerateJwtTokenAsync(user);

            return new AuthResponseDto(token, user.UserName, role);
        }

        public async Task<UserInfoDto> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new InvalidCredentialsException();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException();

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault("User");

            return new UserInfoDto(user.Id, user.Email, user.UserName, role);
        }

        private async Task<string> GenerateJwtTokenAsync(IdentityUser user)
        {
            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
