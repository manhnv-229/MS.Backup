using MS.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IRoleRepository:IRepository<Role>
    {
        Task<Role> GetRoleByUserId(object userId);
        Task DeleteAllRoleByUserId(object userId);  
    }
}
