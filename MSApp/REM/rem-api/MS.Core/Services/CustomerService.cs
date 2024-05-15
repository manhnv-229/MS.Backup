using AutoMapper;
using MS.Core.Entities;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Services
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        public CustomerService(ICustomerRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }

        protected override async Task ValidateObjectCustom(dynamic entity)
        {
            var customer = entity as Customer;
            var mobile = customer?.Mobile;
            if (mobile != null && customer!= null)
            {
               var isDuplicate = await UnitOfWork.Customers.CheckDuplicateValue<Customer>("Mobile", mobile, customer);
                if (isDuplicate)
                {
                    AddErrors("Mobile", "Số điện thoại của khách hàng đã có trong hệ thống.");
                }
            }
        }
    }
}
