<template>
  <div
    class="mdialog"
    :class="{ 'mdialog-overlay': overlay }"
    :style="`z-index:${1001 + zindex || 0}`"
  >
    <div class="mdialog-wrapper" :style="{ width: width, height: height }">
      <button class="mdialog__button-close" @click="onClose">
        <i class="icofont-ui-close"></i>
      </button>
      <div
        v-if="showHeader"
        class="mdialog__header"
        :class="{ 'm-movable': dragable }"
        :style="{ backgroundColor: headerBgColor }"
      >
        <div v-if="title" class="mdialog__header--title">{{ title }}</div>
        <div v-else>
          <slot name="header"></slot>
        </div>
      </div>
      <div
        class="mdialog__content"
        :style="{ backgroundColor: contentBgColor }"
      >
        <slot name="content"></slot>
      </div>
      <div class="mdialog__footer" v-if="showFooter">
        <slot name="footer"></slot>
        <button
          v-if="showButton"
          class="btn btn--cancel mdialog__button--close"
        ></button>
        <button
          v-if="showButton"
          class="btn btn-default mdialog__button--save"
          @click="onSubmit"
        >
          {{ submitText }}
        </button>
      </div>
    </div>
  </div>
</template>
<script src="./script.js"></script>
<style scoped>
@import url(./style.css);
</style>
