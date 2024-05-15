// Hàng hóa/ Nhóm hàng hoá:
import InventoryItemCategoryList from "../views/inventory/InventoryItemCategoryList.vue";
import InventoryItemDetail from "../views/inventory/InventoryItemDetail.vue";
import InventoryItemList from "../views/inventory/InventoryItemList.vue";
import Index from "../views/inventory/Index.vue";
const inventoryRouter = {
    path: "/inventories",
    name: "InventoryIndex",
    meta: { title: "Hàng hóa" },
    component: Index,
    redirect: { name: "InventoryListRouter" },
    children: [
        {
            path: "list",
            name: "InventoryListRouter",
            components: {
                default: Index,
                InventoryRouterView: InventoryItemList,
            },
            children: [
                {
                    path: "create",
                    name: "InventoryListRouter",
                    components: {
                        default: Index,
                        InventoryRouterView: InventoryItemList,
                        InventoryItemDetailView: InventoryItemDetail
                    },
                    props: true,
                },
                {
                    path: ":id",
                    name: "InventoryUpdateRouter",
                    components: {
                        default: Index,
                        InventoryRouterView: InventoryItemList,
                        InventoryItemDetailView: InventoryItemDetail
                    },
                    props: true,
                },
            ]
        },
        {
            path: "groups",
            name: "InventoryItemCategoryList",
            meta: { title: "Nhóm hàng hoá" },
            components: {
                default: Index,
                InventoryRouterView: InventoryItemCategoryList,
            },
        },
    ],
}
export default inventoryRouter;