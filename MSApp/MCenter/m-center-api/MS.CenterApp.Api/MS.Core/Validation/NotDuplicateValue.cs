using MS.Core.Helpers;
using MS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.CustomValidation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class NotDuplicateValue : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var propName = validationContext.DisplayName;

            var objInstance = validationContext.ObjectInstance;

            var genericType = typeof(IAsyncRepository<>).MakeGenericType(validationContext.ObjectType);

            // Lấy ra đối tượng được khởi tạo trong service (config từ DI)
            var context = validationContext.GetService(genericType);

            // Lấy ra phương thức CheckDuplicate:
            var method = context?.GetType().GetMethod("CheckDuplicate");

            var task = (Task?)method?.Invoke(context, new object[] { value, propName, objInstance });

            if (task != null)
            {
                task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");
                var add = (bool)resultProperty?.GetValue(task);
                if (add == false)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    if (ErrorMessage != null)
                    {
                        return new ValidationResult(this.ErrorMessage);

                    }
                    else
                    {
                        return new ValidationResult($"Thông tin {propName} đã có trong hệ thống.");
                    }
                }
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
