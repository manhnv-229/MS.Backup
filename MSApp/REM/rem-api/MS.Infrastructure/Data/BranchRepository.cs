using AutoMapper;
using Microsoft.AspNetCore.Http;
using MS.Core.Entities;
using MS.Core.Interfaces.Repositories;
using MS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class BranchRepository : DapperRepository<Branch>, IBranchRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BranchRepository(IMSDatabaseContext dbContext, IHttpContextAccessor httpContextAccessor,AutoMapper.IMapper mapper) : base(dbContext,mapper)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async override Task<IEnumerable<Branch>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
    }
}
