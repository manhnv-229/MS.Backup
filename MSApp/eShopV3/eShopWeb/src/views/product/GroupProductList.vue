<template>
  <m-page title="Danh mục nhóm Sản phẩm, nhóm Dịch vụ" :rightSlot="true">
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
            ref="tbGroupProducts"
            :data="groupProducts"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column prop="RowIndex" label="#" width="50px"></m-column>
            <m-column prop="GroupProductName" label="Tên nhóm"></m-column>
            <m-column prop="Description" label="Mô tả"></m-column>
            <m-column width="50" fixed="right">
              <template #default="scope">
                <div v-if="scope.row.IsSystem != 1" class="button-column">
                  <button
                    class="btn--table-mini --color-edit"
                    :title="scope.row.GroupProductName"
                    @click="onUpdate(scope.row)"
                  >
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button
                    class="btn--table-mini --color-red"
                    :title="scope.row.GroupProductName"
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
            entityName="nhóm sản phẩm/dịch vụ"
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
  <GroupProductDetail
    v-if="showDetail"
    @onClose="onCloseDetail"
    :id="idSelected"
  ></GroupProductDetail>
</template>
<script>
import debounce from "@/scripts/debounce";
import GroupProductDetail from "./GroupProductDetail.vue";
export default {
  name: "GroupProductList",
  components: { GroupProductDetail },
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
          `groupProducts/paging?limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.groupProducts = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      this.showDetail = true;
      this.idSelected = null;
    },
    onUpdate(groupProduct) {
      this.idSelected = groupProduct.GroupProductId;
      this.showDetail = true;
    },
    onDelete(groupProduct) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${groupProduct.GroupProductName}] không?`,
        () => {
          this.maxios.delete(`groupProducts/${groupProduct.GroupProductId}`);
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
      groupProducts: [],
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
