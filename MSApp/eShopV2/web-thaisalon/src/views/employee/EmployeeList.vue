<template>
  <m-page title="QUẢN LÝ NHÂN VIÊN" :rightSlot="true">
    <template v-slot:header-right>
      <button class="btn btn--default btn--refresh" @click="onAddNew">
        <i class="icofont-ui-add"></i> Thêm mới
      </button>
    </template>
    <template v-slot:container>
      <m-table-layout>
        <!-- TOOLBAR -->
        <template v-slot:toolbar>
          <div class="toolbar__left">
            <m-input-search
              width="200px"
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
            ref="tbListDocument"
            :data="employees"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column prop="FullName" label="#" width="68px">
              <template #default="scope">
                <div
                  class="avatar"
                  :style="{
                    'background-image': `url(${
                      scope.row.AvatarFullPath ||
                      'https://file.nmanh.com/e-contact/Content/imgs/avatar.png'
                    })`,
                  }"
                ></div>
              </template>
            </m-column>
            <m-column prop="FullName" label="Họ và tên"></m-column>
            <m-column label="Số điện thoại">
              <template #default="scope">
                <div>{{ scope.row.Mobile }}</div>
              </template>
            </m-column>
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
                    :title="scope.row.FullName"
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
            entityName="Nhân viên"
            :totalRecords="totalRecords"
            :pageSizeData="[10, 20, 50, 100, 200]"
            :pageSizeDefault="20"
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
  <employee-detail
    v-if="showDetail"
    @onClose="resetDefault"
    :id="employeeUpdateId"
    @onSaveSuccess="onSaveSuccess"
  ></employee-detail>
</template>
<script>
import debounce from "@/scripts/debounce";
import EmployeeDetail from "./EmployeeDetail.vue";
export default {
  name: "EmployeeList",
  components: { EmployeeDetail },
  emits: [],
  props: {},
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
          `employees/paging?limit=${this.limit}&offset=${this.offset}&key=${key || ""}`
        )
        .then((res) => {
          this.employees = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      this.showDetail = true;
      this.employeeUpdateId = null;
    },
    onUpdate(employee) {
      this.showDetail = true;
      this.employeeUpdateId = employee.EmployeeId;
    },
    onSaveSuccess() {
      this.loadData();
      this.resetDefault();
    },
    onDelete(employee) {
      this.commonJs.showConfirm("Bạn có chắc chắn muốn xóa nhân viên này?", () => {
        this.maxios.delete(`/employees/${employee.EmployeeId}`);
      });
    },
    resetDefault() {
      this.showDetail = false;
      this.employeeUpdateId = null;
    },
  },
  data() {
    return {
      employees: [],
      showDetail: false,
      employeeUpdateId: null,
      totalRecords: 0,
      isReload: false,
      keySearch: "",
      limit: 0,
      offset: 0,
    };
  },
};
</script>
<style scoped>
.avatar {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  background-size: cover;
}
</style>
