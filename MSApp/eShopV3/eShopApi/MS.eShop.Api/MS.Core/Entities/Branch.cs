using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Branch:BaseEntity
    {
        public Guid BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string? Tel { get; set; }
        public string? Email { get; set; }
        public string Address { get; set; }
        public bool Inactive { get; set; }
    }
}
