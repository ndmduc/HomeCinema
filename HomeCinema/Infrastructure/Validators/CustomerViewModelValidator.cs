using FluentValidation;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Infrastructure.Validators
{
    public class CustomerViewModelValidator : AbstractValidator<CustomerViewModel>
    {
        public CustomerViewModelValidator()
        {
            RuleFor(c => c.FirstName).NotEmpty().Length(1, 100).WithMessage("First name must be between 1 - 100 characters.");
            RuleFor(c => c.LastName).NotEmpty().Length(1, 100).WithMessage("Last name must be between 1 - 100 characters.");
            RuleFor(c => c.IdentifyCard).NotEmpty().Length(1, 50).WithMessage("Identify card must be between 1 - 50 characters.");
            RuleFor(c => c.DateOfBirth).NotNull().LessThan(DateTime.Now.AddYears(-16)).WithMessage("Customer must be at least 16 years old.");
            RuleFor(c => c.Mobile).NotNull().Matches(@"^d{10}$").Length(10).WithMessage("Mobile phone must be have 10 digits;");
            RuleFor(c => c.Email).NotNull().EmailAddress().WithMessage("Enter a valid email address");
        }
    }
}