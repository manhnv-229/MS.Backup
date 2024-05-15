<template>
  <m-page title="Triển khai" :rightSlot="true">
    <template #header> </template>
    <template #container>
      <m-table-layout>
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
            <MButtonIcon
              text="Reset Database Server Cache"
              cls="btn--default"
              iconCls="icon icon-20 icon--refresh"
              :disabled="stepIndexComplete < 5 || stepError > 0"
              @click="onResetDbServerCache"
            ></MButtonIcon>
            <button
              class="btn btn--default btn--refresh"
              @click="onRegisterClick"
            >
              <i class="icofont-ui-add"></i> Thêm thuê bao
            </button>
          </div>
        </template>
        <template v-slot:container>
          <m-table
            ref="tbTenants"
            :data="tenants"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column type="index" label="#" width="50"></m-column>
            <m-column
              prop="TenantCode"
              label="Mã thuê bao"
              width="150px"
            ></m-column>
            <m-column
              prop="OrganizationName"
              width="250px"
              label="Tên cửa hàng"
            >
              <template #default="scope">
                <el-tooltip
                  class="box-item"
                  effect="customized"
                  :content="scope.row.OrganizationName"
                  placement="top"
                >
                  {{ scope.row.OrganizationName }}
                </el-tooltip>
              </template>
            </m-column>
            <m-column width="250px" label="Tên miền">
              <template #default="scope">
                <a
                  :href="`https://${scope.row.SubDomain}.${scope.row.RootDomain}`"
                  target="_blank"
                  >{{ scope.row.SubDomain }}.{{ scope.row.RootDomain }}</a
                >
              </template>
            </m-column>
            <m-column
              prop="DatabaseName"
              label="Tên CSDL"
              width="150px"
            ></m-column>
            <m-column
              prop="OrganizationPhoneNumber"
              label="Số điện thoại"
            ></m-column>
            <m-column prop="Address" label="Địa chỉ"></m-column>
            <m-column width="80" fixed="right">
              <template #default="scope">
                <div class="button-column">
                  <button
                    class="btn--table-mini --color-edit"
                    :title="scope.row.TenantName"
                    @click="onUpdate(scope.row)"
                  >
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button
                    class="btn--table-mini --color-red"
                    :title="scope.row.TenantName"
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
            entityName="thuê bao"
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
    <template #footer> </template>
  </m-page>
</template>
<script>
import router from "@/router";
import debounce from "@/scripts/debounce";
export default {
  name: "DeployApp",
  components: {},
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
          `tenants/paging?limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.tenants = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onRegisterClick() {
      router.push({ name: "TenantRegisterRouter" });
    },
    onUpdate(tenant) {
      router.push(`tenants/${tenant.TenantId}`);
    },
    async onDelete(tenant) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${tenant.TenantName}]
          -[<a href='https://${tenant.SubDomain}.${tenant.RootDomain}' target='_blank'>${tenant.SubDomain}.${tenant.RootDomain}</a>] không?.<br/> 
          Lưu ý: tiến trình xoá sẽ không thể dừng lại, toàn bộ dữ liệu xoá sẽ <b>không thể khôi phục</b>.`,
        () => {
          this.lastTenantDelete = tenant;
          this.showDeleteProgress = true;
          this.maxios.delete(
            `tenants/${tenant.TenantId}`,
            () => {
              this.loadData();
            },
            true
          );
        }
      );
    },
    onDeleteFinish() {
      this.showDeleteProgress = false;
    },
    onResetDbServerCache(){
        var appCode="baza";
        this.maxios.post(`caches/app-db-caches/${appCode}/reset`)
        .then(res=>{
            console.log(res);
        })
    }
  },
  data() {
    return {
      lastTenantDelete: {},
      showDeleteProgress: false,
      totalRecords: 0,
      keySearch: "",
      limit: 0,
      offset: 0,
      tenants: [],
    };
  },
};
</script>
<style scoped></style>
