using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using MS.Core.Core;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Exceptions;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Services;
using MS.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class InventoryItemService : BaseService<InventoryItem>, IInventoryItemService
    {
        readonly IFileTransfer _fileTransfer;
        IConfiguration _config;
        private readonly IHubContext<NotificationHub> _notificationHub;
        public InventoryItemService(IInventoryItemRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IFileTransfer fileTransfer, IConfiguration config, IHubContext<NotificationHub> notificationHub) : base(repository, unitOfWork, mapper)
        {
            _fileTransfer = fileTransfer;
            _config = config;
            _notificationHub = notificationHub;
        }

        public override async Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo> columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null)
        {
            var colmnsSortString = new string[] { "CreatedDate DESC", "InventoryItemCode ASC" };
            var columnsFilterString = new string[] { "Barcode","InventoryItemCode", "InventoryItemName", "UnitPrice" };
            var data = await UnitOfWork.InventoryItems.GetFilterPaging(colmnsSortString, columnsFilterString, keyFilter, limit, offset);
            return data;
        }


        public async Task<int> AddNewService(InventoryItem inventoryItem, IFormFile imgFile)
        {
            // Thực hiện validate:
            await Validate<InventoryItem>(inventoryItem);

            if (inventoryItem.UnitPrice < 0)
            {
                AddErrors("UnitPrice", "Giá mua không thể là số âm.");
            }

            if (inventoryItem.CostPrice < 0)
            {
                AddErrors("UnitPrice", "Giá bán không thể là số âm.");
            }

            if (Errors.Any())
            {
                throw new MSException(HttpStatusCode.BadRequest, Errors);
            }
            else
            {
                var newInventoryItemId = Guid.NewGuid();
                inventoryItem.InventoryItemId = newInventoryItemId;
                // Thực hiện thêm mới Avatar:
                var imgFileName = newInventoryItemId.ToString().Replace("-", "");
                if (imgFile != null)
                {
                    inventoryItem.ImgPath = GetFilePath(imgFile, imgFileName);
                    var configMaxSize = _config["MaxSizeImg"];
                    // ảnh không được có dung lượng lớn hơn 3M:
                    if (_config != null && configMaxSize != null)
                    {
                        var imgMaxSize = int.Parse(configMaxSize);

                        // Ảnh có dung lượng quá lớn thì resize lại:
                        if (imgFile != null)
                        {
                            
                            if (imgFile.Length > imgMaxSize)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    imgFile.CopyTo(ms);
                                    Image image = Image.FromStream(ms, true);

                                    // Thay đổi kích thước ảnh:
                                    Image imgResize = ImgDraw.ResizeImage(image, new Size(300, 300));
                                    using (var ms2 = new MemoryStream())
                                    {
                                        imgResize.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);
                                        var imgStream = new FormFile(ms2, 0, ms2.ToArray().Length, "inventories", "thread-groups.png");
                                        inventoryItem.ImgPath = await _fileTransfer.UploadFileAsync(imgStream, "inventories", imgFileName);
                                    }
                                }
                            }
                            else
                            {
                                _fileTransfer.UploadFile(imgFile, "inventories", imgFileName);
                            }
                            //AddErrors("ImgPath", $"Ảnh sản phẩm không được có dung lượng lớn hơn {imgMaxSize}KB");
                        }
                    }
                    else
                    {
                         _fileTransfer.UploadFile(imgFile, "inventories", imgFileName);
                    }
                    Console.WriteLine("Inventory Img path: ", inventoryItem.ImgPath);
                }

                UnitOfWork.BeginTransaction();
                // Thêm mới thông tin hàng hóa:
                var res = await UnitOfWork.InventoryItems.AddAsync(inventoryItem);
                UnitOfWork.Commit();
                // Yêu cầu cập nhật lại danh sách sản phẩm phía Client:
                if (res > 0)
                {
                    await _notificationHub.Clients.All.SendAsync("RefreshInventoryItems");
                }
                return res;
            }
        }
        private string GetFilePath(IFormFile file, string fileName)
        {
            var fileNameSplit = file.FileName.Split(".");
            var extFile = fileNameSplit[fileNameSplit.Length - 1];
            return $"/inventories/{fileName}.{extFile}";
        }

        public override async Task<int> UpdateAsync(InventoryItem entity, object pks, IFormCollection? formData = null)
        {
            if (entity != null)
            {
                return await base.UpdateAsync(entity, pks, formData);
            }
            else
            {
                if (formData != null)
                {
                    var inventoryItem = new InventoryItem();
                    inventoryItem = JsonConvert.DeserializeObject<InventoryItem>(formData["inventory"].First());
                    var imgFile = formData.Files?.FirstOrDefault();

                    // Cập nhật ảnh mới:
                    if (imgFile != null)
                    {
                        var imgFileName = inventoryItem.InventoryItemId.ToString().Replace("-", "");
                        inventoryItem.ImgPath = GetFilePath(imgFile, imgFileName);
                        _fileTransfer.UploadFile(imgFile, "inventories", imgFileName);
                        Console.WriteLine("Updated Img path: ", inventoryItem.ImgPath);
                    }

                    UnitOfWork.BeginTransaction();
                    // Thêm mới thông tin hàng hóa:
                    var res = await UnitOfWork.InventoryItems.UpdateAsync(inventoryItem);
                    UnitOfWork.Commit();

                    // Yêu cầu cập nhật lại danh sách sản phẩm phía Client:
                    if (res > 0)
                    {
                        await _notificationHub.Clients.All.SendAsync("RefreshInventoryItems");
                    }
                    return res;
                }
                else
                {
                    AddErrors("InventoryItem", "Dữ liệu đầu vào bị null");
                    throw new MSException(HttpStatusCode.BadRequest, Errors);
                }
            }

        }

        public async Task<string> GetNewCodeAuto(string name)
        {
            var newName = name.Trim();
            
            // Cắt chuỗi:
            string[] arrayCode = newName.Split(' ');

            var preCode = "";
            // chỉ lấy ký tự đầu tiên:
            foreach (string code in arrayCode)
            {
                var codeAfter = CommonFunction.RemoveDiacritics(code);
                var firstChar = codeAfter.Substring(0, 1).Trim().ToUpper();
                preCode = $"{preCode}{firstChar}";
            }

            // Lấy mã mới trong hệ thống:
            var newCode = await UnitOfWork.InventoryItems.GetNewCodeAuto(preCode);
            
            return newCode;
        }
    }
}
