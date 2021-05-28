using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HADES.Attributes
{
    public class OuNameAttribute : ValidationAttribute
    {
        private static readonly char[] forbiddenChar = { '"', '[', ']', ':', ';', '|', '=', '+', '*', '?', '<', '>', '/', '\\', ',' };

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value is string)
            {
                
                var ouNameValue = (string)value;
                var match = ouNameValue.IndexOfAny(forbiddenChar) != -1;
                if (!match)
                {
                    return ValidationResult.Success;
                }
            }
            string defaultErrorMessage = "Caractères interdits: " + string.Join(",", forbiddenChar);
            return new ValidationResult(defaultErrorMessage);
        }
    }
}
