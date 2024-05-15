using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IPictureRepository: IRepository<Picture>
    {
        Task<int> UpdateTotalViewPicture(Guid albumId);
    }
}
