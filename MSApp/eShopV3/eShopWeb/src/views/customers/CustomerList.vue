<template>
  <m-page title="Quản lý khách hàng" :rightSlot="true">
    <!-- <template v-slot:header-right> </template> -->
    <template v-slot:container>
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
              <i class="icofont-ui-add"></i> Thêm khách hàng
            </button>
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->
        <template v-slot:container>
          <m-table
            ref="tbCustomers"
            :data="customers"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column prop="CustomerCode" label="#" width="100px"></m-column>
            <m-column prop="FullName" label="Tên khách hàng"></m-column>
            <m-column prop="Mobile" label="SĐT" align="left"> </m-column>
            <m-column width="50" fixed="right">
              <template #default="scope">
                <div class="button-column">
                  <button
                    class="btn--table-mini --color-edit"
                    :title="scope.row.FullName"
                    @click="onUpdate(scope.row)"
                  >
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button
                    class="btn--table-mini --color-red"
                    :title="scope.row.CustomerName"
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
            entityName="khách hàng"
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
    </template>
    <template v-slot:footer> </template>
  </m-page>
  <customer-detail
    v-if="showDetail"
    :id="customerId"
    @onClose="onCloseDetail"
  ></customer-detail>
</template>
<script>
import debounce from "@/scripts/debounce";
import CustomerDetail from "./CustomerDetail.vue";
export default {
  name: "CustomerList",
  components: { CustomerDetail },
  emits: [],
  props: {},
  watch: {
    keySearch: debounce(function () {
      this.loadData();
    }, 500),
  },
  created() {},
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
          `customers/paging?limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.customers = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      this.showDetail = true;
    },
    onUpdate(customer) {
      this.customerId = customer.CustomerId;
      this.showDetail = true;
    },
    onDelete(customer) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${customer.FullName}] không?`,
        () => {
          this.maxios.delete(`customers/${customer.CustomerId}`);
          this.loadData();
        }
      );
    },
    onCloseDetail() {
      this.showDetail = false;
      this.loadData();
    },
  },
  data() {
    return {
      customers: [],
      totalRecords: 0,
      isReload: false,
      customerId: null,
      keySearch: "",
      limit: 0,
      offset: 0,
      showDetail: false,
    };
  },
};
</script>
