using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Exceptions;
using MS.Core.Helpers;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Caches;
using MS.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MS.Core.Services
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity
    {
        readonly IRepository<TEntity> _repository;
        protected Dictionary<string, List<string>> Errors = new();
        protected IUnitOfWork UnitOfWork;
        protected readonly IMapper Mapper;
        private PropertyInfo[] _properties;

        public BaseService(IRepository<TEntity> repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            //_repository = serviceProvider.GetService(typeof(IRepository<TEntity>)) as IRepository<TEntity>;
            //UnitOfWork = serviceProvider.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
            _repository = repository;
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            _properties = typeof(TEntity).GetProperties();
        }

        public virtual async Task<int> AddAsync(TEntity entity, IFormCollection? formData = null)
        {
            UnitOfWork.BeginTransaction();
            await ValidateObject(entity);
            entity.EntityState = MSEnums.MSEntityState.ADD;
            await ProcessEntityBeforeSave(entity);
            if (Errors.Count == 0)
            {
                var res = await _repository.AddAsync(entity, true);
                await AfterSave(entity);
                UnitOfWork.Commit();
                return res;
            }
            else
                throw new MSException(System.Net.HttpStatusCode.BadRequest, Errors);
        }

        public async Task<int> AddAsync(IEnumerable<TEntity> entities)
        {
            if (Errors.Count == 0)
                return await _repository.AddAsync(entities);
            else
                throw new MSException(System.Net.HttpStatusCode.BadRequest, Errors);
        }

        public virtual async Task<int> RemoveAsync(object key)
        {
            return await _repository.RemoveAsync(key);
        }

        public virtual async Task<int> UpdateAsync(TEntity entity, object pks, IFormCollection? formData = null)
        {
            UnitOfWork.BeginTransaction();
            entity.EntityState = MSEnums.MSEntityState.UPDATE;
            await ValidateObject(entity);
            await ProcessEntityBeforeSave(entity);
            if (Errors.Count == 0)
            {
                var res = await _repository.UpdateAsync(entity, pks);
                await AfterSave(entity, res > 0);
                UnitOfWork.Commit();
                return res;
            }
            else
                throw new MSException(System.Net.HttpStatusCode.BadRequest, Errors);
        }

        private async Task ProcessEntityBeforeSave(TEntity entity)
        {
            // Xử lý dữ liệu ngày tháng -> chuyển đúng múi giờ Việt Nam:
            var props = _properties.Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

            foreach (var prop in props)
            {
                var propName = prop.Name;
                var propValue = prop.GetValue(entity);
                var propType = prop.PropertyType;
                if (propValue != null)
                {
                    var timeConvert = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((DateTime)propValue, "SE Asia Standard Time");
                    prop.SetValue(entity, timeConvert);
                }
            }

            // Xử lý các thông tin bổ sung khác:
            var timeNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "SE Asia Standard Time");
            if (entity.EntityState == MSEnums.MSEntityState.ADD)
            {
                entity.CreatedDate = timeNow;
                entity.CreatedBy = CommonFunction.UserId;
            }
            else
            {
                entity.ModifiedDate = timeNow;
                entity.ModifiedBy = CommonFunction.UserId;
            }
            await BeforeSave(entity);
        }

        protected virtual async Task BeforeSave(TEntity entity)
        {
        }

        protected virtual async Task AfterSave(TEntity entity, bool saveSuccess = true)
        {
        }

        private async Task ValidateObject(TEntity entity)
        {
            await Validate<TEntity>(entity);
            await ValidateObjectCustom(entity);
        }

        protected virtual async Task ValidateObjectCustom(TEntity entity)
        {

        }

        /// <summary>
        /// Hàm thực hiện validate đối tượng bất kỳ
        /// </summary>
        /// <typeparam name="TValidate">Kiểu của đối tượng</typeparam>
        /// <param name="entityValidate">Đối tượng</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (20.11.2022)
        protected async Task Validate<TValidate>(TValidate entityValidate) where TValidate : BaseEntity
        {
            // Validate thông tin bắt buộc nhập:
            var props = typeof(TValidate).GetProperties();
            foreach (var prop in props)
            {
                var propName = prop.Name;
                var propValue = prop.GetValue(entityValidate);
                var propType = prop.PropertyType;
                var propRequired = prop.IsDefined(typeof(Required), true);
                var propEmailCheck = prop.IsDefined(typeof(EmailValid), true);
                var propDuplicateCheck = prop.IsDefined(typeof(NotDuplicate), true);
                if (propRequired && (propValue == null || string.IsNullOrEmpty(propValue.ToString())))
                {
                    var requiredAttr = Attribute.GetCustomAttributes(prop, typeof(Required))[0];
                    AddErrors(propName, (requiredAttr as Required).ErroValidate);
                }

                if (propEmailCheck && propValue != null)
                {
                    ValidateEmail(propValue.ToString());
                }
                if (propDuplicateCheck)
                {
                    await ValidateDupliacate<TValidate>(prop, propValue, propName, entityValidate);
                }
            }
        }

        /// <summary>
        /// Validate Email có đúng định dạng hay không?
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true- hợp lệ; false - không hợp lệ</returns>
        /// CreatedBy: NVMANH (17/08/2022)
        private bool ValidateEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                AddErrors("Email", "Email không đúng định dạng.");
                return false;
            }
            else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    if (addr.Address != trimmedEmail)
                    {
                        AddErrors("Email", "Email không đúng định dạng.");
                        return false;
                    }
                    return true;

                }
                catch
                {
                    AddErrors("Email", "Email không đúng định dạng.");
                    return false;
                }
            }
        }

        /// <summary>
        /// Kiểm tra thông tin có bị trùng lặp trong hệ thống hay không
        /// </summary>
        /// <typeparam name="TValidate">Kiểu của đối tượng cần kiểm tra</typeparam>
        /// <param name="prop">Property cần kiểm tra</param>
        /// <param name="propValue">Giá trị</param>
        /// <param name="propName">Tên</param>
        /// <returns>true - Hợp lệ; false - không hợp lệ</returns>
        /// CreatedBy: NVMANH (29/09/2022)
        private async Task<bool> ValidateDupliacate<TValidate>(PropertyInfo prop, object? propValue, string propName, TValidate entity) where TValidate : BaseEntity
        {
            var isDuplicate = await _repository.CheckDuplicateValue(prop.Name, propValue, entity) == true;
            if (isDuplicate == true)
            {
                var propDuplicate = Attribute.GetCustomAttribute(prop, typeof(NotDuplicate));
                var propLabel = GetPropLabel(prop);
                var msg = (propDuplicate as NotDuplicate).Error;
                msg = (msg != null ? msg : $"{propLabel} đã tồn tại trong hệ thống.");
                AddErrors(propName, msg);
                return false;
            }
            return true;
        }

        private string GetPropLabel(PropertyInfo prop)
        {
            var propHasLabel = Attribute.IsDefined(prop, typeof(LabelText));
            var propLabel = prop.Name;
            if (propHasLabel)
            {
                var labelText = Attribute.GetCustomAttribute(prop, typeof(LabelText));
                propLabel = (labelText as LabelText).Text;
            }
            return propLabel;
        }
        /// <summary>
        /// Hàm thực hiện thêm mới thông tin lỗi vào Errors:
        /// </summary>
        /// <param name="key">Khóa để xác định lỗi</param>
        /// <param name="errorMsg">Nội dung thông báo lỗi</param>
        /// CreatedBy: NVMANH (17/10/2022)
        protected void AddErrors(string key, string errorMsg)
        {
            // Kiểm tra xem key hiện tại đã có errors chưa?
            // Nếu có thì thực hiện add thêm, nếu chưa thì tạo mới:
            Errors.TryGetValue(key, out var error);
            if (error != null)
                error.Add(errorMsg);
            else
            {
                error = new List<string>() { errorMsg };
                Errors.Add(key, error);
            }

        }

        public virtual async Task<PagingData> GetFilterPaging(string keyFilter, int limit, int offset, HashSet<ColumnSortInfo> columnSorts = null, HashSet<ColumnFilterInfo> columnFilters = null)
        {

            var data = await _repository.GetFilterPaging(keyFilter, limit, offset, columnSorts, columnFilters);
            return data;
        }

        public virtual async Task<int> UpdateAsync(object entity, object? pks = null)
        {
            var res = await _repository.UpdateAsync(entity, pks);
            return res;
        }

        public virtual async Task<string?> GenerateNewCodeUniqueFromStringValue(string? stringValue, string? columnName = null)
        {
            var newName = stringValue?.Trim();
            var preCode = "";
            if (newName != null)
            {
                // Cắt chuỗi:
                string[] arrayCode = newName.Split(' ');

                // chỉ lấy ký tự đầu tiên:
                foreach (string code in arrayCode)
                {
                    var codeAfter = CommonFunction.RemoveDiacritics(code);
                    var firstChar = codeAfter.Substring(0, 1).Trim().ToUpper();
                    preCode = $"{preCode}{firstChar}";
                }
            }

            // Lấy mã mới trong hệ thống:
            var newCode = await UnitOfWork.InventoryItems.GetNewValueUnique<TEntity>(preCode, columnName);

            return newCode;
        }
    }
}
