const SaleIndex = () => import("../views/remsale/Index.vue");
const SaleSlotList = () => import("../views/remsale/SaleSlotList.vue");
const saleRouter = {
  path: "/sales",
  name: "SaleSlotIndexRouter",
  meta: { title: "Quản lý Bán hàng" },
  redirect: "/sales/dashboard",
  component: SaleIndex,
  children: [
    {
      path: "dashboard",
      name: "SaleDashboardRouter",
      meta: { title: "Quản lý Bán hàng" },
      components: {
        default: SaleIndex,
        SaleRouterView: SaleSlotList,
      },
      props: true,
    },
    {
      path: "create",
      name: "SalseSlotListRouter",
      meta: { title: "Quản lý Bán hàng" },
      components: {
        default: SaleIndex,
        SaleRouterView: SaleSlotList,
      },
      props: true,
    },
    {
      path: "branchs",
      name: "SalseSlotListRouter",
      meta: { title: "Quản lý Bán hàng" },
      components: {
        default: SaleIndex,
        SaleRouterView: SaleSlotList,
      },
      props: true,
      children: [
        {
          path: ":branchId",
          name: "SalseSlotListByBranchRouter",
          meta: { title: "Quản lý Bán hàng" },
          components: {
            default: SaleIndex,
            SaleRouterView: SaleSlotList,
          },
          props: true,
        },
      ],
    },
  ],
};

export default saleRouter;
