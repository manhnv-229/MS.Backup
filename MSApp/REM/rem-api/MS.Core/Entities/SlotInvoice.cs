using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class SlotInvoice:BaseEntity
    {
        public Guid SlotInvoiceId { get; set; }
        public Guid RefId { get; set; }
        public Guid SlotId { get; set; }
        public Boolean BilledByHours { get; set; }
        public double PriceByHour { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public double? TotalAmount { get; set; }
        public string? Description { get; set; }
    }
}
