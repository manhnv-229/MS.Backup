using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class DebitDetail:BaseEntity
    {
        public Guid DebitDetailId { get; set; }
        public decimal Amount { get; set; }
        public Guid CustomerId { get; set; }
        public Guid RefId { get; set; }
        public DebitType DebitType { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}
