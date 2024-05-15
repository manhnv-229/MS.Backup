<template>
  <m-page title="Hóa đơn bán hàng/ dịch vụ" :rightSlot="true">
    <template v-slot:container>
      <m-table-layout>
        <!-- TOOLBAR -->
        <template v-slot:toolbar>
          <div class="toolbar__left">
            <m-input-search
              width="300px"
              placeholder="Tìm kiếm"
              v-model="keySearch"
              @blur="onSearchInvoice"
            ></m-input-search>
          </div>
          <div class="toolbar__right">
            <button class="btn btn-refresh" @click="loadData()">
              <i class="icofont-refresh"></i>
            </button>
            <button class="btn btn--default btn--refresh" @click="onAddNew">
              <i class="icofont-ui-add"></i> Thêm hóa đơn
            </button>
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->
        <template v-slot:container>
          <m-table
            ref="tbProducts"
            :data="invoices"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column prop="InvoiceCode" label="Mã hóa đơn" width="100px">
              <template #default="scope">
                <div>{{ scope.row.InvoiceCode }}</div>
                <div class="employee-name" v-if="scope.row.LastName">
                  ({{ scope.row.LastName }})
                </div>
              </template>
            </m-column>
            <m-column prop="ProductList" min-width="200px" label="SP/DV">
              <template #default="scope">
                <div class="product-list">
                  <div
                    class="product-list-item"
                    v-for="(item, index) in processProductList(
                      scope.row.ProductList
                    )"
                    :key="index"
                  >
                    <span
                      >-
                      <span class="product-name"
                        >{{ item.ProductName }}:</span
                      ></span
                    ><span class="product-money">+{{ item.Total || 0 }}</span>
                  </div>
                </div>
              </template>
            </m-column>
            <m-column
              prop="InvoiceDate"
              width="100px"
              label="Ngày lập"
              align="center"
            >
              <template #default="scope">
                <div
                  v-html="commonJs.formatDateTimeHTML(scope.row.InvoiceDate)"
                ></div>
              </template>
            </m-column>
            <m-column
              label="Tổng tiền"
              width="100px"
              fixed="right"
              align="right"
            >
              <template #default="scope">
                <el-tag
                  ><div>
                    <b>{{ commonJs.formatMoney(scope.row.TotalMoney) }}</b>
                  </div></el-tag
                >
              </template>
            </m-column>
            <m-column width="100" fixed="right" align="right">
              <template #default="scope">
                <div class="button-column">
                  <button
                    class="btn--table-mini --color-green"
                    :title="scope.row.InvoiceName"
                    @click="onPrint(scope.row)"
                  >
                    <i class="icofont-printer"></i>
                  </button>
                  <button
                    class="btn--table-mini --color-edit"
                    :title="scope.row.InvoiceName"
                    @click="onUpdate(scope.row)"
                  >
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button
                    class="btn--table-mini --color-red"
                    :title="scope.row.InvoiceName"
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
            entityName="hóa đơn"
            :totalRecords="totalRecords"
            :pageSizeData="[10, 20, 50, 100, 200, 500]"
            v-model:limit="limit"
            v-model:offset="offset"
            @onChangePagesize="onChangePagesize"
            @onPrevPage="onPrevPage"
            @onNextPage="onNextPage"
            @onInitPaging="onInitPaging"
          ></m-paging>
        </template>
      </m-table-layout>
    </template>
    <template v-slot:footer></template>
  </m-page>
  <router-view
    name="InvoiceDetailView"
    v-model:isReload="isReload"
  ></router-view>
  <router-view name="InvoicePrintFromMobile"></router-view>
  <invoice-print
    v-if="showPrint"
    :id="invoiceIdSelected"
    @onClose="showPrint = false"
  ></invoice-print>
</template>
<script>
/* eslint-disable */
import router from "@/router";
import debounce from "@/scripts/debounce";
import InvoicePrint from "./InvoicePrint.vue";
export default {
  name: "InvoiceList",
  components: { InvoicePrint },
  emits: [],
  props: [],
  setup() {},
  created() {},
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
          `/invoices/paging?limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.invoices = res.Data;
          this.totalRecords = res.TotalRecords;
        });
      this.isReload = false;
    },
    onSearchInvoice() {},
    onAddNew() {
      router.push(`/invoices/create`);
    },
    onUpdate(invoice) {
      router.push(`/invoices/${invoice.InvoiceId}`);
    },
    onDelete(invoice) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa hóa đơn [${invoice.InvoiceCode}] không?`,
        () => {
          this.maxios.delete(`invoices/${invoice.InvoiceId}`, () => {
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
      this.invoiceIdSelected = invoice.InvoiceId;
      var isMobile = this.commonJs.mobileCheck();
      // var isTablet = this.commonJs.mobileCheck();
      // var isMobile = navigator.userAgentData.mobile;
      if (isMobile) {
        this.hubConnection.invoke("PrintFromMobile", this.invoiceIdSelected);
      }
      this.showPrint = true;
    },
    processProductList(productListJSON) {
      let products = JSON.parse(productListJSON);
      for (const item of products) {
        item.Total = this.commonJs.formatMoney(item.Total);
      }
      console.log("products:", products);
      return products;
    },
  },
  data() {
    return {
      invoices: [],
      totalRecords: 0,
      isReload: false,
      keySearch: "",
      invoiceIdSelected: null,
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

.product-list-item span.product-name::first-letter {
  text-transform: capitalize;
}

.product-money {
  color: green;
  font-weight: 700;
}

.employee-name {
  font-size: 11px;
  font-weight: 700;
}
</style>
