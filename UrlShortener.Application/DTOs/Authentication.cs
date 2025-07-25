using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs
{
    public record LoginDto(string Email, string Password);
    public record RegisterDto(string Email, string Username, string Password);
    public record AuthResponseDto(string Token, string Username, string Role);
    public record UserInfoDto(string Id, string Email, string Username, string Role);

}
