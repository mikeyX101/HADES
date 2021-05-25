using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models
{
    public class ThemeFileExistAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            string themeFileName = null;
            string themeFilePath = null;
            if (value is string)
            {
                themeFileName = (string) value;
                themeFilePath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\css\\" + themeFileName;
                if (File.Exists(themeFilePath))
                {
                    return ValidationResult.Success;
                }
            }
            string defaultErrorMessage = "Le fichier " + themeFileName + ".css n'existe pas";
            return new ValidationResult(defaultErrorMessage);
        }
    }
}
