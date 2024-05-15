using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using MS.Core.Authorization;
using MS.Core.Core;
using MS.Core.DTOs;
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
    public class SlotService : BaseService<Slot>, ISlotService
    {
        ISlotRepository _repo;
        IMapper _mapper;
        IJwtUtils _tokenUtils;
        IFileTransfer _fileTransfer;
        ICommonFunction _commonFunc;
        private readonly IHubContext<NotificationHub> _notificationHub;
        IConfiguration _config;
        public SlotService(ISlotRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IJwtUtils tokenUtils, IFileTransfer fileTransfer, ICommonFunction commonFunc, IConfiguration config, IHubContext<NotificationHub> notificationHub) : base(repository, unitOfWork, mapper)
        {
            _repo = repository;
            _mapper = mapper;
            _tokenUtils = tokenUtils;
            _fileTransfer = fileTransfer;
            _commonFunc = commonFunc;
            _config = config;
            _notificationHub = notificationHub;
        }

        protected async override Task AfterSave(Slot entity, bool saveSuccess = true)
        {
            if (saveSuccess)
            {
                // Lấy thông tin SlotDto mới:
                var slotId = entity.SlotId;
                var slotDto = await UnitOfWork.Slots.GetSlotDtoAsync(slotId);
                // Thông báo và cập nhật cho các Clients:
                var connections = NotificationHub._connections;
                if (CommonFunction.OrganizationId != null)
                {
                    var connectionsByOrg = connections.GetConnectionsByOrgId(CommonFunction.OrganizationId);
                    foreach (var connectionId in connectionsByOrg)
                    {
                        await _notificationHub.Clients.Client(connectionId).SendAsync($"ProcessAfterAddNewSlotSuccess", slotDto);
                    }
                }
            }
        }
    }
}
