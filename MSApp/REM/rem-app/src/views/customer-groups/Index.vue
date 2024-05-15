<template>
  <m-page title="Danh mục nhóm khách hàng" :rightSlot="true">
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
              <i class="icofont-ui-add"></i> Thêm nhóm khách hàng
            </button>
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->
        <template v-slot:container>
          <m-table
            ref="tbCustomerGroups"
            :data="customerGroups"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column type="index" label="#" width="100px"></m-column>
            <m-column prop="CustomerGroupName" label="Tên nhóm"></m-column>
            <m-column prop="Description" label="Mô tả"></m-column>
            <m-column width="80" fixed="right">
              <template #default="scope">
                <div class="button-column">
                  <button
                    class="btn--table-mini --color-edit"
                    :title="scope.row.CustomerGroupName"
                    @click="onUpdate(scope.row)"
                  >
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button
                    class="btn--table-mini --color-red"
                    :title="scope.row.CustomerGroupName"
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
            entityName="nhóm khách hàng"
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
  <CustomerGroupDetail
    v-if="showDetail"
    @onClose="onCloseDetail"
    :id="idSelected"
  ></CustomerGroupDetail>
</template>
<script>
import debounce from "../../scripts/debounce";
import CustomerGroupDetail from "./CustomerGroupDetail.vue";
export default {
  name: "ProductList",
  components: { CustomerGroupDetail },
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
          `customergroups/paging?limit=${this.limit}&offset=${
            this.offset
          }&key=${key || ""}`
        )
        .then((res) => {
          this.customerGroups = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      this.showDetail = true;
    },
    onUpdate(customerGroup) {
      this.idSelected = customerGroup.CustomerGroupId;
      this.showDetail = true;
    },
    onDelete(customerGroup) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${customerGroup.CustomerGroupName}] không?`,
        () => {
          this.maxios.delete(`customergroups/${customerGroup.CustomerGroupId}`);
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
      customerGroups: [],
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
