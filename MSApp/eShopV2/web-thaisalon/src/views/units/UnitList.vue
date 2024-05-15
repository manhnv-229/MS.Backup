<template>
  <m-page title="Danh mục đơn vị tính" :rightSlot="true">
    <template v-slot:header-right>
      <button class="btn btn--default btn--refresh" @click="onAddNew">
        <i class="icofont-ui-add"></i> Thêm đơn vị
      </button>
    </template>
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
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->
        <template v-slot:container>
          <m-table
            ref="tbUnits"
            :data="units"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column type="index" label="#" width="50px"></m-column>
            <m-column prop="UnitName" label="Tên đơn vị"></m-column>
            <m-column prop="Description" label="Mô tả"></m-column>
            <m-column width="50" fixed="right">
              <template #default="scope">
                <div v-if="scope.row.IsSystem == 0" class="button-column">
                  <button
                    class="btn--table-mini --color-edit"
                    :title="scope.row.UnitName"
                    @click="onUpdate(scope.row)"
                  >
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button
                    class="btn--table-mini --color-red"
                    :title="scope.row.UnitName"
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
            entityName="đơn vị tính"
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
  <UnitDetail
    v-if="showDetail"
    @onClose="onCloseDetail"
    :id="idSelected"
  ></UnitDetail>
</template>
    <script>
import debounce from "@/scripts/debounce";
import UnitDetail from "./UnitDetail.vue";
export default {
  name: "UnitList",
  components: { UnitDetail },
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
          `units/paging?limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.units = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      this.showDetail = true;
      this.idSelected = null;
    },
    onUpdate(unit) {
      this.idSelected = unit.UnitId;
      this.showDetail = true;
    },
    onDelete(unit) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${unit.UnitName}] không?`,
        () => {
          this.maxios.delete(`units/${unit.UnitId}`);
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
      units: [],
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
    