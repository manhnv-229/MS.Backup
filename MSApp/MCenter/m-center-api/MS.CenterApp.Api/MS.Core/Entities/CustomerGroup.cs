using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class CustomerGroup:BaseEntity
    {
        public Guid CustomerGroupId { get; set; }

        [Required("Tên nhóm khách hàng không được phép để trống.")]
        public string CustomerGroupName { get; set; }
        public string? Description { get; set; }
    }
}
