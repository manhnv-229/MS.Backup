<template>
  <div class="paging">
    <div class="paging__left">
      Tổng số: <b>{{ totalRecords }}</b> <span class="paging-entity-name">{{ entityName.toLowerCase() }}</span>
    </div>
    <div class="paging__right">
      <div class="paging__pagesize">
        <span>Số bản ghi/ trang</span>
        <div class="pagesize-cbx">
          <input type="text" :value="pageSizeComputed" @click="onShowPagesizeData" />
          <div v-show="showPagesizeData" class="pagesize-data">
            <a
              class="pagesize-item"
              v-for="(item, index) in pageSizeData"
              :key="index"
              :class="{ 'pagesize-item--selected': index == indexPageSizeSelected }"
              @click="onSelectPagesizeItem(index, item)"
            >
              <div class="pagesize-item__text">{{ item }}</div>
              <div class="pagesize-item__icon"></div>
            </a>
          </div>
        </div>
      </div>
      <div class="paging-index">Từ {{ startIndex }}-{{ endIndex }}</div>
      <div class="paging__button">
        <button class="btn-paging--prev" @click="onPrevPage" :disabled="disabledPrev">
          <i class="icofont-rounded-left"></i>
        </button>
        <button class="btn-paging--next" @click="onNextPage" :disabled="disabledNext">
          <i class="icofont-rounded-right"></i>
        </button>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: "MPaging",
  emits: [
    "onChangePagesize",
    "onPrevPage",
    "onNextPage",
    "onInitPaging",
    "update:limit",
    "update:offset",
  ],
  props: {
    // Tên của đối tượng (VD: Nhân viên, khách hàng -> sử dụng để hiển thị thay cho bản ghi)
    entityName: {
      type: String,
      default: "bản ghi",
      required: false,
    },
    pageSizeDefault: Number,
    // Tổng số bản ghi
    totalRecords: {
      type: Number,
      default: 0,
      required: false,
    },
    limit: {
      type: Number,
      default: 0,
      required: false,
    },
    offset: {
      type: Number,
      default: 0,
      required: false,
    },
    // Kích thước số bản ghi/trang
    pageSizeData: {
      type: Array,
      default: () => [10, 20, 50, 100, 200],
      required: false,
    },
  },
  computed: {
    pageSizeComputed: function () {
      return this.pageSizeData[this.indexPageSizeSelected];
    },
    startIndexComputed: function () {
      var newStartIndex = this.startIndex - this.pageSizeSelected;
      if (newStartIndex < 0) {
        return 1;
      } else {
        return newStartIndex;
      }
    },
    endIndexComputed: function () {
      var newEndIndex = this.startIndex + this.pageSizeSelected - 1;
      if (this.totalRecords < this.pageSizeSelected || this.totalRecords <= newEndIndex) {
        return this.totalRecords;
      } else {
        return newEndIndex;
      }
    },
    disabledNext: function () {
      return this.endIndex >= this.totalRecords;
    },
    disabledPrev: function () {
      return this.startIndex <= 1;
    },
  },
  watch: {
    /**
     * Khi tổng số bản ghi thay đổi -> mặc định chuyển về trang đầu tiên
     */
    totalRecords(newValue) {
      if (newValue > 0) {
        this.startIndex = 1;
        this.endIndex = this.endIndexComputed;
      } else {
        this.startIndex = 0;
        this.endIndex = 0;
      }
    },
    // Thay đổi kích thước trang thì cập nhật lại startIndex và endIndex:
    pageSizeSelected() {
      this.endIndex = this.endIndexComputed;
      let numberRecords = this.endIndex - this.startIndex + 1;
      if (this.endIndex >= this.totalRecords && numberRecords < this.pageSizeSelected) {
        this.startIndex = this.startIndexComputed;
      }
    },
  },
  created() {
    // Gán các giá trị ban đầu:
    this.pageSizeSelected = this.pageSizeComputed;
    this.$emit("onInitPaging", this.startIndex, this.endIndex, this.pageSizeSelected);
  },
  methods: {
    /** -----------------------------
     * Hiển thị các lựa chọn kích thước trang
     * Author: NVMANH (09/12/2022)
     */
    onShowPagesizeData() {
      this.showPagesizeData = true;
    },

    /** -----------------------------
     * Thay đổi kích thước trang
     * Author: NVMANH (09/12/2022)
     */
    onSelectPagesizeItem(index, item) {
      this.indexPageSizeSelected = index;
      this.pageSizeSelected = item;
      this.showPagesizeData = false;
      this.$emit("update:limit", this.pageSizeSelected);
      this.$emit("onChangePagesize", item);
    },

    /** -----------------------------
     * Chuyển về trang trước đó
     * Author: NVMANH (09/12/2022)
     */
    onPrevPage() {
      this.startIndex = this.startIndexComputed;
      this.endIndex = this.endIndexComputed;
      this.$emit("update:offset", this.startIndex - 1);
      this.$emit("onPrevPage", this.startIndex, this.endIndex, this.pageSizeComputed);
    },

    /** -----------------------------
     * Chuyển tới trang tiếp theo
     * Author: NVMANH (09/12/2022)
     */
    onNextPage() {
      this.startIndex += this.pageSizeSelected;
      this.endIndex = this.endIndexComputed;
      this.$emit("update:offset", this.startIndex - 1);
      this.$emit("onNextPage", this.startIndex, this.endIndex, this.pageSizeComputed);
    },
  },
  data() {
    return {
      indexPageSizeSelected: 0, // Chỉ mục của số bản ghi/ trang đang chọn
      pageSizeSelected: null, // Độ lớn của trang hiện tại
      showPagesizeData: false, // Ẩn hay hiển thị các lựa chọn độ lớn trang
      startIndex: 1,
      endIndex: null,
    };
  },
};
</script>
<style scoped>
@import url(../../styles/base/paging.css);
</style>
