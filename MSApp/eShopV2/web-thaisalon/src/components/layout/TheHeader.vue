<template>
  <div class="page-title animate-charcter" style="">
    <div>{{ organizationInfo.OrganizationName }}</div>
    <div> - {{ organizationInfo.Slogan }} - </div>
  </div>
  <nav class="navbar">
    <div class="navbar-container">
      <div class="navbar-list">
        <a
          id="nav-home"
          class="navbar-item"
          @click="showSubMenu = !showSubMenu"
          v-clickoutside="hideSubMenu"
        >
          <i class="icofont-navigation-menu"></i>
        </a>
        <router-link to="/invoices" class="navbar-item">
          <span class="navbar-item__text"
            ><i class="icofont-sale-discount"></i
          ></span>
          <span class="item__text-label">Bán hàng</span>
        </router-link>
        <router-link  v-if="isSuperManagement" to="/reports" class="navbar-item">
          <span class="navbar-item__text"
            ><i class="icofont-chart-line-alt"></i
          ></span>
          <span class="item__text-label">Thống kê</span>
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
          <router-link v-if="isSuperManagement" to="/settings" class="nav-sub-item">
            <span class="sub-item__icon"
              ><i class="icofont-ui-settings"></i
            ></span>
            <span class="sub-item__text"> Cài đặt hệ thống</span>
          </router-link>
        </div>
      </div>
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
  </nav>
  <div  v-if="isSuperManagement" class="org-info">
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
  </div>
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
    console.log( localStorage.getItem("userRoleValue"));
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
    isSuperManagement: function(){
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
    };
  },
};
</script>

<style scoped>
.page-title {
  position: sticky;
  top: 0;
  width: 100%;
  display: flex;
  flex-wrap: wrap;
  column-gap: 5px;
  align-items: center;
  justify-content: center;
  background-color: #00000078;
  color: #fff;
  text-align: center;
  text-transform: uppercase;
  font-weight: 700;
}
.navbar {
  position: sticky;
  top: 20px;
  width: 100%;
  height: 40px;
  background-color: #00000078;
  color: #fff;
  border-top: 1px solid #fff;
  z-index: 101;
  box-sizing: border-box;
}

.navbar-container {
  position: relative;
  max-width: 700px;
  height: 100%;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.navbar-list {
  height: 100%;
  display: flex;
  align-items: center;
  column-gap: 4px;
}
.navbar-item {
  height: 100%;
  padding: 0 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  cursor: pointer;
}

.navbar-item:hover {
  background-color: #656565cc;
}

#nav-home {
  background-color: #004982;
}

#nav-home i {
  font-size: 20px !important;
}
.navbar-item i {
  font-size: 16px;
  margin-right: 5px;
}
.router-link-active {
  background-color: #656565cc;
}

.nav-account {
  height: 100%;
}
.navbar a {
  color: #fff;
  text-decoration: none;
}

.navbar-item__avatar {
  width: 24px;
  height: 24px;
  background-size: cover;
  background-position: center;
  border-radius: 50%;
  /* margin-right: 10px; */
  background-repeat: no-repeat;
}
.account-info {
  height: 100%;
}
.account {
  position: relative;
  height: 100%;
  cursor: pointer;
  background-color: #004982;
  display: flex;
  align-items: center;
  padding: 0 12px;
}
.account-option {
  position: absolute;
  top: calc(100% + 1px);
  right: 0;
  z-index: 9999;
  background-color: #000000;
  box-shadow: 0 3px 6px #ccc;
  padding: 5px;
  display: flex;
  flex-direction: column;
  row-gap: 4px;
  border-radius: 4px;
}

.option-item {
  padding: 10px;
  white-space: nowrap;
  display: flex;
  column-gap: 10px;
}

.option-item:hover {
  background-color: #4a4a4a;
}

.org-info {
  height: 25px;
  width: 100%;
  top: 60px;
  position: sticky;
  background-color: #fffb02;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  padding: 0 24px;
  box-sizing: border-box;
}

.info-item + .info-item {
  margin-left: 10px;
}

.total-money {
  color: #0c701e;
}

.expenditure a {
  text-decoration: none;
  margin: 0 10px;
}

.expenditure .router-link-active {
  color: #fff;
  background-color: #ff0000;
}
@media screen and (max-width: 411px) {
  /* .item__text-label{
    display: none;
  } */
  .navbar-item {
    padding: 4px 10px;
    box-sizing: border-box;
  }
}

.nav-sub {
  display: flex;
  flex-direction: column;
  row-gap: 4px;
  position: absolute;
  left: 0;
  top: 100%;
  background-color: #000000c8;
  padding: 5px;
  border-radius: 0 0 4px 4px;
}

.nav-sub-item {
  height: 36px;
  display: flex;
  align-items: center;
  column-gap: 10px;
  cursor: pointer;
  padding: 0 4px;
}

.nav-sub-item:hover,
.nav-sub-item:focus {
  background-color: #656565cc;
}

.sub-item__icon {
  font-size: 16px;
}

.sub-item__text {
  font-weight: 600;
}
</style>
