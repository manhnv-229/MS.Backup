using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    [DataTableName("RefDetail")]
    public class RefDetail : BaseEntity
    {
        public Guid RefDetailId { get; set; }
        public Guid RefId { get; set; }
        public int RefDetailType { get; set; }
        public Guid InventoryItemId { get; set; }
        public decimal Quantity { get; set; }
        public Guid UnitId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CostPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountRate { get; set; }
    }
}
