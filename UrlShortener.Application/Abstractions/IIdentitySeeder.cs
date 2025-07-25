using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.Abstractions
{
    public interface IIdentitySeeder
    {
        Task SeedRolesAsync();
        Task SeedAdminUserAsync(string email, string password);
    }
}
