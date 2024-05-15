using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Entities
{
    public class ServiceGroup:BaseEntity
    {
        public Guid ServiceGroupId { get; set; }
        public string ServiceGroupName { get; set; }
        public string? Description { get; set; }

    }
}
