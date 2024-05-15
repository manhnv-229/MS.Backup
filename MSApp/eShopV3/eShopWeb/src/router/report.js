// Báo cáo:
const ReportList = () => import("../views/report/ReportList.vue");
const ReportRevenue = () => import("../views/report/RevenueReport.vue");
const ReportRevenueByEmployee = () =>
  import("../views/report/RevunueReportByEmployee.vue");
const ReportRevenueByCustomer = () =>
  import("../views/report/RevenueReportByCustomer.vue");
const ReportExpense = () => import("../views/report/ReportExpense.vue");

const reportRouter = {
  path: "/reports",
  name: "ReportList",
  meta: { title: "Báo cáo" },
  component: ReportList,
  redirect: "/reports/revenue",
  children: [
    {
      path: "",
      name: "ReportRevenueRouter",
      meta: { title: "Báo cáo doanh thu" },
      components: {
        default: ReportList,
        ReportRouterView: ReportRevenue,
      },
      alias: ["/revenue"],
      prop: true,
    },
    {
      path: "revenue",
      name: "ReportRevenueRouter",
      meta: { title: "Báo cáo doanh thu" },
      components: {
        default: ReportList,
        ReportRouterView: ReportRevenue,
      },
      prop: true,
    },
    {
      path: "revenue-employee",
      name: "ReportRevenueByEmployeeRouter",
      meta: { title: "Báo cáo doanh thu theo nhân viên" },
      components: {
        default: ReportList,
        ReportRouterView: ReportRevenueByEmployee,
      },
      prop: true,
    },
    {
      path: "revenue-customer",
      name: "ReportRevenueByCustomerRouter",
      meta: { title: "Báo cáo doanh thu theo khách hàng" },
      components: {
        default: ReportList,
        ReportRouterView: ReportRevenueByCustomer,
      },
      prop: true,
    },
    {
      path: "expenses",
      name: "ReportExpense",
      meta: { title: "Thống kê chi phí" },
      components: {
        default: ReportList,
        ReportRouterView: ReportExpense,
      },
      prop: true,
    },
  ],
};

export default reportRouter;
