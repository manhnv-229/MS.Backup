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
          <i class="icofont-ui-add"></i> Thêm phiếu nhập
        </button>
      </div>
    </template>
    <!-- BẢNG DỮ LIỆU -->

    <template v-slot:container>
      <m-table
        ref="tbStocks"
        :data="refs"
        empty-text="Không có dữ liệu"
        width="100%"
        height="100%"
      >
        <m-column type="index" label="#" width="50px"></m-column>
        <m-column prop="RefNo" width="150px" label="Mã phiếu">
          <template #default="scope">
            <a class="--link" @click.prevent="onClickRefItem(scope.row)">
              <el-tooltip
                class="box-item"
                effect="customized"
                :content="`${scope.row.RefNo}`"
                placement="top"
              >
                {{ scope.row.RefNo }}
              </el-tooltip>
            </a>
          </template>
        </m-column>
        <m-column prop="RefDate" label="Ngày nhập" align="center" width="100">
          <template #default="scope">
            <div
              class="time-info"
              v-if="commonJs.checkDateIsToday(scope.row.RefDate)"
            >
              <div style="color: #008000"><b>Hôm nay</b></div>
              <div>{{ commonJs.getTime(scope.row.RefDate) }}</div>
            </div>
            <div
              v-else
              v-html="commonJs.formatDateTimeHTML(scope.row.RefDate)"
            ></div>
          </template>
        </m-column>
        <m-column prop="AccountObjectName" label="Nhà phân phối"></m-column>
        <m-column
          prop="ContactName"
          label="Nhân viên liên hệ"
          width="220"
        ></m-column>
        <m-column
          prop="ContactPhoneNumber"
          label="Số điện thoại"
          width="150"
        ></m-column>
        <m-column
          prop="TotalAmount"
          label="Tổng tiền"
          align="right"
          width="200"
        >
          <template #default="scope">
            <div class="total-money-info">
              <el-tag>
                <div>
                  <b>{{ commonJs.formatMoney(scope.row.TotalAmount) }}</b>
                </div>
              </el-tag>
            </div>
          </template>
        </m-column>
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
                <i class="icofont-ui-delete"></i>
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
  <!-- <StockReceiptDetail v-if="showDetail"></StockReceiptDetail> -->
  <router-view name="StockReceiptRV"></router-view>
</template>

<script>
/* eslint-disable */
import router from "@/router";
import debounce from "@/scripts/debounce";
// import StockReceiptDetail from "./StockReceiptDetail.vue"
export default {
  name: "StockReceipt",
  // components: { StockReceiptDetail },
  watch: {
    keySearch: debounce(function () {
      this.loadData();
    }, 500),
    // eslint-disable-next-line
    $route(to, from) {
      if (from.path = "/stocks/receipt/create") {
        this.loadData();
      }
    },
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
          `refs?refType=2095&limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.refs = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      // this.showDetail = true;
      this.idSelected = null;
      router.push(`/stocks/receipt/create`);
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
        }
      );
    },
    onCloseDetail() {
      this.showDetail = false;
      this.loadData();
    },
    onClickRefItem(ref) {
      // this.showDetail = true;
      router.push(`/stocks/receipt/${ref.RefId}`);
    },
  },
  data() {
    return {
      refs: [],
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
