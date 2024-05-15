<template>
    <m-page title="Danh mục nhóm Hàng hoá" :rightSlot="true">
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
                <i class="icofont-ui-add"></i> Thêm nhóm
              </button>
            </div>
          </template>
          <!-- BẢNG DỮ LIỆU -->
          <template v-slot:container>
            <m-table
              ref="tbInventoryItemCategories"
              :data="inventoryItemCategories"
              empty-text="Không có dữ liệu"
              width="100%"
              height="100%"
            >
              <m-column prop="RowIndex" label="#" width="50px"></m-column>
              <m-column prop="InventoryItemCategoryCode" width="150px" label="Mã nhóm"></m-column>
              <m-column prop="InventoryItemCategoryName" label="Tên nhóm"></m-column>
              <m-column prop="Description" label="Mô tả"></m-column>
              <m-column width="50" fixed="right">
                <template #default="scope">
                  <div v-if="scope.row.IsSystem != 1" class="button-column">
                    <button
                      class="btn--table-mini --color-edit"
                      :title="scope.row.InventoryItemCategoryName"
                      @click="onUpdate(scope.row)"
                    >
                      <i class="icofont-ui-edit"></i>
                    </button>
                    <button
                      class="btn--table-mini --color-red"
                      :title="scope.row.InventoryItemCategoryName"
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
              entityName="nhóm hàng hoá"
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
    <InventoryItemCategoryDetail
      v-if="showDetail"
      @onClose="onCloseDetail"
      :id="idSelected"
    ></InventoryItemCategoryDetail>
  </template>
  <script>
  import debounce from "@/scripts/debounce";
  import InventoryItemCategoryDetail from "./InventoryItemCategoryDetail.vue";
  export default {
    name: "InventoryItemCategoryList",
    components: { InventoryItemCategoryDetail },
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
            `inventoryItemCategories/paging?limit=${this.limit}&offset=${this.offset}&key=${
              key || ""
            }`
          )
          .then((res) => {
            this.inventoryItemCategories = res.Data;
            this.totalRecords = res.TotalRecords;
          });
      },
      onAddNew() {
        this.showDetail = true;
        this.idSelected = null;
      },
      onUpdate(inventoryItemCategory) {
        this.idSelected = inventoryItemCategory.InventoryItemCategoryId;
        this.showDetail = true;
      },
      onDelete(inventoryItemCategory) {
        this.commonJs.showConfirm(
          `Bạn có chắc chắn muốn xóa [${inventoryItemCategory.InventoryItemCategoryName}] không?`,
          () => {
            this.maxios.delete(`inventoryItemCategories/${inventoryItemCategory.InventoryItemCategoryId}`);
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
        inventoryItemCategories: [],
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
  