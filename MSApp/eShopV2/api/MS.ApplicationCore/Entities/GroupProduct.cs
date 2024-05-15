using MS.ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Entities
{
    [HasSystemData]
    public class GroupProduct:BaseEntity
    {
        public Guid GroupProductId { get; set; }
        public string GroupProductName { get; set; }
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        public bool? IsSystem { get; set; }
    }
}
