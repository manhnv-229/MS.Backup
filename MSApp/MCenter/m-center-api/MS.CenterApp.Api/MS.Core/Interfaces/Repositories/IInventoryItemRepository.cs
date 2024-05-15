using MS.Core.DTOs;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IInventoryItemRepository : IRepository<InventoryItem>
    {
        Task<IEnumerable<InventoryItemResponse>> GetInventoryItemAdminResponses();
        Task<IEnumerable<InventoryItemSaleResponse>> GetInventoryItemSaleResponses();
        Task<IEnumerable<InventoryItemSaleResponse>> FilterInventoryItemSaleResponses(string key);
        Task<string> GetNewCodeAuto(string prefixCode);
    }
}
