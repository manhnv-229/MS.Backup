using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class InvoiceResponse: Invoice
    {
        public override Organization Organization { get => base.Organization; set => base.Organization = value; }
        public override ICollection<InvoiceDetail> InvoiceDetail { get => base.InvoiceDetail; set => base.InvoiceDetail = value; }
    }
}
