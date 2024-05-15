using MS.Core.MSEnums;
using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities.Auth
{
    public class UserRole:BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        [NotMapQuery]
        public MSEnums.MSRole? RoleValue { get; set; }
    }
}
