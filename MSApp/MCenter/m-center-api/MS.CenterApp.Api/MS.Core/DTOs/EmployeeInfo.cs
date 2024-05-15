using Microsoft.AspNetCore.Http;
using MS.Core.Entities;
using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    [DataTableName("Employee")]
    public class EmployeeInfo:Employee
    {
        public UserRequest? User { get; set; }
        public IFormFile? AvatarFile { get; set; }
        public Guid? RoleId { get; set; }
    }
}
