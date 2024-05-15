﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.DTOs;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Entities.Auth;
using MS.ApplicationCore.Exceptions;
using MS.ApplicationCore.Interface.Service;
using MS.ApplicationCore.Interfaces;
using MS.ApplicationCore.MSEnums;
using MS.ApplicationCore.Services;
using MS.Infrastructure.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MS.ThaiSanlonV2.Api.Controllers
{
    public class AccountsController : BaseController<User>
    {
        readonly IUserRepository _repository;
        readonly IUserService _service;
        readonly IJwtUtils _jwtUntils;
        private readonly IConfiguration _configuration;
        readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AccountsController(IUserRepository repository, IUserService baseService, IJwtUtils jwtUntils, IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, baseService)
        {
            _repository = repository;
            _service = baseService;
            _jwtUntils = jwtUntils;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [Authorize(ApplicationCore.MSEnums.MSRole.Member)]
        public async override Task<IActionResult> Get(string id)
        {
            var data = await _unitOfWork.Users.GetUserInfoResponseById(id);
            return Ok(data);
        }

        [Authorize]
        [HttpGet("organization-info")]
        public async Task<IActionResult> GetClassInfo()
        {
            var userId = HttpContext?.User?.Claims?.First(x => x.Type == "id").Value;
            var classInfo = await _unitOfWork.Users.GetOrganizationInfoByUserID(userId);
            return Ok(classInfo);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("register")]
        public async Task<IActionResult> GetByPhone(string phoneNumber)
        {
            return Ok(await _unitOfWork.Users.GetByPhoneNumber(phoneNumber));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel useModel)
        {
            var userInfo = await _unitOfWork.Users.GetUserAuthenticate(useModel.Username, _jwtUntils.HashPassword(useModel.Password));
            if (userInfo != null)
            {
                // Lấy thông tin đơn vị/ license:
                var licenseInfo = await _unitOfWork.MSLicenses.GetLicenseByUserId(userInfo.UserId);

                userInfo.LicenseExpiredDate = licenseInfo?.ExpiredDate;

                var token = _jwtUntils.GenerateJwtToken(userInfo);
                var refreshToken = JwtUtils.GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
                _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

                var user = _mapper.Map<UserInfo, User>(userInfo);

                

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
                await _unitOfWork.Users.UpdateAsync(user);

                var authenticateResponse = new AuthenticateResponse(userInfo, token);
                authenticateResponse.Expiration = DateTime.Now.AddDays(tokenValidityInMinutes);
                // Lấy ra quyền cao nhất của User nếu có
                var role = userInfo.Roles.FirstOrDefault();
                if (role != null)
                {
                    authenticateResponse.RoleValue = role.RoleValue;
                    authenticateResponse.RoleName = role.RoleName;
                }

                // Lấy thông tin lớp báo cho các client:

                return Ok(authenticateResponse);
            }
            else
            {
                var res = new
                {
                    userMsg = "Thông tin người dùng không hợp lệ",
                };
                return Unauthorized(res);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel user)
        {
            var result = await _service.Register(user);

            if (result == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "Không thể xử lý quá trình đăng ký, vui lòng liên hệ quản trị viên để được trợ giúp." });

            return CreatedAtAction("register",new { Status = "Success", Message = "Tạo tài khoản thành công!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _repository.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User already exists!" });


            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            //var result = await _userManager.CreateAsync(user, model.Password);
            var result = await _repository.AddAsync(user);
            if (result == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new  { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = _jwtUntils.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var username = principal.Identity.Name;

            var user = await _unitOfWork.Users.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var newAccessToken = _jwtUntils.CreateToken(principal.Claims.ToList());
            var newRefreshToken = JwtUtils.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _unitOfWork.Users.UpdateAsync(user);

            return new ObjectResult(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _unitOfWork.Users.FindByNameAsync(username);
            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;
            await _unitOfWork.Users.UpdateAsync(user);

            return NoContent();
        }

        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            _unitOfWork.BeginTransaction();
            var users = await _unitOfWork.Users.GetAllAsync();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _unitOfWork.Users.UpdateAsync(user);
            }
            _unitOfWork.Commit();
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail(UserTokenConfirm user)
        {
            var res = await _service.ConfirmEmail(user);
            if (res)
                return Ok(res);
            else
                throw new MISAException(System.Net.HttpStatusCode.BadRequest, $"Mã xác nhận không hợp lệ. Vui lòng nhập chính xác mã xác nhận của bạn được gửi tới Email. <br/> Lưu ý. Mã xác nhận chỉ có hiệu lực trong 24h.");
        }

        /// <summary>
        /// Thực hiện xác thực thông tin hàng loạt Email (Chỉ dành cho cấp nhân viên trở lên)
        /// </summary>
        /// <param name="users">Danh sách xác nhận</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (27/09/2022)
        [Authorize(MSRole.SuperManager)]
        [HttpPut("confirm-multi")]
        public async Task<IActionResult> ConfirmMultiEmail(IEnumerable<User> users)
        {
            var res = await _service.ConfirmMultiUser(users);
            if (res > 0)
            {
                return Ok(res);

            }
            else
            {
                throw new MISAException(System.Net.HttpStatusCode.NoContent,"Không có bản ghi nào được cập nhật.");
            }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(UserChangePassword userChangePassword)
        {
            var res = await _service.ChangePassword(userChangePassword);
            if (res)
            {
                return Ok(res);

            }
            else
            {
                throw new MISAException(System.Net.HttpStatusCode.InternalServerError, "Không thể thay đổi mật khẩu của bạn, vui lòng thử lại sau hoặc liên hệ Quản trị viên để được trợ giúp.");
            }
        }

    }
}
