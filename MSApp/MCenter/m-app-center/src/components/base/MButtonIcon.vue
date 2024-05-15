<template>
  <button
    class="btn btn-icon"
    :class="`${cls} ${iconCls}`"
  >
    {{ text }}
    <slot></slot>
  </button>
</template>
<script>
export default {
  name: "MButtonIcon",
  props: ["text", "cls", "iconCls"],
};
</script>
<style scoped>
@import url(../../styles/base/button.css);

.btn-icon{
  min-width: 80px;
  width: unset;
  padding-left: 36px;
  padding-right: 12px;
  background-position: 12px center;
  background-size: 16px 16px;
  background-repeat: no-repeat;
}

</style>
