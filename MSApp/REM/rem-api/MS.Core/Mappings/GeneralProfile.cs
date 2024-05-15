using AutoMapper;
using MS.Core.Authorization;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Core.Mappings
{
    public class GeneralProfile : Profile
    {
        //IJwtUtils _jwtUtils;
        public GeneralProfile()
        {
            //_jwtUtils = jwtUtils;
            CreateMap<UnitModel, Unit>();
            CreateMap<User, UserRegisterResponse>();
            CreateMap<UserRequest, User>();
            CreateMap<EmployeeInfo, Employee>();
            CreateMap<ExpenditurePlan, ExpenditurePlanResponse>();
            CreateMap<InvoiceSlotDto, Ref>();
            CreateMap<SlotInvoiceDto, SlotInvoice>();
            CreateMap<SlotInvoiceDto, Slot>();
            CreateMap<SlotInvoice, SlotDto>();
            CreateMap<SlotDto, Slot>();
            CreateMap<Slot, SlotDto>();
            CreateMap<UserInfo, User>();
            CreateMap<User, UserInfo>();
            CreateMap<RegisterModel, User>()
                .ForMember(model => model.SecurityStamp, act => act.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(user => user.PhoneNumber, act => act.MapFrom(src => src.PhoneNumber))
                .ForMember(user => user.Organization, act => act.MapFrom(src => src.Organization))
                .ForMember(user => user.OrganizationId, act => act.MapFrom(src => src.OrganizationId));

            CreateMap<RegisterModel, Employee>()
               .ForMember(emp => emp.Mobile, act => act.MapFrom(src => src.PhoneNumber))
               .ForMember(emp => emp.FullName, act => act.MapFrom(src => src.FullName))
               .ForMember(emp => emp.Email, act => act.MapFrom(src => src.Email))
               .ForMember(emp => emp.Gender, act => act.MapFrom(src => src.Gender))
               .ForMember(emp => emp.DateOfBirth, act => act.MapFrom(src => src.DateOfBirth))
               .ForMember(emp => emp.OrganizationId, act => act.MapFrom(src => src.OrganizationId));
            //.ForMember(user => user.PasswordHash, act => act.MapFrom(src => _jwtUtils.HashPassword(src.Password)));

            CreateMap<EmployeeInfo, User>()
                .ForMember(user => user.SecurityStamp, act => act.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(user => user.UserName, act => act.MapFrom(src => src.User.UserName))
                .ForMember(user => user.PhoneNumber, act => act.MapFrom(src => src.Mobile))
                .ForMember(user => user.Email, act => act.MapFrom(src => src.Email))
                .ForMember(user => user.EmployeeId, act => act.MapFrom(src => src.EmployeeId))
                .ForMember(user => user.UserId, act => act.MapFrom(src => src.User.UserId));
        }
    }
}
