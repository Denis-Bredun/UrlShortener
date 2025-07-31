using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs;


// Recommended: namespace; not namespace{}

namespace UrlShortener.Application.Validation
{
    public class UpdateAboutDtoValidator : AbstractValidator<UpdateAboutDto>
    {
        public UpdateAboutDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description must not be empty.")
                .MaximumLength(2000).WithMessage("Description must be at most 2000 characters long.");
        }
    }
}
