using MS.ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Entities
{
    [HasSystemData]
    public class EmployeePosition:BaseEntity
    {
        public Guid PositionId { get; set; }
        public string PositionName { get; set; }
        public string? Description { get; set; }
        public bool? IsSystem { get; set; }
    }
}
