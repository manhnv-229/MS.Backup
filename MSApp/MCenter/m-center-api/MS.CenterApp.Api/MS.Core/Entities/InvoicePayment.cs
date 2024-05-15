using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class InvoicePayment : BaseEntity
    {
        public Guid InvoicePaymentId { get; set; }
        public PaymentType PaymentType { get; set; }
        public Guid RefId { get; set; }
        public decimal Amount { get; set; }
        public Guid? BankAccountId { get; set; }
        public string? BankAccountNumber { get; set; }
    }
}
