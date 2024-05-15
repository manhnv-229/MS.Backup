import { createRouter, createWebHistory } from 'vue-router'

import store from '@/store'
import Enum from '@/scripts/enum.js'

// Quản lý:
import AdminIndex from '../views/admin/AdminIndex.vue'
import AdminOrganizationDetail from '../views/admin/OrganizationDetail.vue'

// Nhân viên
import EmployeeList from '../views/employee/EmployeeList.vue'

// Hóa đơn
import InvoiceIndex from '../views/invoice/InvoiceIndex.vue'
import InvoiceDetail from '../views/invoice/InvoiceDetail.vue'
import InvoiceList from '../views/invoice/InvoiceList.vue'
import InvoicePrintFromMobile from '../views/invoice/InvoicePrintFromMobile.vue'

//  Khách hàng
import CustomerList from '../views/customers/CustomerList.vue'
import CustomerGroupList from '../views/customer-groups/Index.vue'

// Đơn vị tính:
import UnitList from '../views/units/UnitList.vue'
import PositionList from '../views/positions/PositionList.vue'

// Nhóm sản phẩm/ dịch vụ:
import GroupProductList from '../views/product/GroupProductList.vue'
import ProductDetail from '../views/product/ProductDetail.vue'
import ProductList from '../views/product/ProductList.vue'

// Đăng ký, đăng nhập, Tài khoản
import LoginPage from '../views/account/Login.vue'
import Register from '../views/account/Register.vue'
import AccountInfo from '../views/account/AccountInfo.vue'
import HomePage from '../views/Index.vue'

// Thông tin đơn vị:
import OrganizationSettings from '../views/systems/OrganizationSettings.vue'

// Báo cáo:
import ReportList from '../views/report/ReportList.vue'

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


const routes = [{
    path: '/',
    name: 'HomePage',
    component: HomePage,
    beforeEnter: ifAuthenticated
},
{
    path: '/login',
    name: 'LoginPage',
    components: {
        LoginPage: LoginPage
    },
    beforeEnter: ifNotAuthenticated
},
{
    path: '/register',
    name: 'Register',
    components: {
        Register: Register
    },
    beforeEnter: ifNotAuthenticated
},
{
    path: '/admin',
    name: 'AdminIndex',
    component: AdminIndex,
    beforeEnter: ifAuthenticated
},
{
    path: '/admin/organizations/create',
    name: 'AdminOrganizationDetailCreate',
    component: AdminOrganizationDetail,
    props: false,
    beforeEnter: ifAuthenticated
},
{
    path: '/admin/organizations/:id',
    name: 'AdminOrganizationDetailUpdate',
    component: AdminOrganizationDetail,
    props: true,
    beforeEnter: ifAuthenticated
},
{
    path: '/settings',
    name: 'OrganizationSettings',
    component: OrganizationSettings,
    beforeEnter: ifAuthenticated
},
{
    path: '/employees',
    name: 'EmployeeList',
    component: EmployeeList,
    beforeEnter: ifAuthenticated
},
{
    path: '/customers',
    name: 'CustomerList',
    component: CustomerList,
    beforeEnter: ifAuthenticated
},
{
    path: '/customer-groups',
    name: 'CustomerGroup',
    component: CustomerGroupList,
    beforeEnter: ifAuthenticated
},
{
    path: '/units',
    name: 'UnitList',
    component: UnitList,
    beforeEnter: ifAuthenticated
},
{
    path: '/positions',
    name: 'PositionList',
    component: PositionList,
    beforeEnter: ifAuthenticated
},
{
    path: '/group-products',
    name: 'GroupProductList',
    component: GroupProductList,
    beforeEnter: ifAuthenticated
},
{
    path: '/products',
    name: 'ProductList',
    component: ProductList,
    beforeEnter: ifAuthenticated,
    children: [{
        path: "create",
        components: { ProductDetailView: ProductDetail },
    },
    {
        path: ":id",
        components: { ProductDetailView: ProductDetail },
        props: true
    }
    ]
},
{
    path: '/invoices',
    name: 'InvoiceList',
    component: InvoiceIndex,
    beforeEnter: ifAuthenticated,
    children: [{
        path: "",
        components: { default: InvoiceIndex, InvoiceView: InvoiceList },
    },
    {
        path: "create",
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
        props: true
    }
    ]
},
{
    path: '/reports',
    name: 'ReportList',
    beforeEnter: isSuperManagement,
    component: ReportList
},
{
    path: '/account',
    name: 'AccountInfo',
    component: AccountInfo,
    beforeEnter: ifAuthenticated
},

]

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes
})

export default router