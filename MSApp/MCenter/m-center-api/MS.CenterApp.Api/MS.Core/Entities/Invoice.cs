using MS.Core.Helpers;
using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Invoice:BaseEntity
    {
        public Invoice()
        {
            InvoiceDetail = new HashSet<InvoiceDetail>();
        }
        public Guid InvoiceId { get; set; }

        [NotDuplicate("Mã hóa đơn đã tồn tại trong hệ thống.")]
        public string InvoiceCode { get; set; }

        public DateTime InvoiceDate { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public decimal TotalMoney { get; set; }

        [Required("Nhân viên lập hóa đơn không được phép trống")]
        public Guid? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? Description { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetail { get; set; }
    }
}
