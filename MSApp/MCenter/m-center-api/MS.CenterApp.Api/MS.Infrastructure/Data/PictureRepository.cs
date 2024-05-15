using Dapper;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Infrastructure.Interfaces;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class PictureRepository : DapperRepository<Picture>, IPictureRepository
    {
        public PictureRepository(IMSDatabaseContext mySqlDbContext) : base(mySqlDbContext)
        {
        }
        public async override Task<int> AddAsync(Picture entity, bool addChild = false)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PictureId", entity.PictureId.ToString());
            parameters.Add("@Description", entity.Description);
            parameters.Add("@UrlPath", entity.UrlPath);
            parameters.Add("@AlbumId", entity.AlbumId.ToString());
            var rowAffects = await DbContext.Connection.ExecuteAsync($"Proc_Picture_Insert", param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return rowAffects;
        }

        public async Task<int> UpdateTotalViewPicture(Guid pictureId)
        {
            var sql = "UPDATE Picture p SET TotalViews = IFNULL(TotalViews,0)+1 WHERE PictureId = @PictureId";
            var parameters = new DynamicParameters();
            parameters.Add("@PictureId", pictureId);
            var res = await DbContext.Connection.ExecuteAsync(sql, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.Text);
            return res;
        }
    }
}
