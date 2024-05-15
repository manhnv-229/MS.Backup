using AutoMapper;
using Microsoft.AspNetCore.Http;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class BrachService : BaseService<Branch>, IBranchService
    {
        IBranchRepository _repository;
        IUnitOfWork _unitOfWork;
        public BrachService(IBranchRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async override Task<int> AddAsync(Branch entity, IFormCollection? formData = null)
        {
            entity.BranchId = Guid.NewGuid();
            // Thêm nhóm phòng/ chỗ mặc định:
            var slotGroup = new SlotGroup()
            {
                SlotGroupId = Guid.NewGuid(),
                BranchId = entity.BranchId,
                BilledByHours = false,
                EntityState = MSEnums.MSEntityState.ADD,
                SlotGroupName = "Nhóm mặc định"
            };
            entity.SlotGroups.Add(slotGroup);
            var res = await _unitOfWork.Branches.AddAsync(entity,true);
            return res;
        }
    }
}
