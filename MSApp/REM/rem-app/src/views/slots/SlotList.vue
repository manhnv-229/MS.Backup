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
          <i class="icofont-ui-add"></i> Thêm Bàn/Phòng
        </button>
      </div>
    </template>
    <!-- BẢNG DỮ LIỆU -->
    <template v-slot:container>
      <m-table
        ref="tbSlots"
        :data="slots"
        empty-text="Không có dữ liệu"
        width="100%"
        height="100%"
      >
        <m-column type="index" label="#" width="50px"></m-column>
        <m-column
          prop="SlotCode"
          width="150px"
          label="Tên Bàn/Phòng"
        ></m-column>
        <m-column prop="SlotName" label="Tên Bàn/Phòng"></m-column>
        <m-column prop="SlotGroupName" label="Khu vực/Nhóm"></m-column>
        <m-column
          prop="BilledByHours"
          label="Tính tiền theo giờ"
          width="120"
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
        <!-- <m-column prop="Description" label="Mô tả"></m-column> -->
        <m-column width="100" fixed="right" align="center">
          <template #default="scope">
            <div class="button-column">
              <button
                class="btn--table-mini --color-edit"
                :title="scope.row.SlotName"
                @click="onUpdate(scope.row)"
              >
                <i class="icofont-ui-edit"></i>
              </button>
              <button
                class="btn--table-mini --color-red"
                :title="scope.row.SlotName"
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
        entityName="Bàn/Phòng"
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
  <SlotDetail
    v-if="showDetail"
    @onClose="onCloseDetail"
    :id="idSelected"
  ></SlotDetail>
</template>
<script>
import debounce from "../../scripts/debounce";
import SlotDetail from "./SlotDetail.vue";
export default {
  name: "SlotList",
  components: { SlotDetail },
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
          `slots/paging?limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.slots = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      this.showDetail = true;
      this.idSelected = null;
    },
    onUpdate(slot) {
      this.idSelected = slot.SlotId;
      this.showDetail = true;
    },
    onDelete(slot) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${slot.SlotName}] không?`,
        () => {
          this.maxios.delete(`slots/${slot.SlotId}`);
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
      slots: [],
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
