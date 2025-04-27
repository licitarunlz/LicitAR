using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Utils.DataAnnotations
{ 

    public class ValidCuitAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            string cuit = value.ToString()?.Replace("-", "").Trim() ?? string.Empty;

            if (cuit.Length != 11 || !long.TryParse(cuit, out _))
                return new ValidationResult(ErrorMessage ?? "El CUIT no es válido.");

            int[] mult = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int total = 0;

            for (int i = 0; i < 10; i++)
            {
                total += int.Parse(cuit[i].ToString()) * mult[i];
            }

            int mod = total % 11;
            int digitoVerificador = mod == 0 ? 0 : mod == 1 ? 9 : 11 - mod;

            return digitoVerificador == int.Parse(cuit[10].ToString()) 
                ? ValidationResult.Success 
                : new ValidationResult(ErrorMessage ?? "El CUIT no es válido.");
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-cuit", ErrorMessage ?? "El CUIT no es válido.");
        }
    }

}
