<template>
    <m-page title="Danh mục kho hàng" :rightSlot="true">
      <!-- <template v-slot:header-right> </template> -->
      <template v-slot:container>
        <div class="mtab">
          <div class="mtab__list">
            <router-link to="/stocks/dashboard" class="mtab__item">
              Danh sách kho
            </router-link>
            <router-link to="/stocks/receipt" class="mtab__item"
              >Nhập kho</router-link
            >
            <router-link to="/stocks/export" class="mtab__item"
              >Xuất kho</router-link
            >
          </div>
          <div class="mtab__container">
            <router-view name="StockRouterView"></router-view>
          </div>
        </div>
      </template>
      <template v-slot:footer> </template>
    </m-page>

  </template>
  <script>
  import debounce from "@/scripts/debounce";
  export default {
    name: "StockIndex",
    components: {  },
    emits: [],
    props: [],
    created() {},
    watch: {
      keySearch: debounce(function () {
        this.loadData();
      }, 500),
    },
    methods: {
      onInitPaging(startIndex, endIndex, pageSize) {
        this.limit = pageSize;
        this.offset = startIndex - 1;
        this.loadData();
      },
      loadData() {
        var key = this.keySearch;
        this.maxios
          .get(
            `stocks/paging?limit=${this.limit}&offset=${this.offset}&key=${
              key || ""
            }`,
          )
          .then((res) => {
            this.stocks = res.Data;
            this.totalRecords = res.TotalRecords;
          });
      },
      onAddNew() {
        this.showDetail = true;
        this.idSelected = null;
      },
      onUpdate(stock) {
        this.idSelected = stock.StockId;
        this.showDetail = true;
      },
      onDelete(stock) {
        this.commonJs.showConfirm(
          `Bạn có chắc chắn muốn xóa [${stock.StockName}] không?`,
          () => {
            this.maxios.delete(`stocks/${stock.StockId}`);
            this.loadData();
          },
        );
      },
      onCloseDetail() {
        this.showDetail = false;
        this.loadData();
      },
    },
    data() {
      return {
        stocks: [],
        totalRecords: 0,
        isReload: false,
        keySearch: "",
        limit: 0,
        offset: 0,
        showDetail: false,
        idSelected: null,
        reportTabActive: "StockList",
      };
    },
  };
  </script>
  <style scoped></style>
  