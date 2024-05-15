using AutoMapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Exceptions;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.Interfaces.Repositories;
using MS.ApplicationCore.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Services
{
    public class UnitService : BaseService<Unit>, IUnitService
    {
        public UnitService(IUnitRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }

        public override async Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo> columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null)
        {
            var colmnsSortString = new string[] { "UnitName ASC" };
            var columnsFilterString = new string[] { "UnitName" };
            var data = await UnitOfWork.Units.GetFilterPaging(colmnsSortString, columnsFilterString, keyFilter, limit, offset);
            return data;
        }

        protected async override Task ValidateObjectCustom(Unit entity)
        {
            var unit = await UnitOfWork.Units.GetByIdAsync(entity.UnitId);
            if (entity.EntityState == MSEnums.EntityState.UPDATE && unit.IsSystem == true)
            {
                throw new MISAException(System.Net.HttpStatusCode.Forbidden, "Bạn không thể chỉnh sửa dữ liệu mặc định của hệ thống");
            }
        }

        public async override Task<int> RemoveAsync(object key)
        {
            // Lấy thông tin đơn vị:
            var unit = await UnitOfWork.Units.GetByIdAsync(key);
            if (unit.IsSystem == true)
            {
                throw new MISAException(System.Net.HttpStatusCode.Forbidden, "Bạn không thể xóa dữ liệu mặc định của hệ thống");
            }
            return await base.RemoveAsync(key);
        }
    }
}
