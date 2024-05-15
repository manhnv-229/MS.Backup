using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class ReportProfit
    {
        public Guid InventoryItemId { get; set; }
        public string InventoryItemCode { get; set; }
        public string InventoryItemName { get; set; }
        public Guid InventoryItemCategoryId { get; set; }
        public string InventoryItemCategoryName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalCostPrice { get; set; }
        public decimal TotalUnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalProfitAmount { get; set; }
    }
}
