using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Report:BaseEntity
    {
        public Guid ReportId { get; set; }
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public ReportType? ReportType { get; set; } 
        public string? Description { get; set; }
    }
}
