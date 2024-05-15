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
            ref="tbServiceGroups"
            :data="serviceGroups"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column prop="RowIndex" label="#" width="50px"></m-column>
            <m-column prop="ServiceGroupName" label="Tên nhóm"></m-column>
            <m-column prop="Description" label="Mô tả"></m-column>
            <m-column width="80" fixed="right">
              <template #default="scope">
                <div v-if="scope.row.IsSystem != 1" class="button-column">
                  <button
                    class="btn--table-mini --color-edit"
                    :title="scope.row.ServiceGroupName"
                    @click="onUpdate(scope.row)"
                  >
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button
                    class="btn--table-mini --color-red"
                    :title="scope.row.ServiceGroupName"
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
  <router-view name="ServiceGroupDetailView"></router-view>
</template>
<script>
import debounce from "../../scripts/debounce";
import router from "@/router";
export default {
  name: "ServiceGroupList",
  components: {},
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
          `service-groups/paging?limit=${this.limit}&offset=${
            this.offset
          }&key=${key || ""}`
        )
        .then((res) => {
          this.serviceGroups = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      router.replace('groups/create');
    },
    onUpdate(serviceGroup) {
      router.replace(`groups/${serviceGroup.ServiceGroupId}`);
    },
    onDelete(serviceGroup) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${serviceGroup.ServiceGroupName}] không?`,
        () => {
          this.maxios
            .delete(`service-groups/${serviceGroup.ServiceGroupId}`)
            .then((res) => {
              this.loadData();
            });
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
      serviceGroups: [],
      totalRecords: 0,
      isReload: false,
      keySearch: "",
      limit: 0,
      offset: 0,
      idSelected: null,
    };
  },
};
</script>
<style scoped></style>
