using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HADES.Attributes
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
