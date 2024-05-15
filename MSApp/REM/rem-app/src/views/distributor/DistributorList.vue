<template>
  <m-page title="Quản lý Nhà phân phối" :rightSlot="true">
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
              <i class="icofont-ui-add"></i> Thêm Nhà phân phối
            </button>
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->
        <template v-slot:container>
          <m-table
            ref="tbDistributors"
            :data="distributors"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column prop="RowIndex" label="#" width="50"></m-column>
            <m-column
              prop="DistributorName"
              label="Tên Nhà phân phối"
            ></m-column>
            <m-column
              prop="PhoneNumber"
              label="SĐT công ty"
              width="220"
              align="left"
            >
            </m-column>
            <m-column
              prop="ContactName"
              label="Nhân viên liên hệ"
              width="220"
            ></m-column>
            <m-column
              prop="ContactPhoneNumber"
              label="SĐT liên hệ"
              width="220"
            ></m-column>
            <m-column prop="Address" label="Địa chỉ"></m-column>
            <m-column width="80" fixed="right">
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
                    :title="scope.row.DistributorName"
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
            entityName="Nhà phân phối"
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
  <distributor-detail
    v-if="showDetail"
    :id="distributorId"
    @onClose="onCloseDetail"
  ></distributor-detail>
</template>
<script>
import debounce from "../../scripts/debounce";
import DistributorDetail from "./DistributorDetail.vue";
export default {
  name: "DistributorList",
  components: { DistributorDetail },
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
          `distributors/paging?limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.distributors = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      this.showDetail = true;
    },
    onUpdate(distributor) {
      this.distributorId = distributor.DistributorId;
      this.showDetail = true;
    },
    onDelete(distributor) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${distributor.FullName}] không?`,
        () => {
          this.maxios.delete(`distributors/${distributor.DistributorId}`);
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
      distributors: [],
      totalRecords: 0,
      isReload: false,
      distributorId: null,
      keySearch: "",
      limit: 0,
      offset: 0,
      showDetail: false,
    };
  },
};
</script>
