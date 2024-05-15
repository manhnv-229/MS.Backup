using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    [ViewDataName("Views_SlotGroup")]
    public class SlotGroup:BaseEntity
    {
        public Guid SlotGroupId { get; set; }
        public string? SlotGroupName { get; set; }
        public Guid? BranchId { get; set; }
        public string? Description { get; set; }
        public ICollection<Slot>? Slots { get; set;}
        public Boolean BilledByHours { get; set; }
        public decimal PriceByHour { get; set; }
    }
}
