using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MS.Core.Core;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Exceptions;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using MS.Core.Interfaces.Shared;
using MS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MS.Core.Services
{
    public class OrderService : IOrderService
    {
        ICommonFunction _commonFunction;
        IUnitOfWork _uow;
        private readonly IHubContext<NotificationHub> _notificationHub;
        IRefRepository _refRepository;
        IMapper _mapper;
        IShareData _shareData;

        public OrderService(IServiceProvider serviceProvider, ICommonFunction commonFunction, IUnitOfWork uow, IHubContext<NotificationHub> notificationHub, IRefRepository refRepository, IMapper mapper)
        {
            _commonFunction = commonFunction;
            _uow = uow;
            _notificationHub = notificationHub;
            _refRepository = refRepository;
            _mapper = mapper;
            _shareData = serviceProvider.GetService(typeof(IShareData)) as IShareData;
        }


        public async Task CreateSlotOrderAsync(InvoiceSlotDto invoiceSlot)
        {
            var slotId = invoiceSlot.SlotId;
            if (slotId == null)
            {
                throw new MSException(System.Net.HttpStatusCode.BadRequest, "Không có thông tin Bàn/Phòng, vui lòng liên hệ Mr Mạnh để được trợ giúp!");
            }
            var slotDto = invoiceSlot.Slots?.FirstOrDefault(x => x.SlotId == slotId);

            if (slotDto == null)
            {
                slotDto = await _uow.Slots.GetSlotDtoAsync(slotId);
                if (invoiceSlot.Slots == null)
                {
                    invoiceSlot.Slots = new HashSet<SlotDto>();
                }
                invoiceSlot.Slots.Add(slotDto);
            };

            // Cập nhật trạng thái Slot -> BUSY:
            slotDto.SlotStatus = MSEnums.SlotStatus.BUSY;

            //Cập nhật thời gian bắt đầu sử dụng cho Slot:
            slotDto.TimeStart = CommonFunction.ConvertDateToVNTime(DateTime.Now);


            _uow.BeginTransaction();


            // Cập nhật thành công thì thực hiện tiếp:
            // --> Thêm hóa đơn tính bàn theo giờ nếu slot có tính theo giờ (-> bảng SlotInvoice)

            invoiceSlot.RefId = Guid.NewGuid();
            invoiceSlot.RefDate = CommonFunction.ConvertDateToVNTimeNotNull(DateTime.Now);
            invoiceSlot.RefType = MSEnums.RefType.SALE;
            invoiceSlot.PaymentStatus = MSEnums.PaymentStatus.PENDING;
            invoiceSlot.EntityState = MSEnums.MSEntityState.ADD;
            invoiceSlot.BranchId = slotDto.BranchId;
            invoiceSlot.SlotId = slotDto.SlotId;

            var newRef = _mapper.Map<InvoiceSlotDto, Ref>(invoiceSlot);

            /// Lấy mã hoá đơn mới:
            invoiceSlot.RefNo = await _uow.Refs.GetNewRefNoAsync(newRef);
            newRef.RefNo = invoiceSlot.RefNo;

            // chi tiết cho hoá đơn:
            var slotInvoiceDto = new SlotInvoiceDto()
            {
                RefId = newRef.RefId,
                SlotId = slotDto.SlotId,
                BilledByHours = slotDto.BilledByHours,
                PriceByHour = slotDto.PriceByHour,
                TimeStart = CommonFunction.ConvertDateToVNTime(DateTime.Now),
            };
            var slotInvoice = _mapper.Map<SlotInvoiceDto, SlotInvoice>(slotInvoiceDto);
            var slotInvoicesDto = new HashSet<SlotInvoiceDto>();
            var slotInvoices = new HashSet<SlotInvoice>();
            slotInvoicesDto.Add(slotInvoiceDto);
            slotInvoices.Add(slotInvoice);
            newRef.SlotInvoices = slotInvoices;
            invoiceSlot.SlotInvoices = slotInvoicesDto;

            // Tạo mới hoá đơn cho Slot:
            var addRef = await _uow.Refs.AddAsync(newRef, true);
            var slot = _mapper.Map<SlotDto, Slot>(slotDto);
            if (addRef > 0)
            {
                // Update vào Database:
                slot.RefId = newRef.RefId;
                slot.OrdererName = newRef.CustomerName;
                slot.CustomerId = newRef.CustomerId;
                var res = await _uow.Slots.UpdateAsync(slot);
            }
            else
            {
                throw new MSException(System.Net.HttpStatusCode.InternalServerError, $"Không thể thực hiện sử dụng [{slot.SlotName}]. Vui lòng liên hệ quản trị viên để được trợ giúp");
            }
            // Thêm hóa đơn tính tiền dịch vụ nếu có chọn tính theo dịch vụ - VD nhân viên rót bia ( -> Bảng ServiceInvoice)
            if (addRef > 0)
            {
                _uow.Commit();
                // Thông báo và cập nhật cho các Clients:
                var connections = NotificationHub._connections;
                if (CommonFunction.OrganizationId != null)
                {
                    var connectionsByOrg = connections.GetConnectionsByOrgId(CommonFunction.OrganizationId);
                    foreach (var connectionId in connectionsByOrg)
                    {
                        await _notificationHub.Clients.Client(connectionId).SendAsync($"ProcessAfterCreateSlotOrderSuccess_{slotDto.SlotId.ToString()}", slotDto, invoiceSlot, _shareData.User);
                    }
                }

            }
            else
            {
                throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Không thể thực hiện cập nhật hóa đơn. Vui lòng liên hệ quản trị viên để được trợ giúp");
            }
        }

        public Task DeleteSlotOrder(Slot slot)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateSlotOrder(InvoiceSlotDto orderSlot)
        {
            var actionType = orderSlot.InvoiceSlotActionType;
            switch (actionType)
            {
                case MSEnums.InvoiceSlotActionType.ADD:
                    await ProcessAddNewInvoiceSlot(orderSlot);
                    break;
                case MSEnums.InvoiceSlotActionType.UPDATE:
                    await ProcessUpdateInvoiceSlot(orderSlot);
                    break;
                case MSEnums.InvoiceSlotActionType.VIEW:
                    await ProcessViewInvoiceSlot(orderSlot);
                    break;
                case MSEnums.InvoiceSlotActionType.DELETE:
                    await ProcessDeleteOrCancelOrder(orderSlot);
                    break;
                case MSEnums.InvoiceSlotActionType.MERGE:
                    await ProcessMergeSlots(orderSlot);
                    break;
                case MSEnums.InvoiceSlotActionType.UNMERGE:
                    await ProcessUnMergeSlots(orderSlot);
                    break;
                case MSEnums.InvoiceSlotActionType.CHANGE:
                    await ProcessChangeSlot(orderSlot);
                    break;
                case MSEnums.InvoiceSlotActionType.COMPLETE:
                    await ProcessCompleteInvoice(orderSlot);
                    break;
                default:
                    throw new MSException(System.Net.HttpStatusCode.BadRequest, "Vui lòng kiểm tra InvoiceActionType!");
            }

        }

        private async Task ProcessDeleteOrCancelOrder(InvoiceSlotDto orderSlot)
        {
            // Xoá toàn bộ hoá đơn liên quan:
            var refUpdate = _mapper.Map<InvoiceSlotDto, Ref>(orderSlot);
            var res = await _uow.Refs.RemoveAsync(refUpdate.RefId);

            // Cập nhật trạng thái slot về ban đầu:
            var slots = orderSlot.Slots;
            foreach (var slot in slots)
            {
                ResetSlotDefault(slot);
                _ = await _uow.Slots.UpdateAsync(slot);
                // Báo lại cho các client làm mới trạng thái:
                // Thông báo và cập nhật cho các Clients:
                var connections = NotificationHub._connections;
                if (CommonFunction.OrganizationId != null)
                {
                    var connectionsByOrg = connections.GetConnectionsByOrgId(CommonFunction.OrganizationId);
                    foreach (var connectionId in connectionsByOrg)
                    {
                        await _notificationHub.Clients.Client(connectionId).SendAsync($"ProcessAfterDeleteOrCancelOrderSuccess_{slot.SlotId}", slot, orderSlot, _shareData.User);
                    }
                }
            }
        }

        async Task ProcessViewInvoiceSlot(InvoiceSlotDto invoiceSlotDto)
        {
            throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Tính năng chưa hoạt động");
        }

        async Task ProcessUpdateInvoiceSlot(InvoiceSlotDto invoiceSlotDto)
        {
            // Hiện tại mọi việc cập nhật đều được thực hiện dưới Client: BE chỉ thực hiện lưu trữ vào Db:
            var refUpdate = _mapper.Map<InvoiceSlotDto, Ref>(invoiceSlotDto);
            var res = await _uow.Refs.UpdateAsync(refUpdate);
            if (res <= 0)
            {
                var msgError = $"Không thể thực hiện cập nhật Hoá đơn. Liên hệ Mr Mạnh để được xử lý";
                throw new MSException(System.Net.HttpStatusCode.InternalServerError, msgError);
            }
            else
            {
                // Duyệt từng Slot gửi thông tin về cho các client
                var slots = invoiceSlotDto.Slots;
                foreach (var slot in slots)
                {
                    // Báo lại cho các client làm mới trạng thái:
                    var connections = NotificationHub._connections;
                    if (CommonFunction.OrganizationId != null)
                    {
                        var connectionsByOrg = connections.GetConnectionsByOrgId(CommonFunction.OrganizationId);
                        foreach (var connectionId in connectionsByOrg)
                        {
                            await _notificationHub.Clients.Client(connectionId).SendAsync($"ProcessAfterUpdateOrderSuccess_{slot.SlotId}", slot, invoiceSlotDto, _shareData.User);
                        }
                    }
                }
            }
        }

        async Task ProcessAddNewInvoiceSlot(InvoiceSlotDto invoiceSlotDto)
        {
            await CreateSlotOrderAsync(invoiceSlotDto);
        }

        async Task ProcessChangeSlot(InvoiceSlotDto invoiceSlotDto)
        {
            throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Tính năng chuyển Slot chưa hoạt động");
        }

        async Task ProcessUnMergeSlots(InvoiceSlotDto invoiceSlotDto)
        {
            throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Tính năng chưa hoạt động");
        }

        async Task ProcessMergeSlots(InvoiceSlotDto invoiceSlotDto)
        {
            throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Tính năng chưa hoạt động");
        }


        private async Task ProcessCompleteInvoice(InvoiceSlotDto orderSlot)
        {
            // Cập nhật chi tiết hoá đơn:
            var refUpdate = _mapper.Map<InvoiceSlotDto, Ref>(orderSlot);
            _uow.BeginTransaction();
            var updateSuccess = (await _uow.Refs.UpdateAsync(refUpdate)) > 0;

            if (updateSuccess == true)
            {
                orderSlot.RefDetail = new List<RefDetailResponse>();
                orderSlot.ServiceInvoices = new List<ServiceInvoiceDto>();
                orderSlot.SlotInvoices = new List<SlotInvoiceDto>();
                // Thay đổi trạng thái các slots:
                // Các bàn về trạng thái ban đầu (VD ghép -> thường):
                if (orderSlot.Slots != null)
                {
                    foreach (var slot in orderSlot.Slots)
                    {
                        ResetSlotDefault(slot);
                        _ = await _uow.Slots.UpdateAsync(slot);
                        // Gói đến các client thực hiện cập nhật trạng thái cho các bàn:
                        var connections = NotificationHub._connections;
                        if (CommonFunction.OrganizationId != null)
                        {
                            var connectionsByOrg = connections.GetConnectionsByOrgId(CommonFunction.OrganizationId);
                            foreach (var connectionId in connectionsByOrg)
                            {
                                await _notificationHub.Clients.Client(connectionId).SendAsync($"ProcessAfterPayCompleteOrderSuccess_{slot.SlotId}", slot, orderSlot, _shareData.User);
                            }
                        }
                    }
                }

                _uow.Commit();
            }
            else
            {
                throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Không thể thực hiện thanh toán. Liên hệ Mr Mạnh để được trợ giúp!");
            }
        }

        private void ResetSlotDefault(Slot slotItem)
        {
            slotItem.SlotStatus = MSEnums.SlotStatus.FREE;
            slotItem.TimeStart = null;
            slotItem.TimeEnd = null;
            slotItem.CustomerId = null;
            slotItem.EmployeeId = null;
            slotItem.OrdererName = null;
            slotItem.PayerName = null;
            slotItem.TotalAmount = 0;
            slotItem.RefId = null;
        }

        /// <summary>
        /// Thực hiện tính toán chi phí theo thời gian cho từng slot theo giờ
        /// </summary>
        /// <param name="slotInvoices"></param>
        /// <exception cref="MSException"></exception>
        private void CalculatorSlotInvoices(ICollection<SlotInvoice> slotInvoices)
        {
            // Cập nhật chi phí sử dụng slot theo giờ:
            if (slotInvoices?.Count > 0)
            {
                foreach (var item in slotInvoices)
                {
                    // Nếu tính theo giờ:
                    if (item.BilledByHours == true)
                    {
                        // Thời gian bắt đầu:
                        var timeStart = CommonFunction.ConvertDateToVNTime(item.TimeStart);

                        // Cập nhật thời gian kết thúc:
                        var timeEnd = CommonFunction.ConvertDateToVNTime(DateTime.Now);

                        // Tổng thời gian sử dụng:
                        if (timeStart != null && timeStart < timeEnd)
                        {
                            var timeUsed = (timeEnd - timeStart);
                            //var hours = timeUsed.Value.TotalHours;
                            var minutes = timeUsed.Value.TotalMinutes;
                            var totalHours = minutes / 60;
                            item.TotalAmount = totalHours * item.PriceByHour;
                        }
                        else
                        {
                            throw new MSException(System.Net.HttpStatusCode.InternalServerError, "Có lỗi xảy ra khi thực hiện tính toán thời gian sử dụng cho các slots. Vui lòng liên hệ Quản trị viên để được hỗ trợ!");
                            //var slotItem = orderSlot.Slots.FirstOrDefault(i => i.SlotId == item.SlotId);
                            //errors.Add($"Vui lòng kiểm tra lại thời gian bắt đầu sử dụng [${slotItem?.SlotName}]");
                        }
                    }
                }
            }
        }


        private async Task ChangeSlot(InvoiceSlotDto orderSlot)
        {

        }
    }
}
