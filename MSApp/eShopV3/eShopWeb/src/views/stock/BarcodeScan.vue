<template>
  <div class="search-wrapper">
    <el-switch v-model="allowFastScan" active-text="Quét & chọn nhanh sản phẩm" @change="onChangeSearchMethod"
      style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949" />
    <div class="barcode">
      <input type="text" ref="minputBarcode" v-model="keySearch" class="barcode__input" :placeholder="placeholder"
        @keydown="inputOnKeyDown" @input="onInput" />
      <button ref="btnShowClear" v-if="isShowClear" class="clear-button" @click="clearButtonOnClick">
        <i class="icofont-close"></i>
      </button>
      <div v-if="allowFastScan" class="barcode__icon"></div>
      <div v-else class="search__icon"></div>
    </div>
    <div ref="listItemsFilter" class="list-item-filter" v-if="showData">
      <!-- <div ref="filterLoading" v-if="loadingFilter" class="filter--loading">
        <div class="filter--loading-icon"></div>
      </div> -->
      <div class="inventory-item" v-for="(item, index) in items" :key="index" @click="onSelectItem(item)">
        <div class="left">
          <div class="item__img">
            <img :src="item.ImgFullPath" alt="" />
          </div>
          <div class="item-info">
            <div class="item__name">{{ item.InventoryItemName }}</div>
            <div class="item__item-code">
              {{ item.InventoryItemCode }} -
              <el-tag>{{ commonJs.formatMoney(item.CostPrice) }}</el-tag>
            </div>
          </div>
        </div>
        <div class="right">
          <div class="item-price">
            <div class="item__price">
              {{ commonJs.formatMoney(item.CostPrice) }}
            </div>
            <div class="item__quantity">{{ item.Quantity }}</div>
          </div>
        </div>
      </div>
    </div>
    <!-- <div ref="filterLoading" v-if="loadingFilter" class="filter--loading">
      <div class="filter--loading-icon"></div>
    </div> -->
    <div class="list--not-exits" v-if="showListNotExits">
      <button class="dialog__button-close" @click="showListNotExits = false">
        <i class="icofont-ui-close"></i>
      </button>
      <div>Các mã sản phẩm không tồn tại trong hệ thống:</div>
      <li v-for="(item, index) in itemNotExits" :key="index">
        <b>{{ item }}</b>
      </li>
    </div>
    <div v-if="allowFastScan && showFastNotice" class="noti--fast">
      <div class="noti__icon"></div>
      <div class="noti__text">
        Mã vạch [<b>{{ lastKeyNotExits }}</b>] không có trong hệ thống.
      </div>
    </div>
  </div>
</template>
<script>
/* eslint-disable */
import debounce from "@/scripts/debounce";
import $ from 'jquery';
import barcodeScanerEvent from "@/utils/barcodescaner";
import { mapGetters } from "vuex";
export default {
  name: "BarcodeScan",
  emits: ["onSelectItem"],
  props: {
    placeholder: {
      type: String,
      default: "",
      required: false,
    },
  },
  created() {
    // new BarcodeScanerEvents();
    $(document).on("onbarcodescaned", this.onScaner);
    this.loadData();
  },
  // created() {
  //   this.maxios.get(`inventories/list-order`).then((res) => {
  //     this.items = res;
  //   });
  // },
  computed: {
    ...mapGetters([
            "inventories"
        ]),
  },
  watch: {
    keySearch: debounce(function () {
      this.searchItems();
    }, 300),
  },
  methods: {
    onScaner(event, data) {
      const barcode = data?.input?.trim();
      const target = data.target;
      // Nếu đang ở ô tìm kiếm thì không cần làm gì cả.
      if (target == this.$refs.minputBarcode || data.target.tagName === "INPUT" || data.target.tagName === "TEXTAREA") {
        return;
      }
      this.setFocus();
      this.onSetKeySearchValue(barcode);
    },
    onChangeSearchMethod() {
      this.setFocus();
    },
    onSetKeySearchValue(value) {
      this.keySearch = value;
    },
    inputOnKeyDown() {
      // console.log(event);
      if (event.keyCode == 46) {
        this.keySearch = null;
        event.stopPropagation();
        this.setFocus();
      }
    },
    onInput() {
      this.showFastNotice = false;
    },
    setFocus() {
      this.$nextTick(() => {
        this.$refs.minputBarcode.focus();
      });
    },
    loadData() {
      this.itemsCache = this.inventories;
      // this.maxios
      //   .get(`inventories/filter`)
      //   .then((res) => {
      //     this.itemsCache = res;
      //   });
    },
    searchItems() {
      // this.loadingFilter = true;
      if (this.keySearch) {
        this.isShowClear = true;
        // Có cache thì lấy trong cache ra:
        if (this.itemsCache?.length > 0) {
          this.setItemsSearch(this.items);
        } else {
          this.maxios
            .get(`inventories/filter`)
            .then((res) => {
              this.itemsCache = res;
              this.items = res;
              this.setItemsSearch();
            });
        }
      } else {
        this.isShowClear = false;
        this.items = [];
        this.showData = false;
      }
    },
    setItemsSearch() {
      const self = this;
      const key = self.commonJs.change_alias(self.keySearch);
      this.items = self.itemsCache.filter(item => self.commonJs.change_alias(item.Barcode).includes(key) || self.commonJs.change_alias(item.InventoryItemCode).includes(key) || self.commonJs.change_alias(item.InventoryItemName).includes(key));
      if (this.items && this.items.length == 1 && this.allowFastScan == true) {
        // clone item này tránh lỗi cập nhật giá:
        const itemClone = {...this.items[0]};
        this.$emit("onSelectItem", itemClone);
        this.showData = false;
        this.keySearch = null;
        this.setFocus();
      } else if (this.items && this.items.length > 1) {
        this.showData = true;
      } else {
        this.showData = false;
        if (this.allowFastScan == true) {
          this.showFastNotice = true;
          setTimeout(() => {
            this.showFastNotice = false;
          }, 3000);
          this.showListNotExits = true;
          this.itemNotExits.push(this.keySearch);
          this.lastKeyNotExits = this.keySearch;
          this.keySearch = null;
          this.setFocus();
        }
      }
    },
    onSelectItem(item) {
      const itemClone = {...item};
      this.$emit("onSelectItem", itemClone);
      this.showData = false;
      this.keySearch = null;
      this.setFocus();
    },
    clearButtonOnClick() {
      this.keySearch = null;
      this.setFocus();
    },
  },
  data() {
    return {
      items: [],
      itemsCache: [],
      itemNotExits: [],
      keySearch: null,
      lastKeyNotExits: "",
      showData: false,
      showListNotExits: false,
      allowFastScan: true,
      isShowClear: false,
      showFastNotice: false,
      loadingFilter: false,
    };
  },
};
</script>

