using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.DTOs
{
    public class OrganizationStatisticDto
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string Email { get; set; }
        public int? TotalInvoice { get; set; }
        public int? TotalMoney { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}
