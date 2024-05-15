<template>
  <div ref="wdSell" class="mtab" :class="{ 'zoom--out': zoomOut }">
    <div class="mtab__toolbar">
      <button class="mtab__button --refresh" @click="onRefresh"></button>
      <button
        class="mtab__button --zoom"
        :class="[ zoomOut?'--zoom-in':'--zoom-out' ]"
        @click="onZoom"
      ></button>
      <button v-if="zoomOut" class="mtab__button --close" @click="onClose"></button>
    </div>
    <div class="mtab__list">
      <router-link to="/sales/dashboard" class="mtab__item">
        Tổng quan
      </router-link>
      <router-link
        v-for="item in branchs"
        :key="item.BranchId"
        :to="`/sales/branchs/${item.BranchId}`"
        class="mtab__item"
      >
        {{ item.BranchName }}
      </router-link>
    </div>
    <div class="mtab__container">
      <router-view name="SaleRouterView"></router-view>
    </div>
  </div>
</template>
<script>
import { toggleFullScreen } from "@/scripts/helper";
import { mapGetters, mapState } from "vuex";
export default {
  name: "REMSaleIndex",
  emits: [],
  props: {},
  computed: {
    ...mapGetters(["slotGroups", "branchs", "slots"]),
  },
  watch: {},
  created() {},
  methods: {
    onZoom(){
      // this.zoomOut = !this.zoomOut;
      toggleFullScreen(this.$refs.wdSell);
    },
    onClose(){
      this.zoomOut = !this.zoomOut;
    },
    onRefresh(){
      this.$emitter.emit("refreshData");
    }
  },
  data() {
    return {
      zoomOut: false,
    };
  },
};
</script>
<style scoped>
.mtab {
  background-color: #fff;
  border-radius: 4px;
  position: relative;
  overflow-y: auto;
}

.mtab__container {
  position: relative;
  padding-left: 12px;
  padding-right: 12px;
  padding-bottom: 12px;
  box-sizing: border-box;
  width: 100%;
  overflow-y: auto
}

.mtab__container:has(::-webkit-scrollbar){
  padding: 16px 0;
}

.zoom--out {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
}

.mtab__toolbar {
  position: absolute;
  right: 4px;
  top: 4px;
  display: flex;
  column-gap: 6px;
  background-color: #fff;
  padding: 6px;
  z-index: 997;
}

.mtab__button {
  width: 36px;
  height: 36px;
  background-position: center;
  background-repeat: no-repeat;
  background-size: 24px;
  border: unset;
  border-radius: 4px;
  cursor: pointer;
  background-color: #fff;
  border: 1px solid #dedede
}
.mtab__button:hover{
  border: 1px solid var(--color-primary)
}

.mtab__button.--close {
  background-image: url(./icon/close-48.png);
}
.mtab__button.--zoom-out {
  background-image: url(./icon/zoom-out.png);
}

.mtab__button.--zoom-in {
  background-image: url(./icon/zoom-in.png);
}

.mtab__button.--refresh{
  background-image: url(./icon/refresh-32.png);
}
</style>
