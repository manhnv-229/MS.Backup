using MS.Core.DTOs;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Services
{
    public interface ITenantService:IBaseService<Tenant>
    {
        public Task RegisterAsync(TenantRegister tenant);
    }
}
