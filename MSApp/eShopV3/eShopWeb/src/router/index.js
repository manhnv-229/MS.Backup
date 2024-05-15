import { createRouter, createWebHistory } from "vue-router";

import store from "@/store";
import Enum from "@/scripts/enum.js";

// Quản lý:
import AdminIndex from "../views/admin/AdminIndex.vue";
import AdminOrganizationDetail from "../views/admin/OrganizationDetail.vue";

// Nhân viên
import EmployeeList from "../views/employee/EmployeeList.vue";

// Hóa đơn
import InvoiceIndex from "../views/invoice/InvoiceIndex.vue";
import InvoiceDetail from "../views/invoice/InvoiceDetail.vue";
import InvoiceList from "../views/invoice/InvoiceList.vue";
import InvoicePrintFromMobile from "../views/invoice/InvoicePrintFromMobile.vue";

// Bán hàng/ Hóa đơn
import RefIndex from "../views/sale/RefIndex.vue";
import RefDetail from "../views/sale/RefDetail.vue";
import RefList from "../views/sale/RefList.vue";
// import InvoicePrintFromMobile from "../views/invoice/InvoicePrintFromMobile.vue";

//  Khách hàng
import CustomerList from "../views/customers/CustomerList.vue";
import CustomerGroupList from "../views/customer-groups/Index.vue";

//  Nhà phân phối
import DistributorList from "../views/distributor/DistributorList.vue";

// Đơn vị tính:
import UnitList from "../views/units/UnitList.vue";
import PositionList from "../views/positions/PositionList.vue";

// Kho:
import stockRouter from "./stock";

// Chi nhánh:
import BranchList from "../views/branch/BranchList.vue";

// Nhóm sản phẩm/ dịch vụ:
import GroupProductList from "../views/product/GroupProductList.vue";
import ProductDetail from "../views/product/ProductDetail.vue";
import ProductList from "../views/product/ProductList.vue";

// Nhóm hàng hoá:
import InventoryItemCategoryList from "../views/inventory/InventoryItemCategoryList.vue";
import InventoryItemDetail from "../views/inventory/InventoryItemDetail.vue";
import InventoryItemList from "../views/inventory/InventoryItemList.vue";

// Đăng ký, đăng nhập, Tài khoản
import LoginPage from "../views/account/Login.vue";
import Register from "../views/account/Register.vue";
import AccountInfo from "../views/account/AccountInfo.vue";
import HomePage from "../views/Index.vue";

// Thông tin đơn vị:
import OrganizationSettings from "../views/systems/OrganizationSettings.vue";

// Báo cáo: 
import reportRouter from "./report";
const ifNotAuthenticated = (to, from, next) => {
  if (!store.getters.isAuthenticated) {
    next();
    return;
  }
  next("/");
};

const ifAuthenticated = (to, from, next) => {
  if (store.getters.isAuthenticated) {
    next();
    return;
  }
  localStorage.clear();
  next("/login");
};

const isSuperManagement = (to, from, next) => {
  const role = store.getters.role;
  if (store.getters.isAuthenticated && role <= Enum.Role.SuperManagement) {
    next();
    return;
  }
  next("/invoices");
};

