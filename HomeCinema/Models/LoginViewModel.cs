using HomeCinema.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeCinema.Models
{
    public class LoginViewModel : IValidatableObject
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new LoginViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(v => new ValidationResult(v.ErrorMessage, new[] { v.PropertyName }));
        }
    }
}