using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Shared
{
    public class SharedData:IShareData
    {
        public Organization? Organization { get; set; }
        public Employee? Employee { get; set; }
        public UserInfo? User { get; set; }
        public string? ConnectionId {  get; set; }
    }
}
