using MS.Core.Entities;
using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    [DataTableName("SlotInvoice")]
    public class SlotInvoiceDto:SlotInvoice
    {
        public string? SlotCode { get; set; }
        public string? SlotName { get; set; }
    }
}
