using System;
using System.Collections.Generic;
using System.Text;

namespace MS.ApplicationCore.MSEnums
{
    /// <summary>
    /// Enum giới tính
    /// </summary>
    /// Created by: NVMANH (12/03/2017)
    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2
    }

    /// <summary>
    /// Tình trạng hóa đơn
    /// </summary>
    public enum InvoiceStatus
    {
        /// <summary>
        /// Chưa thanh toán
        /// </summary>
        NOT_YET = 0, 

        /// <summary>
        /// Đã thanh toán
        /// </summary>
        DONE = 1,

        /// <summary>
        /// Nháp
        /// </summary>
        DRAF = 3,

        /// <summary>
        /// Không cần thành toán
        /// </summary>
        NOT_NEED_PAYMENT = 2,

        /// <summary>
        /// Đã hủy
        /// </summary>
        DELETED = 4,
    }

    public enum WorkStatus
    {
        WORKING = 1,
        STOPPING = 0,
        OTHER = 2
    }

    /// <summary>
    /// Enum tình trạng hôn nhân
    /// </summary>
    public enum MaritalStatus : int
    {
        //Độc thân
        Singler = 0,
        //Đã đính hôn
        Engaged = 1,
        //Đã kết hôn
        Married = 2,
        //Đã ly hôn
        Separated = 3,
        //Đã ly thân
        Divorced = 4,
        //Góa chồng
        Widow = 5,
        //Góa vợ
        Widower = 6
    }

    /// <summary>
    /// Trạng thái của đối tượng
    /// </summary>
    public enum EntityState
    {
        ADD = 1,
        UPDATE = 2,
        DELETE = 3,
    }
    public enum MSRole
    {
        Administrator = 1,
        SuperManager = 5,
        Manager = 10,
        Member = 100,
        NewUser = 999
    }



    /// <summary>
    /// Loại kế hoạch thu/chi
    /// </summary>
    public enum ExpenditurePlanType
    {
        /// <summary>
        /// Kế hoạch thu cho sự kiện
        /// </summary>
        INCREMENT_EVENT = 100,

        /// <summary>
        /// Kế hoạch thu quỹ hàng năm
        /// </summary>
        INCREMENT_ANNUAL = 101,

        /// <summary>
        /// Thu Khác
        /// </summary>
        INCREMENT_OTHER = 102,

        /// <summary>
        /// Kế hoạch chi cho sự kiện
        /// </summary>
        REDURE_EVENT = 200,

        /// <summary>
        /// Chi khác
        /// </summary>
        REDUCE_OTHER = 201

    }


    /// <summary>
    /// Loại khoản thu/ chi
    /// </summary>
    public enum ExpenditureType
    {
        /// <summary>
        /// Thu theo kế hoạch
        /// </summary>
        INCREMENT_PLAN = 1,

        /// <summary>
        /// Thu từ nguồn ủng hộ của các mạnh thường quân
        /// </summary>
        INCREMENT_SUPER_RICH = 2,

        /// <summary>
        /// Mạnh thường quân
        /// </summary>
        INCREMENT_OTHER = 3,

        /// <summary>
        /// Chi theo kế hoạch
        /// </summary>
        REDURE_PLAN = 20,

        /// <summary>
        /// Chi cho đám cưới
        /// </summary>
        REDURE_WEDDING = 21,

        /// <summary>
        /// Chi cho đám hiếu
        /// </summary>
        REDURE_FUNERAL = 22,

        /// <summary>
        /// Chi ốm đau, nghỉ bệnh
        /// </summary>
        REDUCE_MEDICAL = 23,

        /// <summary>
        /// Chi khác
        /// </summary>
        REDUCE_OTHER = 24

    }

    /// <summary>
    /// Loại phiếu
    /// </summary>
    public enum ReceiptType
    {
        /// <summary>
        /// Tăng (Thu)
        /// </summary>
        Income = 1,

        /// <summary>
        /// Giảm (Chi)
        /// </summary>
        Outcome = 2,
    }

    /// <summary>
    /// Loại lọc dữ liệu
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Chứa
        /// </summary>
        Containt = 1,

        /// <summary>
        /// Không chứa
        /// </summary>
        NotContaint = 2,

        /// <summary>
        /// Bắt đầu với
        /// </summary>
        StartWith =3,
        
        /// <summary>
        /// Kết thúc với
        /// </summary>
        EndWith = 4,

        /// <summary>
        /// Bằng
        /// </summary>
        Equal = 5,

        /// <summary>
        /// Không bằng
        /// </summary>
        NotEqual = 6
    }

    /// <summary>
    /// Loại sắp xếp
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// Tăng dần
        /// </summary>
        ASC = 1,

        /// <summary>
        /// Giảm dần
        /// </summary>
        DESC = 2
    }

    /// <summary>
    /// Loại giấy phép
    /// </summary>
    public enum LicenseType
    {
        /// <summary>
        /// Dùng thử
        /// </summary>
        Trial = 1,

        /// <summary>
        /// Chính thức
        /// </summary>
        Genuine = 2
    }

    public enum TimeRange
    {
        Today = 1,
        Yesterday = 2,
        ThisWeek = 3,
        ThisMonth = 4,
        ThisQuarter = 5,
        ThisYear = 6,
        Custom = 7
    }


    public enum VIPOptions
    {
        /// <summary>
        /// 1 Tuần
        /// </summary>
        ONE_WEEK = 7,

        /// <summary>
        /// 2 Tuần
        /// </summary>
        HALF_MONTH = 15,
        /// <summary>
        /// 1 Tháng
        /// </summary>
        ONE_MONTH = 30,

        /// <summary>
        /// 6 tháng
        /// </summary>
        HALF_YEAR = 180,

        /// <summary>
        /// 1 Năm
        /// </summary>
        ONE_YEAR = 365,

        /// <summary>
        /// Vĩnh viễn
        /// </summary>
        NO_LIMITTED = 999,
    }
}
