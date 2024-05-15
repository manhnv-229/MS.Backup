using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.Entities
{
    public class Note:BaseEntity
    {
        public Guid NoteId { get; set; }
        public DateTime? NoteDateTime { get; set; }
        public string? Content { get; set; }
        public Guid? UserId { get; set; }
        public string? ThemeColor { get; set; }
        public int? OrderZIndex { get; set; }
    }
}
