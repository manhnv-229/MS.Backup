<template>
  <m-page title="Danh mục dịch vụ" :rightSlot="true">
    <!-- <template v-slot:header-right> </template> -->
    <template v-slot:container>
      <m-table-layout>
        <!-- TOOLBAR -->
        <template v-slot:toolbar>
          <div class="toolbar__left">
            <m-input-search
              width="400px"
              placeholder="Tìm kiếm"
              v-model="keySearch"
            ></m-input-search>
          </div>
          <div class="toolbar__right">
            <button class="btn btn-refresh" @click="loadData()">
              <i class="icofont-refresh"></i>
            </button>
            <button class="btn btn--default btn--refresh" @click="onAddNew">
              <i class="icofont-ui-add"></i> Thêm dịch vụ
            </button>
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->
        <template v-slot:container>
          <m-table
            ref="tbInventories"
            :data="services"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column label="Hình ảnh" width="100" align="left">
              <template #default="scope">
                <div class="inventory-img">
                  <img :src="getImgFullPath(scope.row.ImgPath)" alt="" />
                </div>
              </template>
            </m-column>
            <m-column
              prop="ServiceCode"
              label="Mã dịch vụ"
              width="100px"
            ></m-column>
            <m-column prop="ServiceName" width="250px" label="Tên dịch vụ">
              <template #default="scope">
                <router-link
                  :to="{
                    name: 'ServiceUpdateRouter',
                    params: { id: scope.row.ServiceId },
                  }"
                  class="name__link"
                >
                  <el-tooltip
                    class="box-item"
                    effect="customized"
                    :content="scope.row.ServiceName"
                    placement="top"
                  >
                    {{ scope.row.ServiceName }}
                  </el-tooltip>
                </router-link>
              </template>
            </m-column>
            <m-column prop="ServiceGroupName" label="Nhóm dịch vụ"></m-column>
            <m-column prop="Description" label="Mô tả"></m-column>
            <m-column label="Đơn giá" width="80px" align="right">
              <template #default="scope">
                <div class="price">
                  <span class="price-text">
                    <b>{{ commonJs.formatMoney(scope.row.UnitPrice) }}</b>
                  </span>
                </div>
              </template>
            </m-column>
            <m-column width="80" fixed="right">
              <template #default="scope">
                <div class="button-column">
                  <button
                    class="btn--table-mini --color-edit"
                    :title="scope.row.ServiceName"
                    @click="onUpdate(scope.row)"
                  >
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button
                    class="btn--table-mini --color-red"
                    :title="scope.row.ServiceName"
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
            entityName="dịch vụ"
            :totalRecords="totalRecords"
            :pageSizeData="[50, 100, 200]"
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
  <router-view name="ServiceDetailView"></router-view>
</template>
<script>
import router from "../../router";
import debounce from "../../scripts/debounce";
export default {
  name: "InventoryList",
  emits: [],
  props: [],
  created() {
    this.$emitter.on("reloadData", this.loadData);
  },
  beforeUnmount() {
    this.$emitter.off("reloadData");
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
          `services/paging?limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.services = res.Data;
          this.totalRecords = res.TotalRecords;
          this.serverFileUrl = res.ServerFileUrl;
        });
    },
    onAddNew() {
      router.replace(`list/create`);
    },
    onUpdate(service) {
      router.push(`list/${service.ServiceId}`);
      console.log(service);
    },
    async onDelete(service) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${service.ServiceName}] không?`,
        () => {
          this.maxios.delete(`services/${service.ServiceId}`, () => {
            this.loadData();
          });
        }
      );
    },
    getImgFullPath(imgPath) {
      if (imgPath) {
        return `${this.serverFileUrl}/${imgPath}`;
      } else {
        return null;
      }
    },
  },
  data() {
    return {
      services: [],
      serverFileUrl: null,
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
.price {
  font-weight: 700;
  text-align: right;
}

.price-text {
  padding: 4px 6px;
  border-radius: 4px;
  border: 1px solid #67c23a;
  background-color: #67c23a;
  color: #ffffff;
}

.name__link {
  color: #0062cc;
  text-decoration: unset;
}

.name__link:hover {
  cursor: pointer;
  text-decoration: underline;
}
</style>
