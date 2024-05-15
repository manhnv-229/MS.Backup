using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class StatisticInvoiceByEmployee
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; }
        public Guid InventoryItemCategoryId { get; set; }
        public string InventoryItemCategoryName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
