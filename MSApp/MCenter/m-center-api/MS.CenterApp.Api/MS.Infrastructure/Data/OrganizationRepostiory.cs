using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Utilities;
using MS.Infrastructure.UnitOfWork;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MISA.FM.Infrastructure.Repositories;
using MS.Infrastructure.Interfaces;

namespace MS.Infrastructure.Data
{
    public class OrganizationRepostiory : DapperRepository<Organization>, IOrganizationRepository
    {
        IConfiguration _configuration;
        IWebHostEnvironment _webHostEnvironment;
        INavbarItemRepository _navRepository;
        public OrganizationRepostiory(IMSDatabaseContext dbContext, IConfiguration configuration, INavbarItemRepository navRepository) : base(dbContext)
        {
            _configuration = configuration;
            _navRepository = navRepository;
        }

        public async override Task<int> UpdateAsync(Organization entity, object pks)
        {
            return await DbContext.UpdateAsync<Organization>(entity, true);
        }

        public FormFile BackUpDatabase(MemoryStream ms)
        {
            string constring = DbContext.GetConnectionString();
            string file = $"{AppDomain.CurrentDomain.BaseDirectory}/database/backup.sql";
            Console.WriteLine(file);
            //using MemoryStream ms = new MemoryStream();
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(constring))
            {
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand())
                {
                    using (MySql.Data.MySqlClient.MySqlBackup mb = new MySql.Data.MySqlClient.MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        //mb.ExportToFile(file);
                        mb.ExportToMemoryStream(ms);
                        conn.Close();
                    }
                }
            }

            //using (FileStream fileBackup = new FileStream(file, FileMode.Open, FileAccess.Read))
            //{
            //    fileBackup.CopyTo(ms);
            //}

            var backupFile = new FormFile(ms, 0, ms.ToArray().Length, "db_backup", "backup.sql");
            return backupFile;

        }

        public async override Task<PagingData> GetFilterPaging(string[] columnsSort, string[] columlsFilter, string keyFilter, int limit, int offset)
        {
            var pagingData = new PagingData();
            var tableName = "Organization";
            var columnSortString = "CreatedDate DESC";
            if (columnsSort.Length > 0)
            {
                columnSortString = string.Join(",", columnsSort);
            }

            var whereCondition = $"WHERE 1=1";
            var limitOffsetCondition = "";

            if (limit > 0)
            {
                limitOffsetCondition = " LIMIT @limit OFFSET @offset";
            }

            if (columlsFilter.Length > 0 && !string.IsNullOrEmpty(keyFilter))
            {
                var colCondition = new HashSet<string>();
                foreach (var column in columlsFilter)
                {
                    colCondition.Add($"{column} LIKE CONCAT('%',@keyFilter,'%')");
                }

                whereCondition += $" AND ({string.Join(" OR ", colCondition)})";
            }


            var sql = $"CREATE TEMPORARY TABLE IF NOT EXISTS tbTemp AS " +
                         $"SELECT ROW_NUMBER() OVER(ORDER BY {columnSortString}) AS RowIndex, " +
                         $"i.* FROM {tableName} i " +
                         $"{whereCondition};" +
                         $"SELECT COUNT(*) AS TotalRecords FROM tbTemp;" +
                         $"SELECT * FROM tbTemp {limitOffsetCondition};";
            var parameters = new DynamicParameters();
            parameters.Add("@keyFilter", keyFilter);
            parameters.Add("@limit", limit);
            parameters.Add("@offset", offset);
            //var res = await Connection.QueryMultipleAsync(sql, parameters, transaction: Transaction);
            using (var multi = await DbContext.Connection.QueryMultipleAsync(sql, parameters, transaction: DbContext.Transaction))
            {
                pagingData.TotalRecords = multi.ReadFirstOrDefault<int>();
                pagingData.Data = multi.Read<object>();
            }
            return pagingData;
        }

        public async Task<Organization> GetOrganizationByUserName(string userName)
        {
            var sql = "SELECT o.* FROM Organization o " +
                "INNER JOIN Employee e ON o.OrganizationId = e.OrganizationId " +
                "INNER JOIN User u ON u.EmployeeId = e.EmployeeId " +
                "WHERE u.UserName = @UserName";
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", userName);
            var org = await DbContext.Connection.QueryFirstOrDefaultAsync<Organization>(sql, parameters, transaction: DbContext.Transaction);
            return org;
        }

        public Task<IEnumerable<OrganizationStatisticDto>> GetOrgStatisticByDay(DateTime? date)
        {
            var storeName = "Proc_Statistic_GetOrgStatisticByDay";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_Date", date);
            var data = DbContext.Connection.QueryAsync<OrganizationStatisticDto>(storeName, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            return data;
        }

        public async override Task<Organization> GetByIdAsync(object pksFields)
        {
            var org = await base.GetByIdAsync(pksFields);
            var storeGetSetting = "Proc_Setting_GetByOrgID";
            var sqlNavbar = "SELECT * FROM NavbarItem n WHERE n.OrganizationId = @p_OrganizationId OR n.IsSystem = 1 ORDER BY n.Sort";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("p_OrganizationId", CommonFunction.OrganizationId);
            var setting = await DbContext.Connection.QueryAsync<Setting>(storeGetSetting, parameters, transaction: DbContext.Transaction);
            var navs = await _navRepository.GetAllAsync();
            //var navs = await DbContext.Connection.QueryAsync<NavbarItem>(sqlNavbar, parameters, transaction: DbContext.Transaction);
            org.Setting = setting.ToList();
            org.NavbarItem = navs.ToList();
            return org;
        }
    }
}
