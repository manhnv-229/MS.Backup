<template>
  <m-page title="Danh mục kho hàng" :rightSlot="true">
    <template v-slot:container>
      <div class="mtab">
        <div class="mtab__list">
          <router-link to="/inventories/list" class="mtab__item">
            Danh sách hàng hóa
          </router-link>
          <router-link to="/inventories/groups" class="mtab__item"
            >Nhóm hàng hóa</router-link
          >
        </div>
        <div class="mtab__container">
          <router-view name="InventoryRouterView"></router-view>
        </div>
      </div>
    </template>
  </m-page>
</template>
<script>
export default {
  name: "InventoryIndex",
};
</script>
<style scoped>
</style>