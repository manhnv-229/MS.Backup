using MS.Core.Entities;
using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    [DataTableName("User")]
    public class UserRequest:BaseEntity
    {
        [Required("Thông tin tài khoản không được để trống.")]
        [NotDuplicate]
        [LabelText("Tài khoản")]
        public string UserName { get; set; }

        [Required("Mật khẩu không được phép để trống.")]
        public string Password { get; set; }

        public string Repassword { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? UserId { get; set; }
    }
}
