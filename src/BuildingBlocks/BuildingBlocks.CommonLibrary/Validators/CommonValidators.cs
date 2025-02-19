using FluentValidation;
using System.Text.RegularExpressions;

namespace BuildingBlocks.CommonLibrary.Validators
{
    public static class CommonValidators
    {
        public static IRuleBuilderOptions<T, string> MustBeValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email address is required");
        }

        public static IRuleBuilderOptions<T, string> MustBeValidPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("A valid phone number is required");
        }

        public static IRuleBuilderOptions<T, string> MustBeValidGuid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .Must(guid => Guid.TryParse(guid, out _))
                .WithMessage("A valid GUID is required");
        }

        public static IRuleBuilderOptions<T, DateTime> MustBeValidDate<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .Must(date => date != default)
                .WithMessage("A valid date is required");
        }

        public static IRuleBuilderOptions<T, string> MustBeValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        }

        public static IRuleBuilderOptions<T, decimal> MustBePositiveAmount<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero");
        }
    }
}