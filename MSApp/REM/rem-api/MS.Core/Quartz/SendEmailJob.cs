//using Microsoft.AspNetCore.Cors.Infrastructure;
using MS.Core.Interfaces;
using MS.Core.Interfaces.Repositories;
using MS.Core.Interfaces.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Quartz
{
    public class SendEmailJob : IJob
    {
        IEmailService _emailService;
        IOrganizationService _organizationService;
        public SendEmailJob(IEmailService emailService, IOrganizationRepository organizationRepository, IOrganizationService organizationService)
        {
            _emailService = emailService;
            _organizationService = organizationService;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                // Code that sends a periodic email to the user (for example)
                // Note: This method must always return a value 
                // This is especially important for trigger listers watching job execution 
                Console.WriteLine("Quartz Yesterday!!!!");
                var date = DateTime.Now.AddDays(-1);
                // Lấy thông tin thống kê các đơn vị ngày trước:
                _ = _organizationService.GetOrgStatisticByDay(date, true);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
    }
}
