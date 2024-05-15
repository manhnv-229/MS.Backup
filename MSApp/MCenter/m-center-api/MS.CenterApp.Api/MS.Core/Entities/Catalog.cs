using MS.Core.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Catalog : BaseEntity
    {
        public Guid TenantId { get; set; }

        [Required(ErrorMessage = "SubDomain thuê bao không được để trống")]

        [NotDuplicateValue(ErrorMessage ="Sub domain đã được sử dụng bởi thuê bao khác")]
        public string SubDomain { get; set; }

        public string? RootDomain { get; set; }

        [Required(ErrorMessage = "Server thuê bao không được để trống")]
        public string Server { get; set; }

        public int? Port { get; set; }

        [Required(ErrorMessage = "DatabaseName thuê bao không được để trống")]
        [NotDuplicateValue(ErrorMessage = "Tên cơ sở dữ liệu trùng lặp với đơn vị khác.")]
        public string DatabaseName { get; set; }

        [Required(ErrorMessage = "UserId thuê bao không được để trống")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Password thuê bao không được để trống")]
        public string Password { get; set; }

       
    }
}
