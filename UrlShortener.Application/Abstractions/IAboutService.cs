using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs;

namespace UrlShortener.Application.Abstractions
{
    public interface IAboutService
    {
        Task<AboutDto> GetAboutInfoAsync();
        Task UpdateAboutInfoAsync(UpdateAboutDto dto, string userId);
    }
}
