using MS.ApplicationCore.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Entities
{
    public class MSLicense :BaseEntity
    {
        public Guid MSLicenseId { get; set; }
        public string MSLicenseCode { get; set; }
        public Guid? OrganizationId { get; set; }
        public LicenseType LicenseType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }
}
