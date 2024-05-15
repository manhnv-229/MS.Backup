using MS.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IUserRolesRepository:IRepository<UserRole>
    {
        public Task<Guid> GetUserRoleIdByRoleValue(int roleValue);
    }
}
