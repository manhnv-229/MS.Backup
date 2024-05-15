using MS.Core.Entities;
using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    [DataTableName("ServiceInvoice")]
    public class ServiceInvoiceDto:ServiceInvoice
    {
        public string? ServiceName { get; set; }
    }
}
