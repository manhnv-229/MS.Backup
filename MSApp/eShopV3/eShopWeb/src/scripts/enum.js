var Enum = {
  FormMode: {
    ADD: 1,
    UPDATE: 2,
    VIEW: 3,
  },
  MsgType: {
    Success: 1,
    Info: 2,
    Error: 3,
    Confirm: 4,
    Warning: 5,
    Question: 6,
  },
  Role: {
    Administrator: 1,
    SuperManagement: 5,
    Management: 10,
    Employee: 100,
    NewUser: 999,
  },
  MessageGroupType: {
    Private: 1,
    Group: 2,
    Public: 3,
  },
  KeyCode: {
    ENTER: 13,
  },

  ExpenditureType: {
    INCREMENT_PLAN: 1,
    INCREMENT_SUPER_RICH: 2,
    INCREMENT_OTHER: 3,
    REDURE_PLAN: 20,
    REDURE_WEDDING: 21,
    REDURE_FUNERAL: 22,
    REDUCE_MEDICAL: 23,
    REDUCE_OTHER: 24,
  },
  /**
   * Kế hoạch thu chi
   */
  ExpenditurePlanType: {
    // Thu cho sự kiện
    INCREMENT_EVENT: 100,
    // Thu quỹ hàng năm
    INCREMENT_ANNUAL: 101,
    // Thu khác
    INCREMENT_OTHER: 102,
    // Chi cho sự kiện
    REDURE_EVENT: 200,
    // Chi khác
    REDUCE_OTHER: 201,
  },
  ReceiptType: {
    Income: 1,
    Outcome: 2,
  },
  OptionExpenditurePlanType: {
    ForPlan: 1,
    ForOther: 2,
  },
  EntityState: {
    ADD: 1,
    UPDATE: 2,
    DELETE: 3,
  },
  TimeRanges: {
    Today: 1,
    Yesterday: 2,
    ThisWeek: 3,
    ThisMonth: 4,
    ThisQuarter: 5,
    ThisYear: 6,
    Custom: 7,
  },
  RefType: {
    IN_WAREHOUSE: 2095,
    OUT_WAREHOUSE: 2906,
    SALE:550
  },
  PaymentType: {
    CASH: 0,
    TRANSFER: 1,
    VISA_DEBIT: 2,
    MASTER_DEBIT: 3,
    American_Express: 4,
    JCB: 5,
  },

  DebitType: {
    Receivable: 0, //Nợ phải thu
    Payable: 1, // Nợ phải trả
  },

  PaymentStatus: {
    PAID: 1,
    PENDING: 0,
  },
  SettingValueType:{
    BOOLEAN:1,
    INTEGER:2,
    STRING:3,
    JSON:4
  },
  ReportType:{
    REVENUE:1,
    EXPENSTS: 2,
    PROFIT: 3
  }
};
export default Enum;