const routes = [
  {
    path: "/",
    name: "HomePage",
    component: HomePage,
    meta: { title: "" },
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/login",
    name: "LoginPage",
    components: {
      LoginPage: LoginPage,
    },
    beforeEnter: ifNotAuthenticated,
  },
  {
    path: "/register",
    name: "Register",
    components: {
      Register: Register,
    },
    beforeEnter: ifNotAuthenticated,
  },
  {
    path: "/admin",
    name: "AdminIndex",
    component: AdminIndex,
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/admin/organizations/create",
    name: "AdminOrganizationDetailCreate",
    component: AdminOrganizationDetail,
    props: false,
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/admin/organizations/:id",
    name: "AdminOrganizationDetailUpdate",
    component: AdminOrganizationDetail,
    props: true,
    meta: { title: "Thông tin đơn vị" },
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/settings",
    name: "OrganizationSettings",
    component: OrganizationSettings,
    meta: { title: "Cài đặt" },
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/branchs",
    name: "BranchList",
    meta: { title: "Chi nhánh" },
    component: BranchList,
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/employees",
    name: "EmployeeList",
    meta: { title: "Nhân viên" },
    component: EmployeeList,
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/customers",
    name: "CustomerList",
    meta: { title: "Khách hàng" },
    component: CustomerList,
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/customer-groups",
    name: "CustomerGroup",
    meta: { title: "Nhóm khách hàng" },
    component: CustomerGroupList,
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/distributors",
    name: "DistributorRV",
    meta: { title: "Nhà phân phối" },
    component: DistributorList,
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/units",
    name: "UnitList",
    meta: { title: "Đơn vị" },
    component: UnitList,
    beforeEnter: ifAuthenticated,
  },
  {
    ...stockRouter,
    beforeEnter: isSuperManagement
  },
  {
    path: "/positions",
    name: "PositionList",
    meta: { title: "Vị trí" },
    component: PositionList,
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/group-products",
    name: "GroupProductList",
    meta: { title: "Nhóm sản phẩm" },
    component: GroupProductList,
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/products",
    name: "ProductList",
    meta: { title: "Sản phẩm/Dịch vụ" },
    component: ProductList,
    beforeEnter: ifAuthenticated,
    children: [
      {
        path: "create",
        components: { ProductDetailView: ProductDetail },
      },
      {
        path: ":id",
        components: { ProductDetailView: ProductDetail },
        props: true,
      },
    ],
  },
  {
    path: "/inventoryItemCategories",
    name: "InventoryItemCategoryList",
    meta: { title: "Nhóm hàng hoá" },
    component: InventoryItemCategoryList,
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/inventories",
    name: "InventoryItemList",
    meta: { title: "Hàng hóa" },
    component: InventoryItemList,
    beforeEnter: ifAuthenticated,
    children: [
      {
        path: ":id",
        components: { InventoryItemDetailView: InventoryItemDetail },
        props: true,
      },
      {
        path: "create",
        name:"InvoiceCreateRouter",
        components: { InventoryItemDetailView: InventoryItemDetail },
      },
    ],
  },
  {
    path: "/invoices",
    name: "InvoiceList",
    meta: { title: "Hoá đơn" },
    component: InvoiceIndex,
    beforeEnter: ifAuthenticated,
    children: [
      {
        path: "",
        name: "InvoiceListHome",
        components: { default: InvoiceIndex, InvoiceView: InvoiceList },
      },
      {
        path: "create",
        name:"InvoiceCreateRouter",
        components: { default: InvoiceIndex, InvoiceView: InvoiceDetail },
      },
      {
        path: ":id",
        components: { default: InvoiceIndex, InvoiceView: InvoiceDetail },
        props: true,
      },
      // {
      //     path: "create",
      //     components: { InvoiceDetailView: InvoiceDetail },
      // },
      // {
      //     path: ":id",
      //     components: { InvoiceDetailView: InvoiceDetail },
      //     props: true,
      // },
      {
        path: "printer/:id/:device",
        components: { InvoicePrintFromMobile: InvoicePrintFromMobile },
        props: true,
      },
    ],
  },

  {
    path: "/refs",
    name: "RefListRouter",
    meta: { title: "Hoá đơn" },
    component: RefIndex,
    beforeEnter: ifAuthenticated,
    children: [
      {
        path: "",
        name: "RefListHomeRouter",
        components: { default: RefIndex, RefView: RefList },
      },
      {
        path: "create",
        meta: { title: "Bán hàng" },
        components: { default: RefIndex, RefView: RefDetail },
      },
      {
        path: ":id",
        components: { default: RefIndex, RefView: RefDetail },
        props: true,
      },
      // {
      //     path: "create",
      //     components: { InvoiceDetailView: InvoiceDetail },
      // },
      // {
      //     path: ":id",
      //     components: { InvoiceDetailView: InvoiceDetail },
      //     props: true,
      // },
      {
        path: "printer/:id/:device",
        components: { InvoicePrintFromMobile: InvoicePrintFromMobile },
        props: true,
      },
    ],
  },
  {
    ...reportRouter,
    beforeEnter: isSuperManagement
  },
  {
    path: "/account",
    name: "AccountInfo",
    meta: { title: "Tài khoản" },
    component: AccountInfo,
    beforeEnter: ifAuthenticated,
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
