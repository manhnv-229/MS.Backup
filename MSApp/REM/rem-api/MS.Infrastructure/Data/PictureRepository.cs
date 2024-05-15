using AutoMapper;
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
        public PictureRepository(IMSDatabaseContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
        public async override Task<int> AddAsync(dynamic entity, bool addChild = false)
        {
            var pic = entity as Picture;
            var parameters = new DynamicParameters();
            parameters.Add("@PictureId", pic.PictureId.ToString());
            parameters.Add("@Description", pic.Description);
            parameters.Add("@UrlPath", pic.UrlPath);
            parameters.Add("@AlbumId", pic.AlbumId.ToString());
            var rowAffects = await DbContext.AppConnection.ExecuteAsync($"Proc_Picture_Insert", param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return rowAffects;
        }

        public async Task<int> UpdateTotalViewPicture(Guid pictureId)
        {
            var sql = "UPDATE Picture p SET TotalViews = IFNULL(TotalViews,0)+1 WHERE PictureId = @PictureId";
            var parameters = new DynamicParameters();
            parameters.Add("@PictureId", pictureId);
            var res = await DbContext.AppConnection.ExecuteAsync(sql, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.Text);
            return res;
        }
    }
}
