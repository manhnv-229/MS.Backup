// Báo cáo:
const StockList = () => import("../views/stock/StockList.vue");
const StockIndex = () => import("../views/stock/StockIndex.vue");
const StockReceipt = () => import("../views/stock/StockReceipt.vue");
const StockExport = () => import("../views/stock/StockExport.vue");
const StockReceiptDetail = () => import("../views/stock/StockReceiptDetail.vue");

const stockRouter = {
  path: "/stocks",
  name: "StockRouter",
  meta: { title: "Quản lý Kho" },
  redirect: "/stocks/dashboard",
  component: StockIndex,
  //   redirect: "/reports/revenue",
  children: [
    {
      path: "dashboard",
      name: "StockListRouter",
      meta: { title: "Quản lý Kho" },
      components: {
        default: StockIndex,
        StockRouterView: StockList,
      },
      alias: ["/dashboard"],
      prop: true,
    },
    {
      path: "receipt",
      name: "StockReceiptRouter",
      meta: { title: "Nhập kho" },
      components: {
        default: StockIndex,
        StockRouterView: StockReceipt,
      },
      prop: true,
      children: [
        {
          path: "create",
          meta: { title: "Phiếu nhập kho" },
          components: { default: StockIndex,  StockRouterView: StockReceipt,StockReceiptRV: StockReceiptDetail},
        },
        {
          path: ":id",
          components: { default: StockIndex,  StockRouterView: StockReceipt,StockReceiptRV: StockReceiptDetail },
          props: true,
        },
      ],
    },
    {
      path: "export",
      name: "StockExportRouter",
      meta: { title: "Xuất kho" },
      components: {
        default: StockIndex,
        StockRouterView: StockExport,
      },
      prop: true,
    },
  ],
};

export default stockRouter;
