import { createRouter, createWebHistory } from "vue-router";
const LoginPage = () => import("../views/account/Login.vue");
const RegisterPage = () => import("../views/account/Register.vue");
const AccountIndex = () => import("../views/account/Index.vue");
const Index = () => import("../views/Index.vue");

const TenantIndex = () => import("../views/tenant/Index.vue");
const TenantList = () => import("../views/tenant/TenantList.vue");
const TenantDetail = () => import("../views/tenant/TenantDetail.vue");
const TenantRegister = () => import("../views/tenant/TenantRegister.vue");

const DeployApp = () => import("../views/deploy/DeployApp.vue");
// Thông tin đơn vị:
import OrganizationSettings from "../views/systems/OrganizationSettings.vue";

import store from "../store";
import Enum from "../scripts/enum.js";

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
    name: "HomeRouter",
    component: Index,
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
      Register: RegisterPage,
    },
    beforeEnter: ifNotAuthenticated,
  },
  {
    path: "/settings",
    name: "OrganizationSettings",
    component: OrganizationSettings,
    meta: { title: "Cài đặt" },
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/deploys",
    name: "DeployApp",
    component: DeployApp,
    meta: { title: "Triển khai" },
    beforeEnter: ifAuthenticated,
  },
  {
    path: "/tenants",
    name: "TenantManagementRouter",
    component: TenantIndex,
    meta: { title: "Thuê bao" },
    beforeEnter: ifAuthenticated,
    children: [
      {
        path: "",
        name: "TenantListRouter",
        meta: { title: "Danh sách thuê bao" },
        components: {
          default: TenantIndex,
          TenantRouterView: TenantList
        },
        props: true
      },
      {
        path: "register",
        name: "TenantRegisterRouter",
        meta: { title: "Đăng ký thuê bao mới" },
        components: {
          default: TenantIndex,
          TenantRouterView: TenantRegister
        },
        props: true
      }
    ]
  },
]
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

export default router;
