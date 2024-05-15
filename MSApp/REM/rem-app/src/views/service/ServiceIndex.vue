<template>
    <m-page title="Danh mục dịch vụ" :rightSlot="true">
      <template v-slot:container>
        <div class="mtab">
          <div class="mtab__list">
            <router-link to="/services/list" class="mtab__item">
              Dịch vụ
            </router-link>
            <router-link to="/services/groups" class="mtab__item"
              >Nhóm dịch vụ</router-link
            >
          </div>
          <div class="mtab__container">
            <router-view name="ServiceRouterView"></router-view>
          </div>
        </div>
      </template>
    </m-page>
  </template>
  <script>
  export default {
    name: "ServiceIndex",
  };
  </script>
  <style scoped>
  </style>