using MS.ApplicationCore.Helpers;
using MS.ApplicationCore.MSEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.ApplicationCore.Entities
{
    [DataTableName("User")]
    public class RegisterModel:BaseEntity
    {
        [Required("Tài khoản không được để trống")]
        [NotDuplicate("Tài khoản đã được đăng ký bởi người khác.")]
        public string UserName { get; set; }

        [Required("Họ và tên không được phép để trống")]
        public string FullName { get; set; }

        [Required("Email không được để trống")]
        [EmailValid]
        //[NotDuplicate("Email đã được đăng ký bởi người khác.")]
        public string Email { get; set; }

        [Required("Số điện thoại không được để trống")]
        //[NotDuplicate("Số điện thoại đã được đăng ký bởi người khác.")]
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }

        [Required("Mật khẩu không được để trống")]
        public string Password { get; set; }
        public string RePassword { get; set; }

        /// <summary>
        /// Admin đăng ký
        /// </summary>
        public bool IsAdminRegister { get; set; }

        // Thông tin đơn vị 
        public Organization Organization { get; set; }
    }
}
