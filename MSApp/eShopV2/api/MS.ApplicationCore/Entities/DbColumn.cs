using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Entities
{
    public class DbColumn
    {
        public Int64? ColumnId { get; set; }
        public string ColumnName { get; set; }
        public string? ColumnDataType { get; set; }
        public Int64? MaxLength { get; set; }
        public decimal DataPrecision { get; set; }
        public string IsNullable { get; set; }
        public string? ColumnDefault { get; set; }
        public string? DatabseName { get; set; }
        public string ColumnKey { get; set; }
    }
}
