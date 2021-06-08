using System.ComponentModel.DataAnnotations;

namespace HADES.Attributes
{
	public class OuGroupNameAttribute : ValidationAttribute
    {
        private static readonly char[] forbiddenChar = { '"', '[', ']', ':', ';', '|', '=', '+', '*', '?', '<', '>', '/', '\\', ',' };

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value is string)
            {
                
                var ouNameValue = (string) value;
                var match = ouNameValue.IndexOfAny(forbiddenChar) != -1;
                if (!match)
                {
                    return ValidationResult.Success;
                }
            }
            string forbChar = string.Join(" ", forbiddenChar);
            string defaultErrorMessage = string.Format(Strings.OuGroupNameError, forbChar);
            return new ValidationResult(defaultErrorMessage);
        }
    }
}
