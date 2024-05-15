using MS.ApplicationCore.Interfaces.Repositories;
using MS.ApplicationCore.Interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MS.ApplicationCore.Interfaces.Services;

namespace MS.ApplicationCore.Quartz
{
    public class SendEmailJobTodayStatistic: IJob
    {
        IEmailService _emailService;
        IOrganizationService _orgService;
        public SendEmailJobTodayStatistic(IEmailService emailService, IOrganizationService orgService)
        {
            _emailService = emailService;
            _orgService = orgService;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                // Code that sends a periodic email to the user (for example)
                // Note: This method must always return a value 
                // This is especially important for trigger listers watching job execution 
                Console.WriteLine("Quartz Today!!!!");

                // Lấy thông tin thống kê các đơn vị ngày hôm nay:
                var date = DateTime.Now;
                _ = _orgService.GetOrgStatisticByDay(date, true);
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
