using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs;

namespace UrlShortener.Application.Validation
{
    public class CreateShortUrlDtoValidator : AbstractValidator<CreateShortUrlDto>
    {
        public CreateShortUrlDtoValidator()
        {
            RuleFor(x => x.OriginalUrl)
                .NotEmpty().WithMessage("Original URL is required.")
                .Must(BeAValidUrl).WithMessage("Original URL must be a valid absolute URL.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
