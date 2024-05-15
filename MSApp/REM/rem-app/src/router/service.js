const ServiceIndex = () => import("../views/service/ServiceIndex.vue");
const ServiceList = () => import("../views/service/ServiceList.vue");
const ServiceDetail = () => import("../views/service/ServiceItemDetail.vue");
const ServiceGroupList = () => import("../views/service/ServiceGroupList.vue");
const ServiceGroupDetail = () => import("../views/service/ServiceGroupItemDetail.vue");
const serviceRouter = {
    path: "/services",
    name: "ServiceRouter",
    meta: { title: "Dịch vụ" },
    redirect: { name: "ServiceListRouter" },
    component: ServiceIndex,
    children: [
        {
            path: "list",
            name: "ServiceListRouter",
            components: {
                default: ServiceIndex,
                ServiceRouterView: ServiceList,
            },
            children: [
                {
                    path: "create",
                    name: "ServiceListRouter",
                    components: {
                        default: ServiceIndex,
                        ServiceRouterView: ServiceList,
                        ServiceDetailView: ServiceDetail
                    },
                    props: true,
                },
                {
                    path: ":id",
                    name: "ServiceUpdateRouter",
                    components: {
                        default: ServiceIndex,
                        ServiceRouterView: ServiceList,
                        ServiceDetailView: ServiceDetail
                    },
                    props: true,
                },
            ]
        },
        {
            path: "groups",
            name: "ServiceItemCategoryList",
            meta: { title: "Nhóm hàng hoá" },
            components: {
                default: ServiceIndex,
                ServiceRouterView: ServiceGroupList,
            },
            children: [
                {
                    path: "create",
                    name: "ServiceGroupAddRouter",
                    components: {
                        default: ServiceIndex,
                        ServiceRouterView: ServiceGroupList,
                        ServiceGroupDetailView: ServiceGroupDetail
                    },
                    props: true,
                },
                {
                    path: ":id",
                    name: "ServiceGroupUpdateRouter",
                    components: {
                        default: ServiceIndex,
                        ServiceRouterView: ServiceGroupList,
                        ServiceGroupDetailView: ServiceGroupDetail
                    },
                    props: true,
                },
            ]
        },
    ],
}
export default serviceRouter;
