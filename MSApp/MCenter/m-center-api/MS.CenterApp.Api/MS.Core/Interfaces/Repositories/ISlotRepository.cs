using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Repositories
{
    public interface ISlotRepository : IRepository<Slot>
    {
        new Task<IEnumerable<object>> GetAllAsync();
        //Task<IEnumerable<object>> GetServiceInvoiceBySlotIdAsync(Guid slotId);
        //Task<IEnumerable<object>> GetOtherDetailBySlotIdAsync(Guid slotId);
        Task<SlotDto?> GetSlotDtoAsync(Guid? slotId);
    }
}
