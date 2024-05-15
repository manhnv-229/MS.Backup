using Microsoft.AspNetCore.Mvc.ModelBinding;
using MS.Core.Entities;
using MS.Core.Entities.Auth;
using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    [DataTableName("Tenant")]
    public class TenantRegister : Tenant
    {
        public TenantRegister()
        {
            Organization = new Organization();
            Catalog = new Catalog();
            TenantUser = new TenantUser();
            License = new MSLicense();
        }
        public MSLicense License { get; set; }
        public bool AddInventoryData { get; set; }
    }
}
