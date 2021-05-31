using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Attributes
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
                themeFilePath = "wwwroot/css/" + themeFileName + "/" + themeFileName + ".css";
                if (File.Exists(themeFilePath))
                {
                    return ValidationResult.Success;
                }
            }
            string defaultErrorMessage = string.Format(Strings.CssFileDoesNotExist, themeFileName);
            return new ValidationResult(defaultErrorMessage);
        }
    }
}
