using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using MS.Core.Core;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Caches;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.MSEnums;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class RefService : BaseService<Ref>, IRefService
    {
        IEmailService _emailService;
        ICommonFunction _commonFunction;
        IMemoryCache _msMemoryCache;
        private readonly IHubContext<NotificationHub> _notificationHub;
        public RefService(IRefRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IHubContext<NotificationHub> notificationHub, ICommonFunction commonFunction, IMemoryCache msMemoryCache) : base(repository, unitOfWork, mapper)
        {
            _emailService = emailService;
            _notificationHub = notificationHub;
            _commonFunction = commonFunction;
            _msMemoryCache = msMemoryCache;
        }

        protected async override Task ValidateObjectCustom(dynamic entity)
        {
            var refEntity = entity as Ref;
            var refType = refEntity.RefType;
            if (refType == RefType.IN_WAREHOUSE)
            {
                var stockInventories = refEntity.StockInventory;
                if (stockInventories == null || stockInventories.Count == 0)
                {
                    AddErrors("RefDetail", "Cần khai báo ít nhất 1 mặt hàng cần nhập kho.");
                }
            }
            else
            {
                var inventoryItems = refEntity.RefDetail;
                var services = refEntity.ServiceInvoices;
                var totalItem = inventoryItems?.Count() ?? 0 + services?.Count() ?? 0;
                if (totalItem == 0)
                {
                    AddErrors("RefDetail", "Hóa đơn cần có ít nhất 1 hàng hóa hoặc dịch vụ.");
                }
            }

            if (refEntity.ChangeAmount < 0)
            {
                AddErrors("ChangeAmount", "Số tiền thanh toán chưa đủ. Nếu khách hàng không đủ tiền thanh toán, vui lòng tích chọn [Ghi nợ].");
            }
        }
        public override async Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo> columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null)
        {
            var colmnsSortString = new string[] { "RefDate DESC", "CreatedDate DESC" };
            var columnsFilterString = new string[] { "RefNo" };
            var data = await UnitOfWork.Refs.GetFilterPaging(colmnsSortString, columnsFilterString, keyFilter, limit, offset);
            return data;
        }

        //protected async override Task BeforeSave(Ref entity)
        //{
        //    // Tạo mã mới:
        //    var newCode = await UnitOfWork.Refs.GetNewCodeDynamic();
        //    entity.RefNo = newCode;
        //}

        protected async override Task BeforeSave(dynamic entity)
        {
            if (entity.DebitDetail != null)
            {
                // Lọc các khoản nợ có số tiền nợ = 0
                var debitDetails = new List<DebitDetail>();
                foreach (var item in entity.DebitDetail)
                {
                    if (entity.CustomerId != null && item.Amount != 0)
                    {
                        item.CustomerId = entity.CustomerId ?? Guid.Empty;
                        debitDetails.Add(item);
                    }
                }
                //Gán lại các khoản nợ:
                entity.DebitDetail = debitDetails;
            }

            // Xử lý thông tin ngày hết hạn đối với phiếu nhập kho:
            if (entity.StockInventory != null && entity.StockInventory.Count > 0)
            {
                foreach (var item in entity.StockInventory)
                {
                    item.ExpiryDate = CommonFunction.ConvertDateToVNTime(item.ExpiryDate);
                    item.DateOfManufacture = CommonFunction.ConvertDateToVNTime(item.DateOfManufacture);
                }
            }

        }

        protected async override Task AfterSave(dynamic entity, bool saveSuccess = true)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //var settings = (List<Setting>)_msMemoryCache.Get("Settings");

            if (_msMemoryCache.TryGetValue("Settings", out IEnumerable<Setting> settings))
            {

            }
            else
            {
                settings = await UnitOfWork.Settings.GetAllAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    //.SetSlidingExpiration(TimeSpan.FromSeconds(60))
                    //.SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.NeverRemove);
                //.SetSize(1024);
                _msMemoryCache.Set("Settings", settings, cacheEntryOptions);
            }

            var sendEmail = settings?.FirstOrDefault<Setting>(st => st.SettingKey == "SentEmailAfterSaveRef")?.SettingValue;
            bool isSent = false;
            if (sendEmail != null && Boolean.TryParse(sendEmail.ToString(), out isSent))
            {
                if (isSent)
                {
                    // Gửi mail thông báo lập hóa đơn thành công cho chủ doanh nghiệp:
                    // Lấy Email chủ cửa hàng:
                    var org = await UnitOfWork.Organizations.GetByIdAsync(CommonFunction.OrganizationId);
                    var moneyString = string.Format("{0:n0}", entity.TotalAmount);
                    var emailContent = $"Hóa đơn <b>[{entity.RefNo}]</b> đã được lập thành công.<br> Tổng giá trị thu về là: <b>{moneyString}</b> đ";
                    var emailTitle = $"[{org.OrganizationName}] +{moneyString} đ. Mã hóa đơn [{entity.RefNo}]";

                    if (entity.RefType == RefType.IN_WAREHOUSE)
                    {
                        emailTitle = $"[{org.OrganizationName}] nhập kho {moneyString} đ. Mã phiếu nhập [{entity.RefNo}]";
                        emailContent = $"Phiếu nhập <b>[{entity.RefNo}]</b> đã được lập thành công.<br> Tổng giá trị nhập kho: <b>{moneyString}</b> đ";
                    }
                    _emailService.SentEmail(org.Email, emailTitle, emailContent);

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
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine($"RunTime (SendEmail: {sendEmail}): " + elapsedTime);
        }

        public async Task<PagingData> GetRefsByRefTypePaging(RefType? type, int limit, int offset, string? key)
        {
            var data = await UnitOfWork.Refs.GetRefsByRefTypePaging(type, limit, offset, key);
            return data;
        }
    }
}
