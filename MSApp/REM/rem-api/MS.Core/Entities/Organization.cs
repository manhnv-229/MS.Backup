using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Organization : BaseEntity
    {
        [Required("Mã cửa hàng không được để trống")]
        public string OrganizationCode { get; set; }

        [Required("Tên cửa hàng không được để trống.")]
        public string OrganizationName { get; set; }
        public string? OrganizationTypeName { get; set; }

        public Guid? ParentId { get; set; }

        public string? Address { get; set; }

        [Required("Số điện thoại không được để trống.")]
        public string? PhoneNumber { get; set; }

        [Required("Email không được để trống.")]
        public string? Email { get; set; }

        public string? Description { get; set; }
        public int? Level { get; set; }
        public string? Slogan { get; set; }
        public string? ShortDescription { get; set; }
        public string? OwnerName { get; set; }
        public string? BusinessLicense { get; set; }
        public string? Website { get; set; }

        public virtual ICollection<MSLicense>? MSLicense { get; set; }
        public virtual ICollection<Setting>? Setting { get; set; }
        public virtual ICollection<NavbarItem>? NavbarItem { get; set; }
        /// <summary>
        /// Đã xác nhận hay chưa?
        /// </summary>
        public bool IsConfirm { get; set; }
    }
}
