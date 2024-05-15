using MS.Core.Entities;
using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class RefResponse:BaseEntity
    {
        public Guid RefId { get; set; }
        public string RefNo { get; set; }
        public DateTime RefDate { get; set; }
        public RefType RefType { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public DateTime? CreateInvoiceDate { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public Guid? AccountObjectId { get; set; }
        public string? AccountObjectName { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhoneNumber { get; set; }
        public string? JournalMemo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public Guid? StockId { get; set; }
        public virtual Organization? Organization { get; set; }
        public virtual IEnumerable<object> RefDetail { get; set; }
        public virtual IEnumerable<object> DebitDetail { get; set; }
        public virtual IEnumerable<object> InvoicePayment { get; set; }
        public virtual IEnumerable<object> StockInventory { get; set; }
        public virtual IEnumerable<object> ServiceInvoice { get; set; }
    }
}
