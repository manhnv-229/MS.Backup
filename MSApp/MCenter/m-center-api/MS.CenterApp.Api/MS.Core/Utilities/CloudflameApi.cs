using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MS.Core.DTOs;
using MS.Core.Entities;
using MS.Core.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MS.Core.Utilities
{
    public interface ICloudflareApi
    {
        Task<List<CloudflareZone>?> GetZones();
        Task<CloudflareZone?> GetZoneById(string id);
        Task<CloudflareZone?> GetZoneByDomainAsync(string domain);
        Task<List<CloudflareDNSRecord>?> GetListDNSRecords(string zoneId);
        Task AddNewDNSRecord(CloudflareDNSRecord dnsREcord, string zoneId);
        Task<CloudflareDNSRecord?> GetDNSRecordByName(string zoneId, string recordName);
        Task UpdateDNSRecord(string zoneId, CloudflareDNSRecord dnsRecord);
        Task DeleteDNSRecordById(string zoneId, string dnsRecordId);
    }
}
