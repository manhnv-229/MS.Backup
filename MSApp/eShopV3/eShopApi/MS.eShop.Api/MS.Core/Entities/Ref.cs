using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Ref : BaseEntity
    {
        public Ref()
        {
            StockInventory = new HashSet<StockInventory>();
            RefDetail = new HashSet<RefDetail>();
            DebitDetail = new HashSet<DebitDetail>();
            InvoicePayment = new HashSet<InvoicePayment>();
        }
        public Guid RefId { get; set; }
        public string RefNo { get; set; }
        public DateTime RefDate { get; set; }
        public RefType RefType { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? EmployeeId { get; set; }
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
        public decimal ActualReceiveAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public Guid? StockId { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<RefDetail>? RefDetail { get; set; }
        public virtual ICollection<DebitDetail>? DebitDetail { get; set; }
        public virtual ICollection<InvoicePayment> InvoicePayment { get; set; }
        public virtual ICollection<StockInventory>? StockInventory { get; set; }
    }
}
