<template>
  <nav class="navbar" :class="{ 'navbar--collapse': !isExpand }">
    <div class="navbar-container">
      <div class="navbar-list">
        <a id="nav-home" class="navbar-item">
          <i class="icofont-navigation-menu"></i>
          <span v-if="isExpand">{{ organization.OrganizationName }}</span>
        </a>
        <NavbarRouterLinkItem
          v-for="item in navbarItems"
          :key="item.Sort"
          :item="item"
          :isExpand="isExpand"
        ></NavbarRouterLinkItem>
      </div>
      <div
        class="navbar__button-collapse icon icon-24 icon--leftw50"
        :class="{ 'icon--rightw50': !isExpand }"
        @click="isExpand = !isExpand"
      >
        <span v-if="isExpand">Thu gọn</span>
      </div>
    </div>
  </nav>
</template>
<script>
import { computed } from "vue";
import store from "@/store";
import NavbarRouterLinkItem from "./NavbarRouterLinkItem.vue";
// import { mapGetters } from "vuex";
export default {
  name: "TheNavbar",
  props: ["isAuthenticated", "isProfileLoaded"],
  components: { NavbarRouterLinkItem },
  inject:["organization"],
  watch: {
    "organization.NavbarItem": function (newValue) {
      this.navbarItems = [...newValue];
    },
  },
  provide() {
    return {
      navbarItems: computed(() => this.navbarItems),
    };
  },
  computed: {
    // ...mapGetters(["inventories"]),
    navbarItemsRoot: function () {
      const data = [...this.navbarItems.filter((i) => i.ParentId === null)];
      return data;
    },

    isAdministrator: function () {
      var roleValue = store.getters.role;
      if (roleValue == this.Enum.Role.Administrator) {
        return true;
      } else {
        return false;
      }
    },
    isSuperManagement: function () {
      var roleValue = store.getters.role;
      if (roleValue <= this.Enum.Role.SuperManagement) {
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
  },
  created() {
    // this.maxios.get("/navbaritems").then((data) => {
    //   console.log("load items");
    //   this.navbarItems = data;
    // });
  },
  methods: {
    hideSubMenu() {
      this.showSubMenu = false;
    },
  },
  data() {
    return {
      showSubMenu: false,
      showStock: false,
      isExpand: true,
      navbarItems: [],
    };
  },
};
</script>
<style>
.navbar-item__main{
  width: 100%;
}

.item__text-label{
  flex: 1
}
</style>
