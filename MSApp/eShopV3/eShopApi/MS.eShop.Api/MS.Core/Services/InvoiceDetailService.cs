﻿using AutoMapper;
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
    public class InvoiceDetailService : BaseService<InvoiceDetail>, IInvoiceDetailService
    {
        public InvoiceDetailService(IInvoiceDetailRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
        {
        }
    }
}
