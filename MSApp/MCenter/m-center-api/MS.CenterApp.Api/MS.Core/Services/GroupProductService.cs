using AutoMapper;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class GroupProductService : BaseService<GroupProduct>, IGroupProductService
    {
        public GroupProductService(IGroupProductRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }

        public override async Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo> columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null)
        {
            var colmnsSortString = new string[] { "GroupProductName ASC" };
            var columnsFilterString = new string[] { "GroupProductName" };
            var data = await UnitOfWork.GroupProducts.GetFilterPaging(colmnsSortString, columnsFilterString, keyFilter, limit, offset);
            return data;
        }
    }
}
