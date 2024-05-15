<template>
  <router-link
    :to="item.Path"
    class="navbar-item"
    @click.native.capture.prevent="onShowSubmenu"
  >
    <div class="navbar-item__main">
      <span
        v-if="item.UseIconFont"
        class="navbar-item__icon-font"
        v-html="item.IconFont"
      ></span>
      <div v-else class="nav-item__icon" :class="item.IconCls"></div>
      <span class="item__text-label" v-if="isExpand">{{ item.Label }}</span>
      <!-- <div class="item__index">{{ item.Sort }}</div> -->
    </div>
    <div class="nav-sub" v-if="showSubMenu && item.NavbarSubItems.length > 0" v-clickoutside="hideSubMenu">
      <router-link
        :to="subItem.Path"
        class="navbar-item"
        v-for="subItem in item.NavbarSubItems"
        :key="subItem.Sort"
        @click.native.capture.prevent="subMenuOnClick(subItem)"
      >
        <span
          v-if="subItem.UseIconFont"
          class="navbar-item__icon-font"
          v-html="subItem.IconFont"
        ></span>
        <div v-else class="nav-item__icon" :class="subItem.IconCls"></div>
        <span class="item__text-label">{{ subItem.Label }}</span>
        <!-- <div class="item__index">{{ subItem.Sort }}</div> -->
      </router-link>
    </div>
  </router-link>
</template>
<script>
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
            event.target === el.parentElement ||
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
export default {
  props: ["item", "isExpand"],
  name: "NavbarRouterLinkItem",
  inject: ["navbarItems"],
  directives: {
    clickoutside,
  },
  watch: {},
  methods: {
    onShowSubmenu() {
      if (this.item.NavbarSubItems.length > 0) {
        event.preventDefault();
        this.showSubMenu = true;
      } else {
        this.$router.push(this.item.Path);
      }
    },
    subMenuOnClick(subItem) {
      this.$router.push(subItem.Path);
      this.showSubMenu = false;
    },
    onClickLink() {
      if (this.item.NavbarSubItems && this.item.NavbarSubItems.length > 0) {
        event.preventDefault();
      } else {
        this.$router.push(this.item.Path);
      }
    },
    hideSubMenu() {
      this.showSubMenu = false;
    },
  },
  data() {
    return {
      showSubMenu: false,
    };
  },
};
</script>
<style scoped>
.navbar-item__main {
  display: flex;
  align-items: center;
  column-gap: 8px;
}
</style>
