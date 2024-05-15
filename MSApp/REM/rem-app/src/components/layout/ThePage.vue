<template>
  <div class="page">
    <!-- <div class="page__header">
      <div class="page__title">{{ title }}</div>
      <div v-show="rightSlot" class="page__header--right">
        <slot name="header-right"></slot>
      </div>
    </div> -->
    <div class="page__container">
      <slot name="container"></slot>
    </div>
    <div class="page__footer">
      <slot name="footer"></slot>
    </div>
  </div>
</template>
<script>
export default {
  name: "ThePage",
  emits: [],
  props: {
    title: String,
    rightSlot: {
      type: Boolean,
      default: false,
      required: false,
    },
  },
  created() {
  },
};
</script>
<style scoped>
@import url(../../styles/base/page.css);
</style>