using System;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Core.Utils.DataAnnotations
{

    public class MaxTodayAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            if (value is DateTime dateValue)
                return dateValue.Date <= DateTime.Today;

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} no puede ser mayor a la fecha actual.";
        }
    }
}