﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.Abstractions
{
    public interface IAboutInfoSeeder
    {
        Task SeedAsync(string adminEmail);
    }
}
