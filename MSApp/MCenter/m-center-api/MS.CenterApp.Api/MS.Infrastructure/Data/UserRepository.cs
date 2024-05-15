﻿using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using MISA.FM.Infrastructure.Repositories;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.Interfaces;
using MS.Core.Utilities;
using MS.Infrastructure.Interfaces;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Infrastructure.Data
{
    public class UserRepository : DapperRepository<User>, IUserRepository
    {
        private readonly IMapper _mapper;
        public UserRepository(IMSDatabaseContext mySqlDbContext, IMapper mapper) : base(mySqlDbContext)
        {
            _mapper = mapper;
        }
        public async Task<UserInfo> GetUserAuthenticate(string userName, string password)
        {

            var sql = "SELECT u.*,e.OrganizationId FROM User u " +
                "LEFT JOIN Employee e ON u.EmployeeId = e.EmployeeId " +
                "WHERE u.UserName = @UserName AND u.PasswordHash = @Password";
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", userName);
            parameters.Add("@Password", password);
            var user = await DbContext.Connection.QueryFirstOrDefaultAsync<UserInfo>(sql, param: parameters, transaction: DbContext.Transaction);

            // get roles:
            if (user == null)
            {
                return null;
            }
            var sqlRoles = "SELECT a.RoleId,a.RoleName,a.RoleValue, b.UserId FROM Role a INNER JOIN UserRole b ON a.RoleId = b.RoleId WHERE b.UserId = @UserId";
            var sqlEmployee = "SELECT * FROM Employee e WHERE e.EmployeeId = @employeeId AND @employeeId IS NOT NULL";
            parameters.Add("@UserId", user.UserId);
            parameters.Add(" @employeeId", user.EmployeeId);

            user.Roles = await DbContext.Connection.QueryAsync<Role>(sqlRoles, param: parameters, transaction: DbContext.Transaction);
            user.Employee = await DbContext.Connection.QueryFirstOrDefaultAsync<Employee>(sqlEmployee, param: parameters, transaction: DbContext.Transaction);
            return user;
        }

        public async override Task<User> GetByIdAsync(object id)
        {
            var sqlCommand = $"SELECT * FROM User WHERE UserId = @UserID";
            var sqlSelectRoles = $"SELECT anr.RoleId,anr.RoleValue," +
                                    "anr.RoleName, anr.OtherName FROM Role anr " +
                                    "LEFT JOIN UserRole anur ON anr.RoleId = anur.RoleId " +
                                    "WHERE anur.UserId = @UserID ORDER BY anr.RoleValue";
            var parameters = new DynamicParameters();
            parameters.Add($"@UserID", id);
            var userResponse = await DbContext.Connection.QueryFirstOrDefaultAsync<UserInfo>(sqlCommand, param: parameters, transaction: DbContext.Transaction);
            if (userResponse != null)
            {
                userResponse.Roles = await DbContext.Connection.QueryAsync<Role>(sqlSelectRoles, param: parameters, transaction: DbContext.Transaction);
                //user.Employee = await UnitOfWork.Connection.QueryFirstOrDefaultAsync<Employee>(sqlSelectEmployeeInfo, param: parameters, transaction: UnitOfWork.Transaction);

            }
            var user = _mapper.Map<UserInfo, User>(userResponse);
            return user;
        }

        public new UserInfo GetById(Guid id)
        {
            var sqlCommand = $"SELECT u.*,c.AvatarLink FROM User u LEFT JOIN Employee c ON u.EmployeeId = c.EmployeeId WHERE u.UserId = @UserID";
            var sqlSelectRoles = $"SELECT anr.RoleId,anr.RoleValue," +
                                    "anr.Name, anr.OtherName FROM Role anr " +
                                    "LEFT JOIN UserRole anur ON anr.RoleId = anur.RoleId " +
                                    "WHERE anur.UserId = @UserID ORDER BY anr.RoleValue DESC";
            var parameters = new DynamicParameters();
            parameters.Add($"@UserID", id);
            var user = DbContext.Connection.QueryFirstOrDefault<UserInfo>(sqlCommand, param: parameters, transaction: DbContext.Transaction);
            if (user != null)
            {
                user.Roles = DbContext.Connection.Query<Role>(sqlSelectRoles, param: parameters, transaction: DbContext.Transaction);
                //user.Employee = UnitOfWork.Connection.QueryFirstOrDefault<Employee>(sqlSelectEmployeeInfo, param: parameters, transaction: UnitOfWork.Transaction);

            }
            return user;
        }

        public async Task<int> Register(User user)
        {
            return await AddAsync(user);
            //return await Task.FromResult(1);
        }

        public async Task<bool> CheckEmailExist(string email)
        {

            var sql = "SELECT Email FROM User WHERE (Email IS NOT NULL AND Email = @Email)";
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);
            var res = await DbContext.Connection.QueryFirstOrDefaultAsync<User>(sql, param: parameters, transaction: DbContext.Transaction);
            if (res == null)
                return false;
            return true;
        }

        public async Task<bool> CheckPhoneNumberExist(string phone)
        {

            var sql = "SELECT PhoneNumber FROM User WHERE (PhoneNumber IS NOT NULL AND PhoneNumber = @PhoneNumber)";
            var parameters = new DynamicParameters();
            parameters.Add("@PhoneNumber", phone);
            var res = await DbContext.Connection.QueryFirstOrDefaultAsync<User>(sql, param: parameters, transaction: DbContext.Transaction);
            if (res == null)
                return false;
            return true;
        }

        public async Task<bool> CheckUserNameExist(string userName)
        {

            var sql = "SELECT UserName FROM User WHERE (UserName IS NOT NULL AND UserName = @UserName)";
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", userName);
            var res = await DbContext.Connection.QueryFirstOrDefaultAsync<Role>(sql, param: parameters, transaction: DbContext.Transaction);
            if (res == null)
                return false;
            return true;
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            var sql = "Proc_GetRolesByUserName";
            var parameters = new DynamicParameters();
            parameters.Add("@p_UserName", user.UserName);
            var data = await DbContext.Connection.QueryAsync<string>(sql, param: parameters, commandType: System.Data.CommandType.StoredProcedure);
            return data.ToList();
        }

        public async Task<User> FindByNameAsync(string userName)
        {

            var sql = "SELECT * FROM User WHERE (UserName = @UserName OR Email = @UserName OR PhoneNumber = @UserName)";
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", userName);
            var data = await DbContext.Connection.QueryFirstOrDefaultAsync<User>(sql, param: parameters, transaction: DbContext.Transaction);
            return data;
        }

        public async Task<int> AddToken(UserTokenConfirm userTokenConfirm)
        {
            //using (var connection = new MySqlConnection(ConnectionString))
            //{
            //    connection.Open();
            //    var sqlAdd = BuildAddQuery<UserTokenConfirm>(userTokenConfirm);
            //    var res = await connection.ExecuteAsync(sqlAdd, param: userTokenConfirm);
            //    return res;
            //}
            return await Task.FromResult(1);
        }

        public async Task<bool> ConfirmUserToken(UserTokenConfirm userTokenConfirm)
        {
            try
            {
                var sql = "SELECT * FROM UserTokenConfirm WHERE UserName = @UserName AND Email = @Email";
                var parameters = new DynamicParameters();
                parameters.Add("@UserName", userTokenConfirm.UserName);
                parameters.Add("@Email", userTokenConfirm.Email);
                var useToken = await DbContext.Connection.QueryFirstOrDefaultAsync<UserTokenConfirm>(sql, param: parameters, transaction: DbContext.Transaction);
                if (useToken.TokenCode == userTokenConfirm.TokenCode)
                {

                    // Cập nhật tình trạng xác nhận:
                    var sqlUpdateConfirm = "UPDATE User anu SET anu.EmailConfirmed = 1 WHERE anu.UserId = @UserId;" +
                        "DELETE FROM UserTokenConfirm WHERE UserId = @UserId";
                    parameters.Add("@UserId", useToken.UserId);
                    var res = await DbContext.Connection.ExecuteAsync(sqlUpdateConfirm, param: parameters, transaction: DbContext.Transaction);
                    if (res > 0)
                    {
                        // Bổ sung thêm 15 ngày VIP cho tài khoản:
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<IEnumerable<Role>> GetRoles()
        {

            var sql = "SELECT * FROM Role";
            var data = await DbContext.Connection.QueryAsync<Role>(sql);
            return data;
        }

        public async Task<int> ConfirmMultiUser(IEnumerable<User> users)
        {
            var res = 0;
            if (users != null)
            {
                foreach (var user in users)
                {
                    var sqlUpdateConfirm = "UPDATE User anu SET anu.EmailConfirmed = 1 WHERE anu.UserId = @UserId;";
                    var sqlDeleteConfirm = "DELETE FROM UserTokenConfirm WHERE UserId = @UserId";

                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", user.UserId);

                    // Cập nhật tình trạng
                    res += await DbContext.Connection.ExecuteAsync(sqlUpdateConfirm, param: parameters, transaction: DbContext.Transaction);

                    // Xóa Token (nếu có):
                    await DbContext.Connection.ExecuteAsync(sqlDeleteConfirm, param: parameters, transaction: DbContext.Transaction);
                }
            }
            return res;
        }

        public async Task<UserRegisterResponse> GetByPhoneNumber(string phoneNumber)
        {
            var sql = "SELECT c.EmployeeId,a.UserId,a.UserName,c.PhoneNumber,c.MobileNumber FROM Employee c LEFT JOIN User a ON c.EmployeeId = a.EmployeeId WHERE (c.PhoneNumber = @PhoneNumber OR c.MobileNumber = @MobileNumber)";
            var parameters = new DynamicParameters();
            parameters.Add("@PhoneNumber", phoneNumber);
            parameters.Add("@MobileNumber", phoneNumber);
            var data = await DbContext.Connection.QueryFirstOrDefaultAsync<UserRegisterResponse>(sql, param: parameters, transaction: DbContext.Transaction);
            return data;
        }

        public async Task<UserInfo> GetUserInfoResponseById(string id)
        {
            var sqlCommand = $"SELECT u.*,c.FirstName,c.LastName,c.FullName,c.AvatarLink FROM User u LEFT JOIN Employee c ON u.EmployeeId = c.EmployeeId WHERE u.UserId = @UserID";
            var sqlSelectRoles = $"SELECT anr.RoleId,anr.RoleValue," +
                                    "anr.RoleName, anr.OtherName FROM Role anr " +
                                    "LEFT JOIN UserRole anur ON anr.RoleId = anur.RoleId " +
                                    "WHERE anur.UserId = @UserID ORDER BY anr.RoleValue DESC";
            var sqlSelectEmployeeInfo = "SELECT e.* FROM Employee e INNER JOIN User u ON e.EmployeeId = u.EmployeeId WHERE u.UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add($"@UserID", id);
            var user = await DbContext.Connection.QueryFirstOrDefaultAsync<UserInfo>(sqlCommand, param: parameters, transaction: DbContext.Transaction);
            if (user != null)
            {
                user.Roles = await DbContext.Connection.QueryAsync<Role>(sqlSelectRoles, param: parameters, transaction: DbContext.Transaction);
                user.Employee = await DbContext.Connection.QueryFirstOrDefaultAsync<Employee>(sqlSelectEmployeeInfo, param: parameters, transaction: DbContext.Transaction);

            }
            return user;
        }

        public async Task<string> GetUserNameByContactId(Guid contactId)
        {
            var sql = "SELECT u.UserName FROM User u WHERE u.ContactId = @ContactId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ContactId", contactId);
            var userName = await DbContext.Connection.QueryFirstOrDefaultAsync<string>(sql, param: parameters, transaction: DbContext.Transaction);
            return userName;
        }

        public async Task<OrganizationInfo> GetOrganizationInfoByUserID(string? id = null)
        {
            var sql = "Proc_GetOrganizationInfoByUserID";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_UserId", id);
            parameters.Add("@p_OrganizationId", CommonFunction.OrganizationId);
            var result = await DbContext.Connection.QueryFirstOrDefaultAsync<OrganizationInfo>(sql, param: parameters, transaction: DbContext.Transaction, commandType: System.Data.CommandType.StoredProcedure);
            if (result != null)
            {
                var sqlNavbar = "SELECT * FROM NavbarItem n WHERE n.OrganizationId = @p_OrganizationId OR n.IsSystem = 1 ORDER BY n.Sort";
                var navs = await DbContext.Connection.QueryAsync<NavbarItem>(sqlNavbar, parameters, transaction: DbContext.Transaction);
                result.NavbarItem = navs.ToList();
            }
            return result;
        }

        public async Task<bool> ChangePassword(object userId, string passwordHash)
        {
            var sql = "UPDATE User u SET u.PasswordHash = @PasswordHash WHERE u.UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("@PasswordHash", passwordHash);
            parameters.Add("@UserId", userId);
            var res = await DbContext.Connection.ExecuteAsync(sql, parameters);
            if (res > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<UserInfo> GetUserInfoResponseByEmployeeId(string id)
        {
            var sqlCommand = $"SELECT u.*,c.FirstName,c.LastName,c.FullName,c.AvatarLink " +
                                $"FROM User u LEFT JOIN Employee c ON u.EmployeeId = c.EmployeeId " +
                                    $"WHERE u.EmployeeId = @EmployeeId";
            var sqlSelectRoles = $"SELECT anr.RoleId,anr.RoleValue," +
                                    "anr.RoleName, anr.OtherName FROM Role anr " +
                                    "LEFT JOIN UserRole anur ON anr.RoleId = anur.RoleId " +
                                    "WHERE anur.UserId = @UserId ORDER BY anr.RoleValue DESC";
            var sqlSelectEmployeeInfo = "SELECT e.* FROM Employee e INNER JOIN User u ON e.EmployeeId = u.EmployeeId WHERE e.EmployeeId = @EmployeeId";
            var parameters = new DynamicParameters();
            parameters.Add($"@EmployeeId", id);
            var user = await DbContext.Connection.QueryFirstOrDefaultAsync<UserInfo>(sqlCommand, param: parameters, transaction: DbContext.Transaction);
            if (user != null)
            {
                parameters.Add($"@UserId", user.UserId);
                user.Roles = await DbContext.Connection.QueryAsync<Role>(sqlSelectRoles, param: parameters, transaction: DbContext.Transaction);
                user.Employee = await DbContext.Connection.QueryFirstOrDefaultAsync<Employee>(sqlSelectEmployeeInfo, param: parameters, transaction: DbContext.Transaction);

            }
            return user;
        }

        public async Task UpdateUserName(object userId, string newUserName)
        {
            var sql = "UPDATE User u SET u.UserName = @UserName WHERE u.UserId = @UserId;";
            var parameters = new DynamicParameters();
            parameters.Add($"@UserId", userId);
            parameters.Add($"@UserName", newUserName);
            await DbContext.Connection.ExecuteAsync(sql: sql, param: parameters, transaction: DbContext.Transaction);
        }
    }
}
