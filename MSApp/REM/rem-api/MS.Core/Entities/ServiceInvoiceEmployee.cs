using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class ServiceInvoiceEmployee:BaseEntity
    {
        public Guid? ServiceInvoiceEmployeeId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid ServiceInvoiceId { get; set; }
        public Guid? ServiceId { get; set; }
    }
}
