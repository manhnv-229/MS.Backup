using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.CustomValidation
{
    public class NumberGreaterZero: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;
            else
            {
                var propName = validationContext.MemberName;
                Decimal number;
                if (Decimal.TryParse(value.ToString(), out number))
                {
                    if (number < 0)
                    {
                        if (ErrorMessage != null)
                        {
                            return new ValidationResult(this.ErrorMessage);

                        }
                        else
                        {
                            return new ValidationResult($"{propName} phải lớn hơn hoặc bằng 0.");
                        }
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return new ValidationResult($"{propName} phải có định dạng là số.");
                }

            }
        }
    }
}
