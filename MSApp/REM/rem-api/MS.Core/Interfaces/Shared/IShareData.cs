using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Shared
{
    public interface IShareData
    {
        public Organization? Organization { get; set; }
        public Employee? Employee { get; set; }
        public UserInfo? User { get; set; }
    }
}
