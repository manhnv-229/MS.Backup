using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class TenantUser:BaseEntity
    {
        [PrimaryKey]
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? EmailConfirmed { get; set; } = 0;
        public int? PhoneNumberConfirmed { get; set; } = 0;
        public int? AccessFailedCount { get; set; } = 0;
        public int TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public string? SecurityStamp { get; set; }
        public int LockoutEnabled { get; set; } = 0;

        public string? Password { get; set; }
        public string? RePassword { get; set; }

        public string? PasswordHash { get; set; }


        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
        public Guid? EmployeeId { get; set; }

        public Guid TenantId { get; set; }

        [NotMapQuery]
        public new Guid? OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        [NotMapQuery]
        public DateTime? LicenseExpiredDate { get; set; }
    }
}
