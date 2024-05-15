<template>
  <m-page title="Danh sách doanh nghiệp" :rightSlot="true">
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
            <button
              class="btn-icon icon icon-24 icon--mail-statistic"
              title="GỬI THỐNG KÊ HÔM NAY"
              @click="sentStatistic()"
            ></button>
            <button
              class="btn-icon icon icon-20 icon--backup"
              title="SAO LƯU DỮ LIỆU"
              @click="backupDatabase()"
            ></button>
            <button
              class="btn btn-refresh"
              title="LẤY DỮ LIỆU MỚI"
              @click="loadData()"
            >
              <i class="icofont-refresh"></i>
            </button>
            <button class="btn btn--default btn--refresh" @click="onAddNew">
              <i class="icofont-ui-add"></i> Thêm đơn vị
            </button>
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->
        <template v-slot:container>
          <m-table
            ref="tbOrganizations"
            :data="organizations"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <!-- <m-column type="index" label="#" width="50px"></m-column> -->
            <el-table-column type="selection" width="30" fixed="left" />
            <m-column prop="OrganizationName" fixed="left" label="Tên cửa hàng">
              <template #default="scope">
                <div class="table-cell org-name">
                  <span
                    class="org-name__icon org--confirm"
                    v-if="scope.row.IsConfirm"
                    ><i class="icofont-check-circled"></i
                  ></span>
                  <span class="org-name__text">
                    {{ scope.row.OrganizationName }}
                  </span>
                </div>
                <div class="org-description">
                  {{ scope.row.Description }}
                </div>
              </template>
            </m-column>
            <m-column
              prop="Address"
              label="Địa chỉ"
              v-if="!isMobile"
            ></m-column>
            <m-column width="50" fixed="right">
              <template #default="scope">
                <div class="button-column">
                  <button
                    class="btn--table-mini --color-edit"
                    :title="scope.row.OrganizationName"
                    @click="onUpdate(scope.row)"
                  >
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button
                    v-if="!scope.row.IsConfirm"
                    class="btn--table-mini --color-red"
                    :title="scope.row.OrganizationName"
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
            entityName="cửa hàng"
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
  <div v-if="showDeleteProgress" class="delete-info">
    <div class="delete-item" v-for="(info, index) in deleteInfos" :key="index">
      <div v-if="info.IsDeleted" class="delete__icon delete__icon--checked">
        <i class="icofont-checked"></i>
      </div>
      <div v-else class="delete__icon delete__icon--unchecked">
        <i class="icofont-spinner"></i>
      </div>
      <div class="delete__text">
        <span v-if="info.IsDeleted">Đã xóa</span><span v-else>Đang xóa</span>
        <span>{{ info.ObjectText }}</span>
      </div>
    </div>
  </div>
