using MS.Core.CustomValidation;
using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    [DataTableName("Tenant")]
    [ViewDataName("Views_Tenant")]
    [DoNotGetByOranization]
    public class Tenant:BaseEntity
    {
        public Tenant()
        {
          
        }
        public Guid TenantId { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage ="Mã thuê bao không được để trống")]
        [NotDuplicateValue(ErrorMessage = "Mã thuê bao trùng lặp.")]
        public string TenantCode { get; set; }

        public string Description { get; set; }

        [NotDuplicateValue(ErrorMessage = "Số điện thoại đã đăng ký cho thuê bao khác.")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        [MSForeignKey("TenantId")]
        public virtual Organization? Organization { get; set; }

        [MSForeignKey("TenantId")]
        public virtual TenantUser? TenantUser { get; set; }

        [MSForeignKey("TenantId")]
        public virtual Catalog? Catalog { get; set; }


    }
}
