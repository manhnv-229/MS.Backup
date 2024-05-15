using MS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Interfaces
{
    public interface IEventRepository: IAsyncRepository<Event>, IRepository<Event>
    {
        Task<IEnumerable<Employee>> GetContactNotYetRegisterEventByEventId(int eventId);

        Task<int> DeleteEventDetailByEventIdAndUserId(int eventId, string userId);

    }
}
