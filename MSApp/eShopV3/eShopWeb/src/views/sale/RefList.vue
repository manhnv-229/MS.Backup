<template>
  <m-page title="Hóa đơn bán hàng/ dịch vụ" :rightSlot="true">
    <template v-slot:container>
      <m-table-layout>
        <!-- TOOLBAR -->
        <template v-slot:toolbar>
          <div class="toolbar__left">
            <m-input-search width="300px" placeholder="Tìm kiếm" v-model="keySearch"
              @blur="onSearchInvoice"></m-input-search>
          </div>
          <div class="toolbar__right">
            <button class="btn btn-refresh" @click="loadData()">
              <i class="icofont-refresh"></i>
            </button>
            <button class="btn btn--default btn--refresh" @click="onAddNew">
              <i class="icofont-ui-add"></i> BÁN HÀNG
            </button>
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->

        <template v-slot:container>
          <m-table ref="tbProducts" :data="refs" empty-text="Không có dữ liệu" width="100%" height="100%">
            <m-column prop="RefNo" label="Mã hóa đơn" width="100px">
              <template #default="scope">
                <div>{{ scope.row.RefNo }}</div>
                <div class="employee-name" v-if="scope.row.LastName">
                  ({{ scope.row.LastName }})
                </div>
              </template>
            </m-column>
            <m-column prop="RefDate" width="150px" label="Ngày lập" align="center">

              <template #default="scope">
                <div class="time-info" v-if="commonJs.checkDateIsToday(scope.row.RefDate)">
                  <div style="color: #008000;"><b>Hôm nay</b></div>
                  <div>{{ commonJs.getTime(scope.row.RefDate) }}</div>
                </div>
                <div v-else v-html="commonJs.formatDateTimeHTML(scope.row.RefDate)"></div>
              </template>
            </m-column>
            <m-column prop="InventoryItemList" min-width="200px" label="SP/DV">

              <template #default="scope">
                <div class="product-list">
                  <div class="product-list-item" v-for="(item, index) in processProductList(
    scope.row.InventoryItemList
  )" :key="index">
                    <span class="product-name">
                      <b>({{ item.Quantity }}) </b>
                      <el-tooltip class="box-item" effect="customized"
                        :content="`${item.InventoryItemName} (SL: ${item.Quantity} x ${commonJs.formatMoney(item.UnitPrice)} = ${item.Total})`"
                        placement="top">
                        {{ item.InventoryItemName }}
                      </el-tooltip>
                      <!-- <b>({{ item.Quantity }})</b>{{ item.InventoryItemName }}: -->
                    </span>
                    <span class="product-money">+{{ item.Total || 0 }}</span>
                  </div>
                </div>
              </template>
            </m-column>
            <m-column label="Tổng tiền" align="left">

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
            <m-column prop="CustomerName" width="150px" label="Khách hàng" align="left">

              <template #default="scope">
                <div v-if="scope.row.CustomerName && scope.row.CustomerName != ''">
                  <el-tooltip v-if="scope.row.CustomerAddress" class="box-item" effect="customized"
                    :content="scope.row.CustomerAddress" placement="top">
                    {{ scope.row.CustomerName }}
                  </el-tooltip>
                  <div v-else>{{ scope.row.CustomerName }}</div>
                </div>
                <div v-else style="font-style: italic;">(Khách vãng lai)</div>
              </template>
            </m-column>

            <m-column prop="FullName" width="150px" label="Nhân viên bán hàng" align="left"></m-column>
            <m-column prop="PaymentStatus" width="150px" label="Tình trạng" align="left">

              <template #default="scope">
                <div v-if="scope.row.PaymentStatus == 0" style="color: #ff0000;">Chưa thanh toán</div>
                <div v-else style="color: #008000;">Đã thanh toán</div>
              </template>
            </m-column>
            <m-column width="100" fixed="right" align="right">

              <template #default="scope">
                <div class="button-column">
                  <button class="btn--table-mini --color-green" :title="scope.row.RefNo" @click="onPrint(scope.row)">
                    <i class="icofont-printer"></i>
                  </button>
                  <button class="btn--table-mini --color-edit" :title="scope.row.RefNo" @click="onUpdate(scope.row)">
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button class="btn--table-mini --color-red" :title="scope.row.RefNo" @click="onDelete(scope.row)">
                    <i class="icofont-ui-delete"></i>
                  </button>
                </div>
              </template>
            </m-column>
          </m-table>
        </template>
        <!-- PHÂN TRANG -->

        <template v-slot:footer>
          <m-paging entityName="hóa đơn" :totalRecords="totalRecords" :pageSizeData="[20, 50, 100, 200, 500]"
            v-model:limit="limit" v-model:offset="offset" @onChangePagesize="onChangePagesize" @onPrevPage="onPrevPage"
            @onNextPage="onNextPage" @onInitPaging="onInitPaging"></m-paging>
        </template>
      </m-table-layout>
    </template>

    <template v-slot:footer></template>
  </m-page>
  <router-view name="InvoiceDetailView" v-model:isReload="isReload"></router-view>
  <router-view name="InvoicePrintFromMobile"></router-view>
  <invoice-print v-if="showPrint" :id="refIdSelected" @onClose="showPrint = false"></invoice-print>
