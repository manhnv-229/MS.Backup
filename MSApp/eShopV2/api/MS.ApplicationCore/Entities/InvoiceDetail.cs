using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Entities
{
    public class InvoiceDetail:BaseEntity
    {
        public Guid InvoiceDetailId { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        public decimal Quantity { get; set; }
        public string? Note { get; set; }
        public Guid? EmployeeId { get; set; }
        public decimal TotalMoney { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Invoice Invoice { get; set; }
        public virtual Product Product { get; set; }
    }
}
