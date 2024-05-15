<template>
  <!-- <div class="page-title animate-charcter" style="">
    <div>{{ organizationInfo.OrganizationName }}</div>
    <div>- {{ organizationInfo.Slogan }} -</div>
  </div> -->
  <nav class="navbar">
    <div class="navbar-container">
      <div class="navbar-list">
        <a id="nav-home" class="navbar-item">
          <i class="icofont-navigation-menu"></i>
          <span>{{ organizationInfo.OrganizationName }}</span>
        </a>
        <!-- <router-link to="/invoices" class="navbar-item">
          <span class="navbar-item__text"
            ><i class="icofont-sale-discount"></i
          ></span>
          <span class="item__text-label">Bán hàng</span>
        </router-link> -->
        <router-link to="/" class="navbar-item">
          <span class="navbar-item__text"
            ><i class="icofont-home"></i></span>
          <span class="item__text-label">Trang chủ</span>
        </router-link>
        <router-link to="/refs" class="navbar-item">
          <span class="navbar-item__text"
            ><i class="icofont-ui-cart"></i></span>
          <span class="item__text-label">Bán hàng NEW</span>
        </router-link>
        <router-link v-if="isSuperManagement" to="/reports" class="navbar-item">
          <span class="navbar-item__text"
            ><i class="icofont-chart-line-alt"></i
          ></span>
          <span class="item__text-label">Thống kê</span>
        </router-link>
        <router-link to="/stocks" class="navbar-item">
          <span class="navbar-item__text"
            ><i class="icofont-stack-overflow"></i
          ></span>
          <span class="item__text-label">Kho</span>
          <div class="nav-sub" v-if="showStock">
            <router-link to="/stocks/receipt" class="navbar-item">
              <div class="nav-item__icon icon icon-20 icon--input"></div>
              <span class="item__text-label">Nhập kho</span>
            </router-link>
            <router-link to="/stocks/export" class="navbar-item">
              <div class="nav-item__icon icon icon-20 icon--output"></div>
              <span class="item__text-label">Xuất kho</span>
            </router-link>
          </div>
        </router-link>
        <router-link to="/inventoryItemCategories" class="navbar-item">
          <span class="navbar-item__text"><i class="icofont-folder"></i></span>
          <span class="item__text-label">Nhóm hàng hoá</span>
        </router-link>
        <router-link to="/inventories" class="navbar-item">
          <span class="navbar-item__text"><i class="icofont-barcode"></i></span>
          <span class="item__text-label">Hàng hoá</span>
        </router-link>
        <router-link to="/branchs" class="navbar-item">
          <span class="navbar-item__text"
            ><i class="icofont-bank-alt"></i
          ></span>
          <span class="item__text-label">Chi nhánh</span>
        </router-link>
        <!-- <router-link to="/dictionary" class="navbar-item">
          <span class="navbar-item__text"
            ><i class="icofont-book-alt"></i
          ></span>
          <span class="item__text-label">Danh mục</span>
        </router-link> -->
        <a
          class="navbar-item"
          @click="showSubMenu = !showSubMenu"
          v-clickoutside="hideSubMenu"
        >
          <span class="navbar-item__text">
            <i class="icofont-folder"></i>
          </span>
          <span class="item__text-label">Danh mục</span>
        </a>
        <router-link
          v-if="isSuperManagement"
          to="/settings"
          class="navbar-item"
        >
          <span class="navbar-item__text"
            ><i class="icofont-ui-settings"></i
          ></span>
          <span class="item__text-label"> Cài đặt hệ thống</span>
        </router-link>
        <div v-show="showSubMenu" class="nav-sub">
          <router-link to="/group-products" class="nav-sub-item">
            <span class="sub-item__icon"><i class="icofont-cubes"></i></span>
            <span class="sub-item__text"> Nhóm sản phẩm/Dịch vụ</span>
          </router-link>
          <router-link to="/products" class="nav-sub-item">
            <span class="sub-item__icon"><i class="icofont-cube"></i></span>
            <span class="sub-item__text"> Sản phẩm/Dịch vụ</span>
          </router-link>
          <router-link to="/positions" class="nav-sub-item">
            <span class="sub-item__icon"
              ><i class="icofont-invisible"></i
            ></span>
            <span class="sub-item__text"> Vị trí/Chức vụ</span>
          </router-link>
          <router-link to="/employees" class="nav-sub-item">
            <span class="sub-item__icon"
              ><i class="icofont-businessman"></i
            ></span>
            <span class="sub-item__text"> Nhân viên</span>
          </router-link>
          <router-link to="/customers" class="nav-sub-item">
            <span class="sub-item__icon"
              ><i class="icofont-handshake-deal"></i
            ></span>
            <span class="sub-item__text"> Khách hàng</span>
          </router-link>
          <router-link to="/customer-groups" class="nav-sub-item">
            <span class="sub-item__icon"
              ><i class="icofont-users-alt-5"></i
            ></span>
            <span class="sub-item__text"> Nhóm khách hàng</span>
          </router-link>
          <router-link to="/units" class="nav-sub-item">
            <span class="sub-item__icon"
              ><i class="icofont-calculations"></i
            ></span>
            <span class="sub-item__text"> Đơn vị tính</span>
          </router-link>
          <router-link to="/distributors" class="nav-sub-item">
            <span class="sub-item__icon"
              ><i class="icofont-atom"></i></span>
            <span class="sub-item__text"> Nhà phân phối</span>
          </router-link>
        </div>
      </div>
    </div>
  </nav>
  <header>
    <div class="header__left">
      <div class="header__action"></div>
      <div class="page-title">{{ $route.meta.title }}</div>
    </div>
    <div class="header__right">
      <div class="account-info">
        <a
          v-if="isAuthenticated"
          class="account"
          style="flex-direction: row; column-gap: 10px"
          @click="showAccountOption = !showAccountOption"
          v-clickoutside="hideListAccountOption"
        >
          <div
            class="navbar-item__avatar"
            :class="{ 'default-avatar': !hasAvatar }"
            :style="{
              'background-image': `url(${avatarPath})`,
            }"
          ></div>
          <span>{{ account.UserName }}</span>
          <div v-if="showAccountOption" class="account-option">
            <router-link to="/account" class="option-item">
              <div class="option__icon">
                <i class="icofont-info-circle"></i>
              </div>
              <div class="option__text">Thông tin</div>
            </router-link>
            <div class="option-item" @click="onChangePassword">
              <div class="option__icon"><i class="icofont-key"></i></div>
              <div class="option__text">Đổi mật khẩu</div>
            </div>
            <router-link v-if="isAdministrator" to="/admin" class="option-item">
              <div class="option__icon">
                <i class="icofont-architecture-alt"></i>
              </div>
              <div class="option__text">Quản lý ứng dụng</div>
            </router-link>
            <div class="option-item" @click="logOut">
              <div class="option__icon"><i class="icofont-sign-out"></i></div>
              <div class="option__text">Đăng xuất</div>
            </div>
          </div>
        </a>
        <router-link v-else to="/login" class="navbar-item">
          <span class="navbar-item__text"
            ><i class="icofont-login"></i> Đăng nhập</span
          >
        </router-link>
      </div>
    </div>
  </header>
  <!-- <div v-if="isSuperManagement" class="org-info">
    <div class="info-item">
      Tổng hóa đơn tháng: {{ organizationInfo.TotalInvoices || 0 }}
    </div>
    <div class="info-item">
      <span class="flipX"
        >Doanh thu tháng:
        <span class="total-money"
          >+{{ commonJs.formatMoney(organizationInfo.TotalMoneys) || 0 }}</span
        ></span
      >
    </div>
  </div> -->
  <ChangePassword
    v-if="showChangePassword"
    @onClose="showChangePassword = false"
  ></ChangePassword>
