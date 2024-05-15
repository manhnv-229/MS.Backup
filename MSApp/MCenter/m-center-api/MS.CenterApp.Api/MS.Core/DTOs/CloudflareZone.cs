using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class CloudflareDNSRecord
    {
        public CloudflareDNSRecord()
        {
            proxiable = true;   
            proxied = true;
        }
        public string id { get; set; }
        public string zone_id { get; set; }
        public string zone_name { get; set; }
        public string content { get; set; }
        public string name { get; set; }
        public bool proxiable { get; set; }
        public bool proxied { get; set; }
        public string type { get; set; }
        public string comment { get; set; }
        public List<string> tags { get; set; }
        public int ttl { get; set; }
        public bool locked { get; set; }
    }
    public class CloudflareServiceResult
    {
        public dynamic result { get; set; }
        public List<CloudflareError> errors { get; set; }
        public object messages { get; set; }
        public bool success { get; set; }
        public cloudflare_result_info result_info { get; set; }
    }

    public class CloudflareError
    {
        public CloudflareError()
        {
            error_chain = new List<CloudflareError>();
        }
        public int? code { get; set; }
        public string message { get; set; }
        public List<CloudflareError> error_chain { get;set; }
    }

    public class CloudflareZone
    {
        public string zone_id { get; set; }
        public string zone_name { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public bool paused { get; set; }
        public string type { get; set; }
        public int development_mode { get; set; }
        public List<string> name_servers { get; set; }
        public List<string> original_name_servers { get; set; }
        public string? original_registrar { get; set; }
        public string? original_dnshost { get; set; }
        public DateTime? modified_on { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? activated_on { get; set; }
        public CloudflareMeta meta { get; set; }
        public CloudflareOwner owner { get; set; }
        public CloudflareAccount account { get; set; }
        public CloudflareTenant tenant { get; set; }
        public CloudflareTenantUnit tenant_unit { get; set; }

        public List<string> permissions { get; set; }
        public CloudflarePlan plan { get; set; }
    }

    public class cloudflare_result_info
    {
        public int page { get; set; }
        public int per_page { get; set; }
        public int total_pages { get; set; }
        public int count { get; set; }

        public int total_count { get; set; }
    }

    public class CloudflareMeta
    {
        public int step { get; set; }
        public int custom_certificate_quota { get; set; }
        public int page_rule_quota { get; set; }
        public bool phishing_detected { get; set; }
        public bool multiple_railguns_allowed { get; set; }
    }

    public class CloudflareOwner
    {
        public string? id { get; set; }
        public string? type { get; set; }
        public string? email { get; set; }
    }

    public class CloudflareAccount
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class CloudflareTenant
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class CloudflareTenantUnit
    {
        public string? id { get; set; }
    }

    public class CloudflarePlan
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public double price { get; set; }
        public string? currency { get; set; }
        public string? frequency { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public string? legacy_id { get; set; }
        public bool legacy_discount { get; set; }
        public bool externally_managed { get; set; }
    }
}
