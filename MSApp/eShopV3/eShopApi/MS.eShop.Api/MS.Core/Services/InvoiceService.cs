using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using MS.Core.Core;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class InvoiceService : BaseService<Invoice>,IInvoiceService
    {
        IEmailService _emailService;
        ICommonFunction _commonFunction;
        private readonly IHubContext<NotificationHub> _notificationHub;
        public InvoiceService(IInvoiceRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IHubContext<NotificationHub> notificationHub, ICommonFunction commonFunction) : base(repository, unitOfWork, mapper)
        {
            _emailService = emailService;
            _notificationHub = notificationHub;
            _commonFunction = commonFunction;
        }

        protected async override Task ValidateObjectCustom(Invoice entity)
        {
            var products = entity.InvoiceDetail;
            if (products == null || products.Count==0)
            {
                AddErrors("InvoiceDetail", "Hóa đơn cần có ít nhất 1 sản phẩm/dịch vụ.");
            }
        }
        public override async Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo> columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null)
        {
            var colmnsSortString = new string[] { "InvoiceDate DESC", "CreatedDate DESC" };
            var columnsFilterString = new string[] { "InvoiceCode" };
            var data = await UnitOfWork.Invoices.GetFilterPaging(colmnsSortString, columnsFilterString, keyFilter, limit, offset);
            return data;
        }

        protected async override Task AfterSave(Invoice entity)
        {
            // Gửi mail thông báo lập hóa đơn thành công cho chủ doanh nghiệp:
            // Lấy Email chủ cửa hàng:
            var org = await UnitOfWork.Organizations.GetByIdAsync(CommonFunction.OrganizationId);
            var moneyString = string.Format("{0:n0}", entity.TotalMoney );
            var emailContent = $"Hóa đơn <b>[{entity.InvoiceCode}]</b> đã được lập thành công.<br> Tổng giá trị thu về là: <b>{moneyString}</b> đ";
            var emailTitle = $"[{org.OrganizationName}] +{moneyString} đ. Mã hóa đơn [{entity.InvoiceCode}]";
            _emailService.SentEmail(org.Email,emailTitle,emailContent);

            // Thông báo và cập nhật cho các Clients:
            var userId = _commonFunction.GetCurrentUserId();
            var connections = NotificationHub._connections;
            var orgInfo = await UnitOfWork.Users.GetOrganizationInfoByUserID(userId);
            foreach (var connectionId in connections.GetConnections(userId))
            {
                await _notificationHub.Clients.Client(connectionId).SendAsync("UpdateOrganizationInfo", orgInfo);
                //await _notificationHub.Clients.All.SendAsync("RecieveRemoveOrganizationInfo");
            }
        }
    }
}