</template>
<script>
import Enum from "@/scripts/enum";
/* eslint-disable */
/**
 * Gán sự kiện nhấn click chuột ra ngoài combobox data (ẩn data list đi)
 * NVMANH (31/07/2022)
 */
const clickoutside = {
  mounted(el, binding, vnode, prevVnode) {
    el.clickOutsideEvent = (event) => {
      // Nếu element hiện tại không phải là element đang click vào
      // Hoặc element đang click vào không phải là button trong combobox hiện tại thì ẩn đi.
      if (
        !(
          (
            el === event.target || // click phạm vi của combobox__data
            el.contains(event.target) || //click vào element con của combobox__data
            (el.previousElementSibling &&
              el.previousElementSibling.contains(event.target))
          ) //click vào element button trước combobox data (nhấn vào button thì hiển thị)
        )
      ) {
        // Thực hiện sự kiện tùy chỉnh:
        binding.value(event, el);
        // vnode.context[binding.expression](event); // vue 2
      }
    };
    document.body.addEventListener("click", el.clickOutsideEvent);
  },
  beforeUnmount: (el) => {
    document.body.removeEventListener("click", el.clickOutsideEvent);
  },
};
import { AUTH_LOGOUT } from "../../store/actions/auth";
import ChangePassword from "../../views/account/ChangePassword.vue";
import store from "@/store";
export default {
  name: "TheHeader",
  directives: {
    clickoutside,
  },
  components: { ChangePassword },
  props: ["isAuthenticated", "isProfileLoaded", "organizationInfo"],
  created() {
    // console.log(localStorage.getItem("userRoleValue"));
    if (this.isAuthenticated) {
      this.account.AvatarFullPath = localStorage.getItem("avatar");
      this.account.FullName = localStorage.getItem("fullName");
    }
  },
  computed: {
    isAdministrator: function () {
      var roleValue = store.getters.role;
      if (roleValue == Enum.Role.Administrator) {
        return true;
      } else {
        return false;
      }
    },
    isSuperManagement: function () {
      var roleValue = store.getters.role;
      if (roleValue <= Enum.Role.SuperManagement) {
        return true;
      } else {
        return false;
      }
    },
    userName: function () {
      if (this.isAuthenticated) {
        return localStorage.getItem("userName");
      }
    },

    hasAvatar: function () {
      if (
        this.account.AvatarFullPath &&
        this.account.AvatarFullPath != "null"
      ) {
        return true;
      } else {
        return false;
      }
    },
    avatarPath: function () {
      if (
        this.account.AvatarFullPath &&
        this.account.AvatarFullPath != "null"
      ) {
        return this.account.AvatarFullPath;
      } else {
        return "../../assets/imgs/avatar.png";
      }
    },
  },
  watch: {
    isProfileLoaded: function (newValue) {
      if (newValue) {
        this.account.AvatarFullPath = localStorage.getItem("avatar");
        this.account.FullName = localStorage.getItem("fullName");
        this.account.UserName = localStorage.getItem("userName");
      }
    },
  },
  methods: {
    logOut() {
      this.hubConnection.invoke("RemoveConnection");
      this.$store.dispatch(AUTH_LOGOUT);
    },
    hideListAccountOption() {
      this.showAccountOption = false;
    },
    hideSubMenu() {
      this.showSubMenu = false;
    },
    onChangePassword() {
      this.showChangePassword = true;
    },
  },
  data() {
    return {
      account: { AvatarFullPath: null },
      showAccountOption: false,
      showSubMenu: false,
      showChangePassword: false,
      showStock: false,
    };
  },
};
</script>

<style scoped>
@import url(../../styles/layout/header.css);
</style>
