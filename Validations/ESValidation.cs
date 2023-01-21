using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RIS.Validations
{
    /// <summary>
    /// Custom validation for attributes used as server side validation for required attributes edited by RIS Admin
    /// </summary>
    public class ESValidation : ValidationAttribute
    {

        /// <summary>
        /// Overriding of IsValid function of the attribute validation.
        /// </summary>
        /// <returns>Validation Success if valid, validation error message if not</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool needed = RIS.Models.RequiredValues.isAttRequired(validationContext.MemberName);
            try
            {
                if (needed)
                {
                    if (String.IsNullOrEmpty(value.ToString().Replace(" ", "")))
                        return new ValidationResult(validationContext.DisplayName + " " + RIS.Resources.Res.NotEmptyAtt);
                    else
                        return ValidationResult.Success;
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            catch
            {
                return new ValidationResult(validationContext.DisplayName + " " + RIS.Resources.Res.NotEmptyAtt);
            }

        }
    }
}