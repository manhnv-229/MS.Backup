using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Interfaces
{
    public interface IWebServerConfig
    {
        void AddDomain(string domain);
        void RemoveDomain(string domain);
        void RestartNginxServer();
    }
}
