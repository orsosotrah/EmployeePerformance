using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BuildingBlocks.CommonLibrary.Validators
{
    public static class ValidationExtensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        public static IDictionary<string, string[]> ToDictionary(this ValidationResult validationResult)
        {
            return validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
        }

        public static string ToFlatString(this ValidationResult validationResult)
        {
            return string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
        }

        public static ValidationFailureResponse ToResponse(this ValidationResult validationResult)
        {
            return new ValidationFailureResponse
            {
                Errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            };
        }
    }

    public class ValidationFailureResponse
    {
        public IDictionary<string, string[]> Errors { get; set; }
    }
}