<template>
  <!-- <div class="page-title animate-charcter" style="">
    <div>{{ organizationInfo.OrganizationName }}</div>
    <div>- {{ organizationInfo.Slogan }} -</div>
  </div> -->

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
  <ChangePassword
    v-if="showChangePassword"
    @onClose="showChangePassword = false"
  ></ChangePassword>
</template>
<script>
import Enum from "../../scripts/enum";
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
import store from "../../store";
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
   
    onChangePassword() {
      this.showChangePassword = true;
    },
  },
  data() {
    return {
      account: { AvatarFullPath: null },
      showAccountOption: false,
      showChangePassword: false,
    };
  },
};
</script>

<style scoped>
@import url(../../styles/layout/header.css);
</style>
