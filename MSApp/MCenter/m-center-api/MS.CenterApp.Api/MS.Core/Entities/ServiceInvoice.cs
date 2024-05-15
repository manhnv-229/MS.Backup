using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class ServiceInvoice:BaseEntity
    {
        public Guid ServiceInvoiceId { get; set; }
        public Guid RefId { get; set; }
        public Guid ServiceId { get; set; }
        public ChargeType ChargeType { get; set; }
        public decimal UnitTime { get; set; }
        public decimal CostPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public decimal? TotalTime { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Description { get; set; }
    }
}
