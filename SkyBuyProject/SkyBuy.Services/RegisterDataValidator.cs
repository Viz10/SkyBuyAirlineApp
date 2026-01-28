using FluentValidation;
using SkyBuy.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SkyBuy.Services
{
    public sealed class RegisterDataValidator : AbstractValidator<RegisterFormCustomerDTO>
    {
        public RegisterDataValidator() {

            RuleFor(obj => obj.Email)
               .NotEmpty().WithMessage("Email required!")
               .EmailAddress().WithMessage("Email format is invalid")
               .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

            RuleFor(obj => obj.Password)
                .NotEmpty().WithMessage("Password field cannot be empty!")
                .MaximumLength(128).WithMessage("Password field must not exceed 128 characters")
                .MinimumLength(8).WithMessage("Password field must have at least 8 characters")
                .Must(NotContainSpaces).WithMessage("Password field must not contain spaces")
                .Must(RespectPasswordCharacteristics).WithMessage("Password must contain at least: one uppercase letter, one lowercase letter, one digit, and one special character");

            RuleFor(obj => obj.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password field must not be empty!")
                .Equal(obj => obj.Password).WithMessage("Passwords don't match!");

            RuleFor(obj => obj.FirstName)
                .NotEmpty().WithMessage("First Name must not be empty!")
                .MaximumLength(100).WithMessage("First Name must not exceed 100 characters")
                .Must(ContainThingsForName).WithMessage("Invalid First Name format");

            RuleFor(obj => obj.LastName)
                .NotEmpty().WithMessage("Last Name must not be empty!")
                .MaximumLength(100).WithMessage("Last Name must not exceed 100 characters")
                .Must(ContainThingsForName).WithMessage("Invalid Last Name format");

            RuleFor(obj => obj.Phone)
                .NotEmpty().WithMessage("Phone must not be empty!")
                .MaximumLength(20).WithMessage("Phone must not exceed 20 characters")
                .Must(BeValidPhone).WithMessage("Invalid phone format");
                
                
            RuleFor(obj => obj.Address)
                .NotEmpty().WithMessage("Address must not be empty!")
                .MaximumLength(250).WithMessage("Address must not exceed 250 characters");
        }

        private bool NotContainSpaces(string field)
        {
            return !field.Contains(' ');
        }
        private bool RespectPasswordCharacteristics(string password)
        {
            return password.Any(char.IsDigit) && password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(ch => !char.IsLetterOrDigit(ch));
        }
        private bool ContainThingsForName(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z\s\-]+$");
        }
        private bool BeValidPhone(string phone)
        {
            /// Allow digits, spaces, +, -, (, )
            return Regex.IsMatch(phone, @"^[\d\s\+\-\(\)]+$");
        }
        }
}
