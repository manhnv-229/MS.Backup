using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Quartz
{
    public interface ISendEmailJob: IJob
    {
        Task Execute(IJobExecutionContext context);
    }
}
