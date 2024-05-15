<template>
  <m-dialog title="Thông tin Kho" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Mã kho hàng"
            ref="txtStockName"
            v-model="stock.StockCode"
            :required="true"
            :isDisabled="false"
            :isFocus="true"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Tên kho hàng"
            ref="txtStockName"
            v-model="stock.StockName"
            :required="true"
            :isDisabled="false"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Thuộc chi nhánh"
            v-model="stock.BranchId"
            url="/branchs"
            propText="BranchName"
            propValue="BranchId"
          ></m-combobox>
        </div>
      </div>
      <div class="m-row">
        <m-text-area label="Mô tả" v-model="stock.Description"></m-text-area>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-checkbox
            id="chkIsDefault"
            :disabled="!isAdministrator"
            v-model="stock.IsDefault"
            label="Là kho nhập hàng mặc định"
          ></m-checkbox>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-checkbox
            id="chkInactive"
            :disabled="!isAdministrator"
            v-model="stock.Inactive"
            label="Ngừng hoạt động"
          ></m-checkbox>
        </div>
      </div>
    </template>
    <template v-slot:footer>
      <button class="btn btn--close" @click="onClose">Hủy</button>
      <button class="btn btn--default mg-left-10" @click="onSave">Lưu</button>
    </template>
  </m-dialog>
</template>
<script>
import Enum from "../../scripts/enum";
export default {
  name: "StockDetail",
  emits: ["onClose"],
  props: ["id"],
  computed: {
    formMode: function () {
      if (this.id) {
        return Enum.FormMode.UPDATE;
      } else {
        return Enum.FormMode.ADD;
      }
    },
    isAdministrator: function () {
      var role = localStorage.getItem("userRoleValue");
      if (role == Enum.Role.Administrator) {
        return true;
      } else {
        return false;
      }
    },
  },
  created() {
    if (this.id) {
      this.maxios.get(`stocks/${this.id}`).then((res) => {
        this.stock = res;
      });
    } else {
      this.maxios.get(`stocks/new-code`).then((res) => {
        this.stock.StockCode = res;
      });
    }
  },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`stocks`, this.stock).then(() => {
          this.$emit("onClose");
          this.$emit("onSaveSuccess", this.stock);
        });
      } else {
        this.maxios.patch(`stocks/${this.id}`, this.stock).then(() => {
          this.$emit("onClose");
        });
      }
    },
  },
  data() {
    return {
      stock: { IsDefault: false, Inactive: false },
    };
  },
};
</script>
<style scoped></style>
