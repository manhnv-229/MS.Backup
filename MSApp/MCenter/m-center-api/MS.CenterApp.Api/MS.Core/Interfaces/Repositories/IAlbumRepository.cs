using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IAlbumRepository:IRepository<Album>
    {
        Task<IEnumerable<Picture>> GetPicturesByAlbumId(Guid albumId);
        Task<int> UpdateTotalViewAlbum(Guid albumId);
    }
}
