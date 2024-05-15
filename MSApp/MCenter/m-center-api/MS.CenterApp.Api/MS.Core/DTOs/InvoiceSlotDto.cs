using MS.Core.Entities;
using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class InvoiceSlotDto:Ref
    {
        public InvoiceSlotDto()
        {
            Slots = new HashSet<SlotDto>();
        }
        /// <summary>
        /// Action Type để xác định hành động với các đối tượng liên quan:
        /// </summary>
        public InvoiceSlotActionType InvoiceSlotActionType { get;set;}

        /// <summary>
        /// Danh sách các slots (nếu ghép)
        /// </summary>
        public ICollection<SlotDto> Slots { get; set; }

        public new ICollection<RefDetailResponse>? RefDetail { get ; set; }
        public new ICollection<ServiceInvoiceDto>? ServiceInvoices { get; set; }
        public new ICollection<SlotInvoiceDto>? SlotInvoices { get; set; }

    }
}
