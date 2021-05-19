using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models
{
    public class LanguageExistAttribute : ValidationAttribute
    {
        private static readonly string[] languages = { "fr-CA", "en-US", "es-US", "pt-BR" };

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value is string)
            {
                var languageValue = (string) value;
                if (languages.Contains(languageValue))
                {
                    return ValidationResult.Success;
                }
            }
            string defaultErrorMessage = "La langue doit être parmi: " + string.Join(",", languages);
            return new ValidationResult(defaultErrorMessage);
        }
    }
}
