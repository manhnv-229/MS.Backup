using MS.Core.DTOs;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IDatabaseHelper
    {
        /// <summary>
        /// Sau lưu Database vào file
        /// </summary>
        /// <param name="catalogInfo"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (25/04/2024)
        public Task BackupDatabaseFromHost(Catalog catalogInfo);

        /// <summary>
        /// Tạo mới một Database
        /// </summary>
        /// <param name="catalogInfo"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (25/04/2024)
        public Task CreateNewDatabase(Catalog catalogInfo);

        /// <summary>
        /// Xoá cơ sở dữ liệu
        /// </summary>
        /// <param name="catalogInfo"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (25/04/2024)
        public Task DropDatabase(Catalog catalogInfo);

        /// <summary>
        /// Khôi phục database từ File
        /// </summary>
        /// <param name="catalogInfo"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (25/04/2024)
        public Task RestoreDatabaseFromBackupFile(Catalog catalogInfo);

        /// <summary>
        /// Sao chép Database từ Server này sang Server khác
        /// </summary>
        /// <param name="catalogSource">Thông tin nguồn Database</param>
        /// <param name="catalogTarget">Thông tin đích database</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (25/04/2024)
        public Task CloneDatabase(Catalog catalogSource, Catalog catalogTarget);

        /// <summary>
        /// Thực hiện sao chép Database sang Db mới từ BAZA App
        /// </summary>
        /// <param name="catalogTarget"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (25/04/2024)
        public Task InitDbBAZA(TenantRegister tenant);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantRegister"></param>
        /// <returns></returns>
        public Task CreateFirstUserAndRole(TenantRegister tenantRegister);

        /// <summary>
        /// Thêm license dùng thử 1 tháng
        /// </summary>
        /// <param name="tenantRegister"></param>
        /// <returns></returns>
        public Task AddFirstLicense(TenantRegister tenantRegister);

        /// <summary>
        /// Kiểm tra có phát sinh nghiệp vụ mua hàng hay chưa
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns>true - đã có; false- chưa có</returns>
        /// CreatedBy: NVMANH (25/04/2024)
        public Task<bool> CheckArisingBussinessDataAsync(Catalog catalog);
    }
}
