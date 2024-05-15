using MS.Core.MSEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Setting : BaseEntity
    {
        public Guid SettingId { get; set; }
        public string SettingName { get; set; }
        public string SettingKey { get; set; }
        public SettingValueType SettingValueType { get; set; }
        public Guid? UserId { get; set; }
        public bool IsSystem { get; set; }

        public string? SettingValue { get; set; }

    }
}
