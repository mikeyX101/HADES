using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HADES.Util;
using HADES.Util.ModelAD;

namespace HADES.Attributes
{
    public class OuGroupNameAttribute : ValidationAttribute
    {
        private static readonly char[] forbiddenChar = { '"', '[', ']', ':', ';', '|', '=', '+', '*', '?', '<', '>', '/', '\\', ',' };

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            string defaultErrorMessage = "";
            if (value is string)
            {
                var ouGroupName = (string)value;
                var match = ouGroupName.IndexOfAny(forbiddenChar) != -1;
                if (!match)
                {
                    ADManager adManager = new ADManager();
                    var adRoot = adManager.getRoot();
                    ItemType itemType = context.ObjectType.Name.Equals("GroupAD") ? ItemType.GROUP : ItemType.OU;
                    if (!alreadyExists(adRoot, ouGroupName, itemType))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        defaultErrorMessage += string.Format(Strings.OuGroupNameAlreadyExistsError, ouGroupName);
                    }
                }
                else
                {
                    string forbChar = string.Join(" ", forbiddenChar);
                    defaultErrorMessage += string.Format(Strings.OuGroupNameFobiddenCharactersError, forbChar);
                }
            }
            
            return new ValidationResult(defaultErrorMessage);
        }

        private bool alreadyExists(List<RootDataInformation> root, string ouOrGroupName, ItemType type)
        {
            bool exists = false;
            foreach (var item in root)
            {
                if (item.Type.Equals(type.ToString().ToLower()) && item.SamAccountName.Equals(ouOrGroupName))
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }
    }

    public enum ItemType
    {
        OU,
        GROUP
    }

}
