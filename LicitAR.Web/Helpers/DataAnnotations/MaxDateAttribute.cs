using System;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Core.Utils.DataAnnotations
{

    public class MaxTodayAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is DateTime dateValue && dateValue.Date <= DateTime.Today)
                return ValidationResult.Success;

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} no puede ser mayor a la fecha actual.";
        }
    }
}