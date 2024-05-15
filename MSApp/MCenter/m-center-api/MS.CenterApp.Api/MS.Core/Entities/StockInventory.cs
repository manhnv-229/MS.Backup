using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class StockInventory:BaseEntity
    {
        public Guid StockInventoryId { get; set; }
        public Guid StockId { get; set; }
        public Guid? RefId { get; set; }
        public Guid InventoryItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public DateTime? DateOfManufacture { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Description { get; set; }
    }
}
