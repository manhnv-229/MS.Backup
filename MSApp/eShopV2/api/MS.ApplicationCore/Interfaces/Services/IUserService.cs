﻿using Microsoft.AspNetCore.Mvc;
using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Entities.Auth;
using MS.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Interface.Service
{
    public interface IUserService:IBaseService<User>
    {
        Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        Task<UserInfo> GetUserInfoById(string id);

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <param name="user"></param>
        Task<int> Register(RegisterModel user);

        object? LogOut();

        /// <summary>
        /// Xác thực Email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> ConfirmEmail(UserTokenConfirm user);

        /// <summary>
        /// Thực hiện xác nhận thông tin User hàng loạt.
        /// </summary>
        /// <param name="users">Danh sách User thực hiện xác thực thông tin</param>
        /// <returns>Số lượng User đã xác thực</returns>
        /// CreatedBy: NVMANH (10/09/2022)
        Task<int> ConfirmMultiUser(IEnumerable<User> users);

        Task<bool> ChangePassword(UserChangePassword userChangePassword);    }
}
