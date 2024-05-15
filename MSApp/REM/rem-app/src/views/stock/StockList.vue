<template>
  <m-table-layout>
    <!-- TOOLBAR -->
    <template v-slot:toolbar>
      <div class="toolbar__left">
        <m-input-search
          width="300px"
          placeholder="Tìm kiếm"
          v-model="keySearch"
        ></m-input-search>
      </div>
      <div class="toolbar__right">
        <button class="btn btn-refresh" @click="loadData()">
          <i class="icofont-refresh"></i>
        </button>
        <button class="btn btn--default btn--refresh" @click="onAddNew">
          <i class="icofont-ui-add"></i> Thêm kho hàng
        </button>
      </div>
    </template>
    <!-- BẢNG DỮ LIỆU -->
    <template v-slot:container>
      <m-table
        ref="tbStocks"
        :data="stocks"
        empty-text="Không có dữ liệu"
        width="100%"
        height="100%"
      >
        <m-column type="index" label="#" width="50px"></m-column>
        <m-column
          prop="StockCode"
          width="150px"
          label="Tên kho hàng"
        ></m-column>
        <m-column prop="StockName" label="Tên kho hàng"></m-column>
        <m-column prop="Description" label="Mô tả"></m-column>
        <m-column width="80" fixed="right">
          <template #default="scope">
            <div v-if="scope.row.IsSystem == 0" class="button-column">
              <button
                class="btn--table-mini --color-edit"
                :title="scope.row.StockName"
                @click="onUpdate(scope.row)"
              >
                <i class="icofont-ui-edit"></i>
              </button>
              <button
                class="btn--table-mini --color-red"
                :title="scope.row.StockName"
                @click="onDelete(scope.row)"
              >
                <i class="icofont-ui-close"></i>
              </button>
            </div>
          </template>
        </m-column>
      </m-table>
    </template>
    <!-- PHÂN TRANG -->
    <template v-slot:footer>
      <m-paging
        entityName="kho hàng"
        :totalRecords="totalRecords"
        :pageSizeData="[10, 20, 50, 100, 200]"
        v-model:limit="limit"
        v-model:offset="offset"
        @onChangePagesize="loadData"
        @onPrevPage="loadData"
        @onNextPage="loadData"
        @onInitPaging="onInitPaging"
      ></m-paging>
    </template>
  </m-table-layout>
  <StockDetail
    v-if="showDetail"
    @onClose="onCloseDetail"
    :id="idSelected"
  ></StockDetail>
</template>
<script>
import debounce from "../../scripts/debounce";
import StockDetail from "./StockDetail.vue";
export default {
  name: "StockList",
  components: { StockDetail },
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
    };
  },
};
</script>
<style scoped></style>
