using MS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    [HasSystemData]
    public class NavbarItem:BaseEntity
    {
        public NavbarItem()
        {
            this.Path = "/";
            this.NavbarSubItems = new HashSet<NavbarItem>();
        }

        [PrimaryKey]
        public Guid NavbarItemId { get; set; }
        public string Path { get; set; }
        public string? Label { get; set; }
        public string? IconFont { get; set; }
        public string? IconCls { get; set; }
        public bool UseIconFont { get; set; }
        public bool Inactive { get; set; }
        public int? Sort { get; set; }
        public bool HasSub { get; set; }

        public Guid? ParentId { get; set; }
        public bool IsSystem { get; set; }

        [MSForeignKey("ParentId", "NavbarItem")]
        public ICollection<NavbarItem> NavbarSubItems { get;set; }
    }
}
