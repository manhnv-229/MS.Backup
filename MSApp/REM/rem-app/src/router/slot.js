// Bàn/Chỗ sử dụng dịch vụ:
const SlotGroupList = () => import("../views/slots/SlotGroupList.vue");
const SlotGroupDetail = () => import("../views/slots/SlotGroupDetail.vue");
const SlotList = () => import("../views/slots/SlotList.vue");
const SlotIndex = () => import("../views/slots/SlotIndex.vue");

const slotRouter = {
  path: "/slots",
  name: "SlotRouter",
  meta: { title: "Quản lý Bàn/Phòngn" },
  redirect: "/slots/dashboard",
  component: SlotIndex,
  //   redirect: "/reports/revenue",
  children: [
    {
      path: "dashboard",
      name: "SlotListRouter",
      meta: { title: "Quản lý Bàn/Phòng" },
      components: {
        default: SlotIndex,
        SlotRouterView: SlotList,
      },
      alias: ["/dashboard"],
      prop: true,
    },
    {
      path: "group",
      name: "SlotGroupRouter",
      meta: { title: "Nhóm Phòng/Bàn" },
      components: {
        default: SlotIndex,
        SlotRouterView: SlotGroupList,
      },
      prop: true,
      children: [
        {
          path: "create",
          meta: { title: "Thông tin Bàn/Phòng" },
          components: { default: SlotIndex,SlotRouterView:SlotGroupList,SlotReceiptRV: SlotGroupDetail},
        },
        {
          path: ":id",
          components: { default: SlotIndex,SlotRouterView:SlotGroupList,SlotReceiptRV: SlotGroupDetail },
          props: true,
        },
      ],
    }
  ],
};

export default slotRouter;
