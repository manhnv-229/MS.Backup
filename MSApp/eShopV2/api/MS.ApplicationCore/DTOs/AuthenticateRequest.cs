using MS.ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.ApplicationCore.Entities
{
    public class AuthenticateRequest
    {
        [Required("Tài khoản không được phép để trống.")]
        public string Username { get; set; }

        [Required("Mật khẩu không được phép để trống")]
        public string Password { get; set; }
    }
}