</template>
<script>
import debounce from "@/scripts/debounce";
import router from "@/router";
import commonJs from "@/scripts/common";
export default {
  name: "OrganizationList",
  components: {},
  emits: [],
  props: [],
  computed: {
    isMobile: function () {
      if (this.innerWidth < 720) {
        return true;
      }
      return false;
    },
  },
  created() {
    this.innerWidth = window.innerWidth;
    window.addEventListener("resize", this.setWindownWith);
    this.deleteInfos_Original = JSON.stringify(this.deleteInfos);
    this.hubConnection.on(
      "RecieveRemoveOrganizationInfo",
      (progressName, isDone) => {
        for (const item of this.deleteInfos) {
          if (item.ObjectName == progressName) {
            item.IsDeleted = true;
            break;
          }
        }
        if (isDone) {
          this.loadData();
        }
      }
    );
  },
  watch: {
    keySearch: debounce(function () {
      this.loadData();
    }, 500),
  },
  methods: {
    setWindownWith() {
      var width = window.innerWidth;
      this.innerWidth = width;
    },
    onInitPaging(startIndex, endIndex, pageSize) {
      this.limit = pageSize;
      this.offset = startIndex - 1;
      this.loadData();
    },
    loadData() {
      var key = this.keySearch;
      this.maxios
        .get(
          `organizations/paging?limit=${this.limit}&offset=${this.offset}&key=${
            key || ""
          }`
        )
        .then((res) => {
          this.organizations = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      router.push(`/admin/organizations/create`);
    },
    onUpdate(organization) {
      router.push(`/admin/organizations/${organization.OrganizationId}`);
    },
    onDelete(organization) {
      // Reset laij thông tin các object xóa:
      this.deleteInfos = JSON.parse(this.deleteInfos_Original);
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [<b>${organization.OrganizationName}</b>] không? <br>
        Sau khi xóa toàn bộ dữ liệu sẽ bị mất và không thể khôi phục.`,
        () => {
          this.showDeleteProgress = true;
          this.maxios
            .delete(`organizations/${organization.OrganizationId}`)
            .then(() => {
              setTimeout(() => {
                this.showDeleteProgress = false;
              }, 2000);
            })
            .catch(() => {
              this.showDeleteProgress = false;
            });
          this.loadData();
        }
      );
    },
    backupDatabase() {
      this.commonJs.showConfirm(
        `Thực hiện sao lưu dữ liệu? <br>
        Quá trình có thể mất vài phút.`,
        () => {
          this.maxios.post(`organizations/backup`);
        }
      );
    },
    sentStatistic() {
      this.commonJs.showConfirm(
        `Việc này sẽ thực hiện gửi báo cáo doanh thu tự động trong <b>ngày hôm nay (${commonJs.formatDate(
          new Date()
        )})</b> cho toàn bộ các đơn vị. <br>
        Quá trình có thể mất vài phút. <br>
        Nhấn <b>[Đồng ý]</b> để tiến hành!`,
        () => {
          const date = new Date();
          const dateString = date.getDate();
          const monthString = date.getMonth() + 1;
          const year = date.getFullYear();
          this.maxios.get(
            `organizations/statistics?date=${year}-${monthString}-${dateString}&sentEmail=true`
          );
        }
      );
    },
  },
  beforeUnmount() {
    this.hubConnection.off("RecieveRemoveOrganizationInfo");
    window.removeEventListener("resize", this.setWindownWith);
  },
  data() {
    return {
      innerWidth: 0,
      organizations: [],
      totalRecords: 0,
      isReload: false,
      keySearch: "",
      limit: 0,
      offset: 0,
      idSelected: null,
      showDeleteProgress: false,
      deleteInfos_Original: "",
      deleteInfos: [
        {
          IsDeleted: false,
          ObjectName: "InvoiceDetails",
          ObjectText: "Chi tiết hóa đơn",
        },
        { IsDeleted: false, ObjectName: "Invoices", ObjectText: "Hóa đơn" },
        { IsDeleted: false, ObjectName: "Products", ObjectText: "Sản phẩm" },
        {
          IsDeleted: false,
          ObjectName: "GroupProducts",
          ObjectText: "Nhóm sản phẩm",
        },
        { IsDeleted: false, ObjectName: "Units", ObjectText: "Đơn vị tính" },
        {
          IsDeleted: false,
          ObjectName: "Customers",
          ObjectText: "Dữ liệu khách hàng",
        },
        {
          IsDeleted: false,
          ObjectName: "CustomerGroups",
          ObjectText: "Nhóm khách hàng",
        },
        {
          IsDeleted: false,
          ObjectName: "Positions",
          ObjectText: "Chức vụ, vị trí",
        },
        { IsDeleted: false, ObjectName: "UserRoles", ObjectText: "Quyền User" },
        { IsDeleted: false, ObjectName: "Users", ObjectText: "Người dùng" },
        {
          IsDeleted: false,
          ObjectName: "Employees",
          ObjectText: "Dữ liệu nhân viên",
        },
        {
          IsDeleted: false,
          ObjectName: "MSLicenses",
          ObjectText: "Thông tin License",
        },
        {
          IsDeleted: false,
          ObjectName: "Organizations",
          ObjectText: "Thông tin đơn vị",
        },
      ],
    };
  },
};
</script>
<style scoped>
.delete-info {
  position: fixed;
  bottom: 5px;
  right: 5px;
  z-index: 101;
  background-color: #fff;
  box-shadow: 0 0 5px #c1c1c1;
  border-radius: 4px;
}

.delete-item {
  background-color: #fff;
  padding: 4px 5px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  column-gap: 5px;
}

.delete-item + .delete-item {
  margin-top: 4px;
}

.delete__icon {
  font-size: 24px;
}

.delete__icon--checked {
  color: #006207;
}

.delete__icon--unchecked {
  animation: spin 3000ms infinite linear;
  -webkit-animation: spin 3000ms infinite linear;
}

.table-cell {
  display: flex;
  column-gap: 5px;
  align-items: center;
}

.org--confirm i {
  color: #006207;
  font-weight: 700;
  font-size: 16px;
}

.org--confirm + .org-name__text {
  color: #006207;
  font-weight: 700;
}

.org-description {
  font-size: 12px;
}

.org-description::before {
  content: "- ";
}
</style>
