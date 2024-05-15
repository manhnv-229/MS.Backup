using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.MSEnums;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        //private readonly IList<MSEnums.MSRole> _roles;
        private readonly MSEnums.MSRole _role;
        private readonly IList<Role>? _roles;

        public AuthorizeAttribute()
        {

        }
        public AuthorizeAttribute(params Role[] roles)
        {
            _roles = roles ?? new Role[] { };
        }
        public AuthorizeAttribute(MSEnums.MSRole role)
        {
            _role = role;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            // Bỏ qua việc xác thực thông tin ủy quyền nếu action được gán thuộc tính [AllowAnonymous]
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;
            if (httpContext != null)
            {

            }
            var user = (UserInfo)httpContext.Items["User"];
            var path = httpContext.Request.Path.Value;
            var licensesPath = new string[] { "/api/v1/invoices" };
            var method = httpContext.Request.Method;
            //var roles = (_roles.Any() && !_roles.Contains(user.Role));
            if (user == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                var hasNotPermission = (user.HighestRole == null || (_role != 0 && user.HighestRole > _role));
                // Chỉ quản lý hoặc người dùng thông thường được phép sửa thông tin liên hệ:
                if (hasNotPermission)
                {
                    context.Result = new JsonResult(new { message = "Bạn không có quyền thực hiện chức năng hoặc truy cập tài nguyên hiện tại." }) { StatusCode = StatusCodes.Status403Forbidden };
                }
            }

            // Kiểm tra License:
            var expiredDate = CommonFunction.ConvertDateToVNTime(user?.LicenseExpiredDate);
            var nowDate = CommonFunction.ConvertDateToVNTime(DateTime.Now);
            var patMatch = false;
            if (path != null)
            {
                foreach (var item in licensesPath)
                {

                    patMatch = path.Contains(item);
                    if (patMatch)
                    {
                        break;
                    }

                }
            }
           
            if ((expiredDate == null || expiredDate < nowDate) && (patMatch == true) && user?.HighestRole != MSRole.Administrator)
            {
                context.Result = new JsonResult(new { message = "Bạn chưa đăng ký thành viên VIP hoặc thời hạn VIP đã hết hạn. Vui lòng đăng ký thành viên VIP để tiếp tục sử dụng phân hệ này.\n Liên hệ: <b>0961.179.969</b>" }) { StatusCode = StatusCodes.Status403Forbidden };
            }

        }
    }
}
