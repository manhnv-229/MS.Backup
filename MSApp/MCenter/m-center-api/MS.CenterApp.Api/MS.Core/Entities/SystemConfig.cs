using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class SystemConfig:BaseEntity
    {
        public Guid SystemConfigId { get; set; }
        public string ConfigKey { get; set; }
        public string Name { get; set; }
        public ConfigValueType ConfigValueType { get; set; }

        public string? Value { get; set; }
        public string? JSON_Value { get; set; }
        public string? Description { get; set; }
    }
}