</template>

<script>
/* eslint-disable */
import router from "@/router";
import debounce from "@/scripts/debounce";
import InvoicePrint from "./InvoicePrint.vue";
import RefDetail from "./RefDetail.vue";
export default {
  name: "RefList",
  components: { InvoicePrint },
  emits: [],
  props: [],
  setup() { },
  created() { },
  watch: {
    isReload(newValue) {
      if (newValue) {
        this.loadData();
      }
    },
    keySearch: debounce(function () {
      this.loadData();
    }, 500),
  },
  // beforeRouteEnter(to, from, next) {
  //   console.log(`to`, to);
  //   console.log(`from`, from);
  //   console.log(`next`, next);
  //   next(vm=>vm.loadData());
  // },
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
          `/refs/paging?limit=${this.limit}&offset=${this.offset}&key=${key || ""
          }`
        )
        .then((res) => {
          this.refs = res.Data;
          this.totalRecords = res.TotalRecords;
        });
      this.isReload = false;
    },
    onSearchInvoice() { },
    onAddNew() {
      router.push(`/refs/create`);
    },
    onUpdate(invoice) {
      router.push(`/refs/${invoice.RefId}`);
    },
    onDelete(invoice) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa hóa đơn [${invoice.RefNo}] không?`,
        () => {
          this.maxios.delete(`refs/${invoice.RefId}`, () => {
            this.loadData();
          });
        }
      );
    },
    onChangePagesize(pageSize) {
      this.loadData();
    },
    onPrevPage(startIndex, endIndex, pageSize) {
      this.loadData();
    },
    onNextPage(startIndex, endIndex, pageSize) {
      this.loadData();
    },
    onPrint(invoice) {
      this.refIdSelected = invoice.RefId;
      var isMobile = this.commonJs.mobileCheck();
      // var isTablet = this.commonJs.mobileCheck();
      // var isMobile = navigator.userAgentData.mobile;
      if (isMobile) {
        this.hubConnection.invoke("PrintFromMobile", this.refIdSelected);
      }
      this.showPrint = true;
    },
    processProductList(productListJSON) {
      let products = JSON.parse(productListJSON);
      for (const item of products) {
        item.Total = this.commonJs.formatMoney(item.Total);
      }
      return products;
    },
  },
  data() {
    return {
      refs: [],
      totalRecords: 0,
      isReload: false,
      keySearch: "",
      refIdSelected: null,
      showPrint: false,
      limit: 0,
      offset: 0,
    };
  },
};
</script>

<style scoped>
ul {
  margin-block-start: 0;
  margin-block-end: 0;
  margin-inline-start: 0px;
  margin-inline-end: 0px;
  padding-inline-start: 16px;
}

.product-list {
  display: flex;
  flex-direction: column;
  overflow: hidden;
  text-overflow: ellipsis;
  word-break: break-all;
  line-height: 23px;
}

.product-list-item {
  width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  word-break: break-all;
  line-height: 23px;
  box-sizing: border-box;
  white-space: nowrap;
  display: flex;
  align-items: center;
  justify-content: space-between;
  column-gap: 10px;
}

.product-list-item span {
  display: inline-block;
}

.product-list-item .product-name {
  display: block;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  flex: 1;
}

.product-list-item .product-name::first-letter {
  text-transform: capitalize;
}

.total-money-info {
  padding-right: 10px;
}

.product-money {
  color: green;
  font-weight: 700;
  flex-grow: 0;
  flex-shrink: 0;
  flex-basis: 50px;
}

.employee-name {
  font-size: 11px;
  font-weight: 700;
}
</style>
