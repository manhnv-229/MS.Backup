using MS.ApplicationCore.Entities.Auth;
using MS.ApplicationCore.Helpers;
using MS.ApplicationCore.MSEnums;
using MS.ApplicationCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Entities
{
    /// <summary>
    /// Nhân viên
    /// </summary>
    public class Employee : BaseEntity
    {
        public Guid EmployeeId { get; set; }

        [LabelText("Mã nhân viên")]
        public string EmployeeCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required("Họ và tên không được phép để trống.")]
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }

        [Required("Số điện thoại không được phép để trống.")]
        [NotDuplicate]
        [LabelText("Số điện thoại")]
        public string Mobile { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankBranchName { get; set; }

        [EmailValid]
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? IdentityNumber { get; set; }
        public DateTime? JoinDate { get; set; }
        public decimal? Salary { get; set; }
        public WorkStatus? WorkStatus { get; set; }
        public string? Note { get; set; }
        public Guid? PositionId { get; set; }
        public string? AvatarLink { get; set; }

        [NotMapQuery]
        public string? AvatarFullPath
        {
            get
            {
                var random = new Random();
                if (AvatarLink != null)
                    return String.Format("{0}/{1}?v={2}", CommonConst.ServerFileUrl, AvatarLink, random.Next(1, 999));
                return null;
            }
        }

    }
}
