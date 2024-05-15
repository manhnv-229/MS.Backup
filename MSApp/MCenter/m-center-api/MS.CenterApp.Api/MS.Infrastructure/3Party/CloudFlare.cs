using Microsoft.Extensions.Configuration;
using MS.Core.DTOs;
using MS.Core.Exceptions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MS.Core.Utilities;

namespace MS.Infrastructure._3Party
{
    public class CloudFlare: ICloudflareApi
    {
        //Cloudframe
        readonly string _cloudflareClientApi;
        readonly string _cloudflareDNSApiToken;
        readonly IConfiguration _config;
        IHttpClientFactory _httpClientFactory;
        IServiceProvider _serviceProvider;
        private List<string> _errors;

        public CloudFlare(IConfiguration config, IServiceProvider serviceProvider)
        {
            _config = config;
            _cloudflareClientApi = _config["Cloudflare:ClientApi"];
            _cloudflareDNSApiToken = _config["Cloudflare:DNSApiToken"];
            _serviceProvider = serviceProvider;
            _httpClientFactory = serviceProvider.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;
            _errors = new List<string>();
        }

        public async Task<List<CloudflareZone>?> GetZones()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"{_cloudflareClientApi}/zones/";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer  {_cloudflareDNSApiToken}");
            //request.Content = jsonContent
            var res = await httpClient.SendAsync(request);
            var resContent = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
            {
                throw new MSException(resContent);
            }

            if (res.IsSuccessStatusCode && resContent != null)
            {
                var jObject = (JObject)JsonConvert.DeserializeObject(resContent);
                var jToken = jObject["result"];
                var zones = jToken.ToObject<CloudflareZone[]>();
                return zones.ToList();
            }
            else
            {
                return null;
            }
        }

        public async Task<CloudflareZone?> GetZoneById(string id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"{_cloudflareClientApi}/zones/{id}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer  {_cloudflareDNSApiToken}");
            //request.Content = jsonContent
            var res = await httpClient.SendAsync(request);
            var resContent = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
            {
                throw new MSException(resContent);
            }
            if (res.IsSuccessStatusCode && resContent != null)
            {
                var jObject = (JObject)JsonConvert.DeserializeObject(resContent);
                var jToken = jObject["result"];
                var zone = jToken.ToObject<CloudflareZone>();
                return zone;
            }
            else
            {
                return null;
            }
        }

        public async Task<CloudflareZone?> GetZoneByDomainAsync(string domain)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"{_cloudflareClientApi}/zones/";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer  {_cloudflareDNSApiToken}");
            //request.Content = jsonContent
            var res = await httpClient.SendAsync(request);
            var resContent = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
            {
                throw new MSException(resContent);
            }

            if (res.IsSuccessStatusCode && resContent != null)
            {
                var jObject = (JObject)JsonConvert.DeserializeObject(resContent);
                var jToken = jObject["result"];
                var zones = jToken.ToObject<CloudflareZone[]>();
                return zones.Where(zone => zone.name == domain).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }


        public async Task<List<CloudflareDNSRecord>?> GetListDNSRecords(string zoneId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var zoneDNSUrl = $"{_cloudflareClientApi}/zones/{zoneId}/dns_records";
            var request = new HttpRequestMessage(HttpMethod.Get, zoneDNSUrl);
            request.Headers.Add("Authorization", $"Bearer  {_cloudflareDNSApiToken}");
            var res = await httpClient.SendAsync(request);

            var resContent = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode && resContent != null)
            {
                var result = (JObject)JsonConvert.DeserializeObject(resContent);
                var jToken = result["result"];
                var obj = jToken.ToObject<CloudflareDNSRecord[]>();
                //var obj = JsonConvert.DeserializeObject<List<CloudflareDNSRecord>?>(jsonString);
                return obj.ToList();
            }
            else
            {
                return null;
            }

        }

        public async Task AddNewDNSRecord(CloudflareDNSRecord dnsREcord, string zoneId)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var zoneUrl = $"https://api.cloudflare.com/client/v4/zones/{zoneId}/dns_records";
            var request = new HttpRequestMessage(HttpMethod.Post, zoneUrl);
            request.Headers.Add("Authorization", $"Bearer  {_cloudflareDNSApiToken}");

            string strJson = JsonConvert.SerializeObject(dnsREcord);
            var content = new StringContent(strJson, Encoding.UTF8, "application/json");
            request.Content = content;
            var res = await httpClient.SendAsync(request);
            var resContent = await res.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<CloudflareServiceResult>(resContent);
            Console.WriteLine(result);

            if (result == null || result?.success == false)
            {
                foreach (var error in result.errors)
                {
                    var code = error.code;
                    _errors.Add($"{error.code}: {error.message}");
                    if (error.error_chain.Count > 0)
                    {
                        foreach (var item in error.error_chain)
                        {
                            _errors.Add($"{item.code}: {item.message}");
                        }
                    }
                }
            }
            if (_errors.Count > 0)
            {
                Console.WriteLine("config dns fail!");
                var dicErrors = new Dictionary<string, List<string>>();
                dicErrors.Add("Cloudflare", _errors);
                throw new MSException(System.Net.HttpStatusCode.BadRequest, dicErrors);
            }
            Console.WriteLine("config dns completed!");
        }

        public async Task<CloudflareDNSRecord?> GetDNSRecordByName(string zoneId, string recordName)
        {
            var dnsRecords = await GetListDNSRecords(zoneId);
            var recordExits = dnsRecords?.Where(r => r.name == recordName).FirstOrDefault();
            return recordExits;
        }

        public async Task UpdateDNSRecord(string zoneId, CloudflareDNSRecord dnsREcord)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var zoneDNSUrl = $"{_cloudflareClientApi}/zones/{zoneId}/dns_records/{dnsREcord.id}";
            var request = new HttpRequestMessage(HttpMethod.Patch, zoneDNSUrl);

            request.Headers.Add("Authorization", $"Bearer  {_cloudflareDNSApiToken}");
            string strJson = JsonConvert.SerializeObject(dnsREcord);
            var content = new StringContent(strJson, Encoding.UTF8, "application/json");
            request.Content = content;

            var res = await httpClient.SendAsync(request);
            var resContent = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode && resContent != null)
            {
                var result = (JObject)JsonConvert.DeserializeObject(resContent);
                var jToken = result["success"];
                var obj = jToken.ToString(); ;
                Console.WriteLine($"Update record success!  ---  {obj}");
            }
            else
            {
                Console.WriteLine("Update record fail!");
            }
        }

        public async Task DeleteDNSRecordById(string zoneId, string dnsRecordId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var zoneDNSUrl = $"{_cloudflareClientApi}/zones/{zoneId}/dns_records/{dnsRecordId}";
            var request = new HttpRequestMessage(HttpMethod.Delete, zoneDNSUrl);

            request.Headers.Add("Authorization", $"Bearer  {_cloudflareDNSApiToken}");

            var res = await httpClient.SendAsync(request);
            var resContent = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode && resContent != null)
            {
                var result = (JObject)JsonConvert.DeserializeObject(resContent);
                var jToken = result["result"];
                var obj = jToken.ToString(); ;
                Console.WriteLine($"Delete record success!  ---  {obj}");
            }
            else
            {
                Console.WriteLine("Delete record fail!");
            }
        }
    }
}