<style>
.search-wrapper {
  position: relative;
}

.barcode {
  position: relative;
  display: flex;
  align-items: center;
  column-gap: 4px;
  border: 1px solid #dedede;
  border-radius: 4px;
  padding-right: 12px;
}

.barcode:has(input:focus) {
  border-color: #0033ff;
}

.barcode__input {
  width: 100%;
  min-width: 200px;
  border: unset !important;
}

.barcode__icon {
  width: 24px;
  height: 24px;
  background-image: url(../../assets/icon/barcode.png);
  background-size: contain;
  background-position: center;
  background-repeat: no-repeat;
}

.search__icon {
  width: 24px;
  height: 24px;
  background-image: url(../../assets/icon/searchgr-50.png);
  background-size: contain;
  background-position: center;
  background-repeat: no-repeat;
}

.list-item-filter {
  position: absolute;
  width: 100%;
  top: calc(100% + 10px);
  display: flex;
  flex-direction: column;
  row-gap: 10px;
  box-shadow: 0px 0px 10px #6d6d6d;
  overflow-y: auto;
  max-height: 400px;
  z-index: 999;
  background-color: #fff;
}

.list-item-filter--loading {
  min-height: 100px;
}

.filter--loading {
  height: 100px;
  width: 100%;
  position: absolute;
  z-index: 999;
  border: 1px solid #dedede;
  box-shadow: 0px 0px 10px #6d6d6d;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #fff;
}

.filter--loading-icon {
  width: 28px;
  height: 28px;
  border-top: 4px solid #dedede;
  border-right: 4px solid #dedede;
  border-bottom: 4px solid #dedede;
  border-left: 4px solid #fff;
  border-radius: 50%;
  animation: spin 1000ms infinite linear;
  -webkit-animation: spin 1000ms infinite linear;
}

.inventory-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  cursor: pointer;
  padding: 10px;
}

.inventory-item:hover {
  background-color: #e6f2ff;
}

.inventory-item .left {
  display: flex;
  align-items: center;
  column-gap: 10px;
  align-content: flex-start;
}

.item__img {
  width: 50px;
}

.item__img img {
  width: 50px;
}

.item__name {
  font-weight: 700;
}

.item-price {
  font-weight: 700;
}

.list--not-exits {
  position: fixed;
  left: 24px;
  bottom: 24px;
  width: 200px;
  height: 200px;
  padding: 24px;
  border-radius: 4px;
  box-shadow: 5px 5px 5px #ccc;
  z-index: 999;
  background-color: #ff00002b;
}

.list--not-exits i {
  font-weight: 700;
}

.dialog__button-close {
  border: 1px solid #ff0000;
  border-radius: 50%;
  position: absolute;
  width: 38px;
  height: 38px;
  top: -10px;
  right: -10px;
  font-size: 20px;
  padding: 0;
  color: #ff0000;
  background-color: #fff;
  cursor: pointer;
  box-shadow: 0px 0px 8px #6d6d6d;
}

.dialog__button-close:hover,
.dialog__button-close:focus {
  /* background-color: #d1d1d1; */
  color: #bd0000;
}

.clear-button {
  border: unset;
  display: block;
  position: absolute;
  right: 36px;
  top: 50%;
  transform: translate(-50%, -50%);
  padding: 2px;
  line-height: 12px;
  font-size: 12px;
  text-align: center;
  color: #fff;
  border-radius: 50%;
  background-color: #b1b1b1;
  cursor: pointer;
}

.clear-button:hover,
.clear-button:focus {
  background-color: #999999;
}

.noti--fast {
  border: 1px solid #ff0000;
  position: absolute;
  bottom: 40px;
  left: 50%;
  transform: translate(-50%, 10px);
  box-shadow: 5px 5px 5px #ccc;
  padding: 12px;
  border-radius: 4px;
  background-color: #ffffff;
  z-index: 9999;
  color: black;
  display: flex;
  align-items: center;
  -moz-column-gap: 10px;
  column-gap: 10px;
}

.noti__icon {
  width: 24px;
  height: 24px;
  flex-shrink: 0;
  flex-grow: 0;
  background-image: url(../../assets/icon/error-48.png);
  background-repeat: no-repeat;
  background-position: center;
  background-size: contain;
}
</style>
