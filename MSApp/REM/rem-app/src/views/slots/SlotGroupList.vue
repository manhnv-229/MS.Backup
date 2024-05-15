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
          <i class="icofont-ui-add"></i> Thêm Nhóm
        </button>
      </div>
    </template>
    <!-- BẢNG DỮ LIỆU -->
    <template v-slot:container>
      <m-table
        ref="tbSlotGroups"
        :data="slotGroups"
        empty-text="Không có dữ liệu"
        width="100%"
        height="100%"
      >
        <m-column type="index" label="#" width="50px"></m-column>
        <m-column prop="SlotGroupName" label="Tên Nhóm"></m-column>
        <m-column prop="BranchName" width="100px" label="Chi nhánh"></m-column>
        <m-column
          prop="BilledByHours"
          label="Tính tiền theo giờ"
          width="150"
          align="center"
        >
          <template #default="scope">
            <div class="flex-center">
              <m-checkbox
                :disabled="true"
                v-model="scope.row.BilledByHours"
              ></m-checkbox>
            </div>
          </template>
        </m-column>
        <m-column prop="PriceByHour" label="Giá/giờ" width="100" align="right">
          <template #default="scope">
            <el-tag>
              {{ commonJs.formatMoney(scope.row.PriceByHour) }}
            </el-tag>
          </template>
        </m-column>
        <m-column prop="Description" label="Mô tả"></m-column>
        <m-column width="100" fixed="right">
          <template #default="scope">
            <div class="button-column">
              <button
                class="btn--table-mini --color-edit"
                :title="scope.row.SlotGroupName"
                @click="onUpdate(scope.row)"
              >
                <i class="icofont-ui-edit"></i>
              </button>
              <button
                class="btn--table-mini --color-red"
                :title="scope.row.SlotGroupName"
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
    <template v-slotGroup:footer>
      <m-paging
        entityName="Nhóm"
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
  <SlotGroupDetail
    v-if="showDetail"
    @onClose="onCloseDetail"
    :id="idSelected"
  ></SlotGroupDetail>
</template>
<script>
import debounce from "../../scripts/debounce";
import SlotGroupDetail from "./SlotGroupDetail.vue";
export default {
  name: "SlotGroupList",
  components: { SlotGroupDetail },
  emits: [],
  props: [],
  created() {
    this.loadData();
  },
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
          `slot-groups/paging?limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.slotGroups = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      this.showDetail = true;
      this.idSelected = null;
    },
    onUpdate(slotGroup) {
      this.idSelected = slotGroup.SlotGroupId;
      this.showDetail = true;
    },
    onDelete(slotGroup) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${slotGroup.SlotGroupName}] không?`,
        () => {
          this.maxios.delete(`slot-groups/${slotGroup.SlotGroupId}`);
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
      slotGroups: [],
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
<style scoped>
.flex-center {
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
