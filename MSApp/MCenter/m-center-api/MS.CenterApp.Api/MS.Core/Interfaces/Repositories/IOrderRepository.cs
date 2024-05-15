﻿using MS.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<InvoiceSlotDto> GetOrderDetailById(Guid refId);
    }
}
