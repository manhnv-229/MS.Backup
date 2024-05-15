using MS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.DTOs
{
    public class OrganizationInfo:Organization
    {
        public int? TotalInvoices { get; set; }
        public decimal? TotalMoneys { get; set; }
        public Organization Organization { get; set; }
    }
}
