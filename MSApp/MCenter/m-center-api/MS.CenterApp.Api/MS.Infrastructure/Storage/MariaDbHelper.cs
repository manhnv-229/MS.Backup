using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Exceptions;
using MS.Core.Interfaces;
using MS.Core.MSEnums;
using MS.Core.Utilities;
using MS.Infrastructure.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Storage
{
    public class MariaDbHelper : IDatabaseHelper
    {
        private IHostingEnvironment _env;
        IConfiguration _config;
        IMSDatabaseContext _dbContext;

        public MariaDbHelper(IMSDatabaseContext dbContext, IHostingEnvironment env, IConfiguration config)
        {
            _dbContext = dbContext;
            _env = env;
            _config = config;
        }

        public async Task BackupDatabaseFromHost(Catalog catalogInfo)
        {
            int? port = 3306;
            if (catalogInfo.Port != null)
            {
                port = catalogInfo.Port;
            }

            string connectionString = $"Host={catalogInfo.Server};" +
                                        $"Port = {port};" +
                                        $"User Id={catalogInfo.UserId};" +
                                        $"Password={catalogInfo.Password};" +
                                        $"Database={catalogInfo.DatabaseName};" +
                                        $"charset=utf8;" +
                                        $"convertzerodatetime=true;" +
                                        $"Allow User Variables = True"; ;


            var file = Path.Combine(_env.ContentRootPath, @"FileTemplate\" + "MS_Baza_Default.sql");
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(file);
                        conn.Close();
                        Console.WriteLine("Backup success!");
                    }
                }
            }
        }

        public async Task CloneDatabase(Catalog catalogSource, Catalog catalogTarget)
        {
            int? port = 3306;
            if (catalogSource.Port != null)
            {
                port = catalogSource.Port;
            }

            // Backup database gốc -> chuyển tệp backup vào stream:
            string connectionStringSOURCE = $"Host={catalogSource.Server};" +
                                            $"Port = {port};" +
                                            $"User Id={catalogSource.UserId};" +
                                            $"Password={catalogSource.Password};" +
                                            $"Database={catalogSource.DatabaseName};" +
                                            $"charset=utf8;" +
                                            $"convertzerodatetime=true;" +
                                            $"Allow User Variables = True"; ;
            using (MemoryStream ms = new())
            {
                using (MySqlConnection connSOURCE = new MySqlConnection(connectionStringSOURCE))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = connSOURCE;
                            connSOURCE.Open();
                            //mb.ExportToFile(file);
                            mb.ExportToMemoryStream(ms);
                            connSOURCE.Close();
                            Console.WriteLine($"backup db success!");
                        }
                    }
                }

                // Tạo database từ strem file:
                await CreateNewDatabase(catalogTarget);
                try
                {
                    //var backupFile = new FormFile(ms, 0, ms.ToArray().Length, "db_backup", "backup.sql");
                    string connectionStringTARGET = $"Host={catalogTarget.Server};" +
                                                      $"Port = {port};" +
                                                      $"User Id={catalogTarget.UserId};" +
                                                      $"Password={catalogTarget.Password};" +
                                                      $"Database={catalogTarget.DatabaseName};" +
                                                      $"charset=utf8;" +
                                                      $"convertzerodatetime=true;" +
                                                      $"Allow User Variables = True";
                    using (MySqlConnection connTARGET = new MySqlConnection(connectionStringTARGET))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = connTARGET;
                                connTARGET.Open();
                                //mb.ExportToFile(file);
                                mb.ImportFromMemoryStream(ms);
                                connTARGET.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DropDatabase(catalogTarget);
                    throw;
                }

            }
        }

        public void ImportDatabase()
        {
            string constring = _dbContext.GetConnectionString();

            // Important Additional Connection Options
            constring += "charset=utf8;convertzerodatetime=true;";

            var file = Path.Combine(_env.ContentRootPath, @"FileTemplate\" + "MS_Baza_Default.sql");

            string script = File.ReadAllText(file);

            using (MySqlConnection conn = new MySqlConnection(constring))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ImportFromFile(file);
                        conn.Close();
                    }
                }
            }
        }


        public async Task CreateNewDatabase(Catalog catalogInfo)
        {
            //string constring = _dbContext.GetConnectionString();
            var connectionString = GetConnectionStringNotIncludeDatabaseFromCatalog(catalogInfo);

            var sqlCreate = $"DROP DATABASE IF EXISTS {catalogInfo.DatabaseName}; " +
                $"CREATE DATABASE {catalogInfo.DatabaseName} CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci; " +
                $"USE {catalogInfo.DatabaseName}";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    //var transaction = conn.BeginTransaction();
                    cmd.CommandText = sqlCreate;
                    cmd.CommandType = CommandType.Text;
                    try
                    {
                        var res = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Create Database success! -- {res} efected!");
                        //transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        //transaction.Rollback();
                        throw new MSException(System.Net.HttpStatusCode.InternalServerError, ex.Message);
                    }
                    finally { conn.Close(); }

                }
            }
        }

        public async Task DropDatabase(Catalog catalogInfo)
        {
            //string constring = _dbContext.GetConnectionString();
            var connectionString = GetConnectionStringNotIncludeDatabaseFromCatalog(catalogInfo);

            var sqlCreate = $"DROP DATABASE IF EXISTS {catalogInfo.DatabaseName};";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    //var transaction = conn.BeginTransaction();
                    cmd.CommandText = sqlCreate;
                    cmd.CommandType = CommandType.Text;
                    try
                    {
                        var res = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Drop Database success! -- {res} efected!");
                        //transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        //transaction.Rollback();
                        throw new MSException(System.Net.HttpStatusCode.InternalServerError, ex.Message);
                    }
                    finally { conn.Close(); }

                }
            }
        }

        public async Task RestoreDatabaseFromBackupFile(Catalog catalogInfo)
        {
            //string constring = _dbContext.GetConnectionString();
            int? port = 3306;
            if (catalogInfo.Port != null)
            {
                port = catalogInfo.Port;
            }
            string connectionString = $"Host={catalogInfo.Server};" +
                $"Port = {port};" +
                $"User Id={catalogInfo.UserId};" +
                $"Password={catalogInfo.Password};" +
                $"Database={catalogInfo.DatabaseName};" +
                $"charset=utf8;" +
                $"convertzerodatetime=true;" +
                $"Allow User Variables = True";

            var file = Path.Combine(_env.ContentRootPath, @"FileTemplate\" + "MS_Baza_Default.sql");

            Console.WriteLine($"Begin connect to {catalogInfo.DatabaseName}...");
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Console.WriteLine($"...connect success!");
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        Console.WriteLine($"begin restore....");
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ImportFromFile(file);
                        conn.Close();
                        Console.WriteLine($"restore complete!");
                    }
                }
            }
        }

        #region Apps
        public async Task InitDbBAZA(TenantRegister tenantRegister)
        {
            var file = Path.Combine(_env.ContentRootPath, @"FileTemplate\" + "MS_Baza_Default.sql");
            var catalogTarget = tenantRegister.Catalog;
            // Backup database gốc -> chuyển tệp backup vào stream:
            string connectionStringSOURCE = _config.GetConnectionString("BAZA_DefaultConnection");
            using (MemoryStream ms = new())
            {
                using (MySqlConnection connSOURCE = new MySqlConnection(connectionStringSOURCE))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            var lstFooter = new List<string>();

                            cmd.Connection = connSOURCE;
                            connSOURCE.Open();
                            mb.ExportInfo.ExportTableStructure = true;
                            mb.ExportInfo.ExportRows = true;
                            //mb.ExportInfo.TablesToBeExportedDic["Setting"] = "SELECT * FROM Setting";
                            //mb.ExportInfo.TablesToBeExportedDic["NavbarItem"] = "SELECT * FROM NavbarItem";
                            //if (tenantRegister.AddInventoryData)
                            //{
                            //    mb.ExportInfo.TablesToBeExportedDic["InventoryItemCategory"] = "SELECT * FROM InventoryItemCategory";
                            //    mb.ExportInfo.TablesToBeExportedDic["Unit"] = "SELECT * FROM Unit";
                            //    mb.ExportInfo.TablesToBeExportedDic["InventoryItem"] = "SELECT * FROM InventoryItem";

                            //}
                            if (tenantRegister.AddInventoryData)
                            {
                                lstFooter.Add($"UPDATE `InventoryItem` SET `OrganizationId` = '{tenantRegister.Organization.OrganizationId.ToString()}';");
                            }
                            else
                            {
                                lstFooter.Add("DELETE FROM `InventoryItem`;");
                                lstFooter.Add("DELETE FROM `InventoryItemCategory`;");
                                lstFooter.Add("DELETE FROM `Unit`;");
                            }
                            mb.ExportInfo.SetDocumentFooters(lstFooter);
                            mb.ExportToMemoryStream(ms);
                            //mb.ExportToFile(file);
                            connSOURCE.Close();
                            Console.WriteLine($"backup db success!");
                        }
                    }
                }

                // Tạo database từ strem file:
                await CreateNewDatabase(catalogTarget);
                try
                {
                    int? port = 3306;
                    if (catalogTarget.Port != null)
                    {
                        port = catalogTarget.Port;
                    }
                    string connectionStringTARGET = $"Host={catalogTarget.Server};" +
                                                      $"Port = {port};" +
                                                      $"User Id={catalogTarget.UserId};" +
                                                      $"Password={catalogTarget.Password};" +
                                                      $"Database={catalogTarget.DatabaseName};" +
                                                      $"charset=utf8;" +
                                                      $"convertzerodatetime=true;" +
                                                      $"Allow User Variables = True";
                    using (MySqlConnection connTARGET = new MySqlConnection(connectionStringTARGET))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = connTARGET;
                                connTARGET.Open();
                                //mb.ExportToFile(file);
                                mb.ImportFromMemoryStream(ms);
                                // Thêm thông tin user:
                                connTARGET.Close();
                                Console.WriteLine($"restore db success!");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DropDatabase(catalogTarget);
                    throw;
                }

            }
        }

        public async Task CreateFirstUserAndRole(TenantRegister tenantRegister)
        {
            var catalogTarget = tenantRegister.Catalog;
            int? port = 3306;
            if (catalogTarget.Port != null)
            {
                port = catalogTarget.Port;
            }
            string connectionStringTARGET = $"Host={catalogTarget.Server};" +
                                              $"Port = {port};" +
                                              $"User Id={catalogTarget.UserId};" +
                                              $"Password={catalogTarget.Password};" +
                                              $"Database={catalogTarget.DatabaseName};" +
                                              $"charset=utf8;" +
                                              $"convertzerodatetime=true;" +
                                              $"Allow User Variables = True";
            using (IDbConnection connection = new MySqlConnection(connectionStringTARGET))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                // Thêm thông tin đơn vị:
                var orgParams = new DynamicParameters();
                orgParams.Add("@p_OrganizationId", tenantRegister.Organization.OrganizationId);
                orgParams.Add("@p_OrganizationCode", tenantRegister.Organization.OrganizationCode);
                orgParams.Add("@p_OrganizationName", tenantRegister.Organization.OrganizationName);
                orgParams.Add("@p_OrganizationTypeName", tenantRegister.Organization.OrganizationTypeName);
                orgParams.Add("@p_ParentId", tenantRegister.Organization.ParentId);
                orgParams.Add("@p_TenantId", tenantRegister.Organization.TenantId);
                orgParams.Add("@p_ShortDescription", tenantRegister.Organization.ShortDescription);
                orgParams.Add("@p_Description", tenantRegister.Organization.Description);
                orgParams.Add("@p_Address", tenantRegister.Organization.Address);
                orgParams.Add("@p_PhoneNumber", tenantRegister.Organization.PhoneNumber);
                orgParams.Add("@p_Email", tenantRegister.Organization.Email);
                orgParams.Add("@p_Level", tenantRegister.Organization.Level);
                orgParams.Add("@p_Slogan", tenantRegister.Organization.Slogan);
                orgParams.Add("@p_OwnerName", tenantRegister.Organization.OwnerName);
                orgParams.Add("@p_BusinessLicense", tenantRegister.Organization.BusinessLicense);
                orgParams.Add("@p_Website", tenantRegister.Organization.Website);
                orgParams.Add("@p_IsConfirm", tenantRegister.Organization.IsConfirm);
                orgParams.Add("@p_CreatedDate", tenantRegister.Organization.CreatedDate);
                orgParams.Add("@p_CreatedBy", tenantRegister.Organization.CreatedBy);
                orgParams.Add("@p_ModifiedDate", tenantRegister.Organization.ModifiedDate);
                orgParams.Add("@p_ModifiedBy", tenantRegister.Organization.ModifiedBy);
                var addOrg = await connection.ExecuteAsync("Proc_Organization_Insert", orgParams, commandType: CommandType.StoredProcedure);

                // Thêm user:
                if (addOrg > 0)
                {
                    Console.WriteLine("add organization success!");
                    tenantRegister.TenantUser.UserId = Guid.NewGuid();
                    var userParams = new DynamicParameters();

                    userParams.Add("@p_UserId", tenantRegister.TenantUser.UserId);
                    userParams.Add("@p_Email", tenantRegister.TenantUser.Email);
                    userParams.Add("@p_EmailConfirmed", tenantRegister.TenantUser.EmailConfirmed);
                    userParams.Add("@p_PasswordHash", tenantRegister.TenantUser.PasswordHash);
                    userParams.Add("@p_SecurityStamp", tenantRegister.TenantUser.SecurityStamp);
                    userParams.Add("@p_PhoneNumber", tenantRegister.TenantUser.PhoneNumber);
                    userParams.Add("@p_PhoneNumberConfirmed", tenantRegister.TenantUser.PhoneNumberConfirmed);
                    userParams.Add("@p_TwoFactorEnabled", tenantRegister.TenantUser.TwoFactorEnabled);
                    userParams.Add("@p_LockoutEndDateUtc", tenantRegister.TenantUser.LockoutEndDateUtc);
                    userParams.Add("@p_LockoutEnabled", tenantRegister.TenantUser.LockoutEnabled);
                    userParams.Add("@p_AccessFailedCount", tenantRegister.TenantUser.AccessFailedCount);
                    userParams.Add("@p_UserName", tenantRegister.TenantUser.UserName);
                    userParams.Add("@p_OrganizationId", tenantRegister.TenantUser.OrganizationId);
                    userParams.Add("@p_EmployeeId", tenantRegister.TenantUser.EmployeeId);
                    userParams.Add("@p_RefreshToken", tenantRegister.TenantUser.RefreshToken);
                    userParams.Add("@p_RefreshTokenExpiryTime", tenantRegister.TenantUser.RefreshTokenExpiryTime);
                    userParams.Add("@p_CreatedDate", tenantRegister.TenantUser.CreatedDate);
                    userParams.Add("@p_CreatedBy", tenantRegister.TenantUser.CreatedBy);
                    userParams.Add("@p_ModifiedDate", tenantRegister.TenantUser.ModifiedDate);
                    userParams.Add("@p_ModifiedBy", tenantRegister.TenantUser.ModifiedBy);
                    var addUser = await connection.ExecuteAsync("Proc_User_Insert", param: userParams, commandType: CommandType.StoredProcedure);
                    if (addUser > 0)
                    {
                        Console.WriteLine("add user success!");
                        // Caaps quyen cao nhat:
                        var addRole = await connection.ExecuteAsync("Proc_UserRole_AddRoleForFirstUser", new { p_UserId = tenantRegister.TenantUser.UserId }, commandType: CommandType.StoredProcedure);
                        if (addRole > 0)
                        {
                            Console.WriteLine("add role success!");
                        }
                        transaction.Commit();
                    }
                }
                connection.Close();

            }
        }

        public async Task UpdateOrganizationId(TenantRegister tenantRegister)
        {

        }
        public async Task AddFirstLicense(TenantRegister tenantRegister)
        {
            var catalogTarget = tenantRegister.Catalog;
            int? port = 3306;
            if (catalogTarget.Port != null)
            {
                port = catalogTarget.Port;
            }
            string connectionStringTARGET = $"Host={catalogTarget.Server};" +
                                              $"Port = {port};" +
                                              $"User Id={catalogTarget.UserId};" +
                                              $"Password={catalogTarget.Password};" +
                                              $"Database={catalogTarget.DatabaseName};" +
                                              $"charset=utf8;" +
                                              $"convertzerodatetime=true;" +
                                              $"Allow User Variables = True";
            using (IDbConnection connection = new MySqlConnection(connectionStringTARGET))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                var sqlAddLicense = "INSERT INTO MSLicense(MSLicenseId,MSLicenseCode,OrganizationId,LicenseType,StartDate,ExpiredDate) VALUES(@MSLicenseId,@MSLicenseCode,@OrganizationId,@LicenseType,@StartDate,@ExpiredDate)";
                var addLicense = await connection.ExecuteAsync(sqlAddLicense, tenantRegister.License, commandType: CommandType.Text);
                if (addLicense > 0)
                {
                    Console.WriteLine("add license success!");
                }
                transaction.Commit();
            }
        }

        public async Task<bool> CheckArisingBussinessDataAsync(Catalog catalog)
        {
            var connectionString = GetConnectionStringFromCatalog(catalog);
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var sql = "SELECT RefNo FROM Ref LIMIT 1;";
                    var data = await connection.QueryFirstOrDefaultAsync<string>(sql);
                    connection.Close();
                    return data != null;
                }
                catch (MySqlException mslEx)
                {
                    if (mslEx.Number != (int)MySqlErrorCode.UnknownDatabase)
                    {
                        Console.WriteLine(mslEx.Message);
                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        /// <summary>
        /// Lấy chuỗi kết nối từ Catalog
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        private string GetConnectionStringFromCatalog(Catalog catalog)
        {
            int? port = 3306;
            if (catalog.Port != null)
            {
                port = catalog.Port;
            }
            return $"Host={catalog.Server};" +
                                             $"Port = {port};" +
                                             $"User Id={catalog.UserId};" +
                                             $"Password={catalog.Password};" +
                                             $"Database={catalog.DatabaseName};" +
                                             $"charset=utf8;" +
                                             $"convertzerodatetime=true;" +
                                             $"Allow User Variables = True";
        }

        /// <summary>
        /// Lấy chuỗi kết nối từ Catalog không bao gồm thống tin CSDL (dùng để thêm hoặc xoá Database)
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        private string GetConnectionStringNotIncludeDatabaseFromCatalog(Catalog catalog)
        {
            int? port = 3306;
            if (catalog.Port != null)
            {
                port = catalog.Port;
            }
            return $"Host={catalog.Server};" +
                                             $"Port = {port};" +
                                             $"User Id={catalog.UserId};" +
                                             $"Password={catalog.Password};" +
                                             $"charset=utf8;" +
                                             $"convertzerodatetime=true;" +
                                             $"Allow User Variables = True";
        }
        #endregion
    }
}
