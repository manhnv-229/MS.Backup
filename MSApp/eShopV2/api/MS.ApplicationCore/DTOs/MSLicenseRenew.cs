using MS.ApplicationCore.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.DTOs
{
    public class MSLicenseRenew
    {
        public Guid OrganizationId { get; set; }
        public VIPOptions VIPOption { get; set; }
    }
}
