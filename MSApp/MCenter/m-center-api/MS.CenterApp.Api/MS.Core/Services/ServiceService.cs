using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using MS.Core.Core;
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
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class ServiceService : BaseService<MS.Core.Entities.Service>, IServiceService
    {
        readonly IFileTransfer _fileTransfer;
        IConfiguration _config;
        private readonly IHubContext<NotificationHub> _notificationHub;
        public ServiceService(IRepository<Service> repository, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config, IFileTransfer fileTransfer, IHubContext<NotificationHub> notificationHub) : base(repository, unitOfWork, mapper)
        {
            _config = config;
            _fileTransfer = fileTransfer;
            _notificationHub = notificationHub;
        }

        public async Task<int> AddNewService(Service service, IFormFile imgFile)
        {
            // Thực hiện validate:
            await Validate<Service>(service);

            if (service.UnitPrice < 0)
            {
                AddErrors("UnitPrice", "Giá vốn dịch vụ không thể là số âm.");
            }

            if (service.CostPrice < 0)
            {
                AddErrors("UnitPrice", "Giá dịch vụ không thể là số âm.");
            }

            if (Errors.Any())
            {
                throw new MSException(HttpStatusCode.BadRequest, Errors);
            }
            else
            {
                var newServiceId = Guid.NewGuid();
                service.ServiceId = newServiceId;
                // Thực hiện thêm mới Avatar:
                var imgFileName = newServiceId.ToString().Replace("-", "");
                if (imgFile != null)
                {
                    service.ImgPath = GetFilePath(imgFile, imgFileName);
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
                                        var imgStream = new FormFile(ms2, 0, ms2.ToArray().Length, "services", "thread-groups.png");
                                        service.ImgPath = await _fileTransfer.UploadFileAsync(imgStream, "services", imgFileName);
                                    }
                                }
                            }
                            else
                            {
                                _fileTransfer.UploadFile(imgFile, "services", imgFileName);
                            }
                            //AddErrors("ImgPath", $"Ảnh sản phẩm không được có dung lượng lớn hơn {imgMaxSize}KB");
                        }
                    }
                    else
                    {
                        _fileTransfer.UploadFile(imgFile, "services", imgFileName);
                    }
                    Console.WriteLine("Service Img path: ", service.ImgPath);
                }

                UnitOfWork.BeginTransaction();
                // Thêm mới thông tin hàng hóa:
                var res = await UnitOfWork.Services.AddAsync(service);
                UnitOfWork.Commit();
                // Yêu cầu cập nhật lại danh sách sản phẩm phía Client:
                if (res > 0)
                {
                    await _notificationHub.Clients.All.SendAsync("RefreshServices");
                }
                return res;
            }
        }

        public override async Task<int> UpdateAsync(Service entity, object pks, IFormCollection? formData = null)
        {
            if (entity != null)
            {
                return await base.UpdateAsync(entity, pks, formData);
            }
            else
            {
                if (formData != null)
                {
                    var service = new Service();
                    service = JsonConvert.DeserializeObject<Service>(formData["service"].First());
                    var imgFile = formData.Files?.FirstOrDefault();

                    // Cập nhật ảnh mới:
                    if (imgFile != null)
                    {
                        var imgFileName = service.ServiceId.ToString().Replace("-", "");
                        service.ImgPath = GetFilePath(imgFile, imgFileName);
                        _fileTransfer.UploadFile(imgFile, "services", imgFileName);
                        Console.WriteLine("Updated Img path: ", service.ImgPath);
                    }

                    UnitOfWork.BeginTransaction();
                    // Thêm mới thông tin hàng hóa:
                    var res = await UnitOfWork.Services.UpdateAsync(service);
                    UnitOfWork.Commit();

                    // Yêu cầu cập nhật lại danh sách sản phẩm phía Client:
                    if (res > 0)
                    {
                        await _notificationHub.Clients.All.SendAsync("RefreshServices");
                    }
                    return res;
                }
                else
                {
                    AddErrors("Service", "Dữ liệu đầu vào bị null");
                    throw new MSException(HttpStatusCode.BadRequest, Errors);
                }
            }

        }

        private string GetFilePath(IFormFile file, string fileName)
        {
            var fileNameSplit = file.FileName.Split(".");
            var extFile = fileNameSplit[fileNameSplit.Length - 1];
            return $"/services/{fileName}.{extFile}";
        }
    }
}
