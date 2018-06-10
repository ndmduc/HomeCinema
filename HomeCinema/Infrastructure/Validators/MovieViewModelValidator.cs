using FluentValidation;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Infrastructure.Validators
{
    public class MovieViewModelValidator : AbstractValidator<MovieViewModel>
    {
        public MovieViewModelValidator()
        {
            RuleFor(m => m.GenreId).GreaterThan(0).WithMessage("Select a Genre");
            RuleFor(m => m.Director).NotEmpty().Length(1, 100).WithMessage("Select a Director");
            RuleFor(m => m.Writer).NotEmpty().Length(1, 50).WithMessage("Select a Writer");
            RuleFor(m => m.Producer).NotEmpty().Length(1, 50).WithMessage("Select a Producer");
            RuleFor(m => m.Description).NotEmpty().WithMessage("Select a Description");
            RuleFor(m => m.Rating).InclusiveBetween((byte)0, (byte)5).WithMessage("Rating must be less than or equal to 5");
            RuleFor(m => m.TrailerURI).NotEmpty().Must(ValidTrailerUri).WithMessage("Only Youtube trailers are supported");
        }

        private bool ValidTrailerUri(string trailerUri)
        {
            return (!string.IsNullOrEmpty(trailerUri) && trailerUri.ToLower().StartsWith("https://www.youtube.com/watch?"));
        }
    }
}