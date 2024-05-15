using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Core.Entities.Auth;
using Ms.eShop.Api.Controllers;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Services;
using MS.Core.Interfaces.Repositories;

namespace MS.eShop.Api.Controllers
{
    public class SettingsController : BaseController<Setting>
    {
        ISettingService _settingService;
        ISettingRepository _settingRepository;
        public SettingsController(ISettingService settingService, ISettingRepository settingRepository) : base(settingRepository, settingService)
        {
            _settingService = settingService;
            this._settingRepository = settingRepository;
        }
    }
}
