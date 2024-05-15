<template>
  <div class="mtab">
    <div class="mtab__list">
      <!-- <router-link to="/slots/dashboard" class="mtab__item">
        Phòng/Bàn
      </router-link>
      <router-link to="/slots/group" class="mtab__item"
        >Nhóm Phòng/Bàn</router-link
      > -->
    </div>
    <div class="mtab__container"> 
        <slot name="container"></slot>
    </div>
  </div>
</template>
<script>
export default {
  name: "MTab",
  props: {},
  emits: {},
  computed: {},
  watch: {},
  created() {},
  methods: {},
  data() {
    return {};
  },
};
</script>
