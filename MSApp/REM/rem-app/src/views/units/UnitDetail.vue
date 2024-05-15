<template>
  <m-dialog title="Thông tin đơn vị tính" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Mã đơn vị"
            ref="txtUnitName"
            v-model="unit.UnitCode"
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
            label="Tên đơn vị"
            ref="txtUnitName"
            v-model="unit.UnitName"
            :required="true"
            :isDisabled="false"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <m-text-area label="Mô tả" v-model="unit.Description"></m-text-area>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-checkbox
            id="chkIsSystem"
            :disabled="!isAdministrator"
            v-model="unit.IsSystem"
            label="Đơn vị hệ thống"
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
  name: "UnitDetail",
  emits: ["onClose", "onSaveSuccess"],
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
      this.maxios.get(`units/${this.id}`).then((res) => {
        this.unit = res;
      });
    } else {
      this.maxios.get(`units/new-code`).then((res) => {
        this.unit.UnitCode = res;
      });
    }
  },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`units`, this.unit).then(() => {
          this.$emit("onClose");
          this.$emit("onSaveSuccess", this.unit);
        });
      } else {
        this.maxios.patch(`units/${this.id}`, this.unit).then(() => {
          this.$emit("onClose");
          this.$emit("onSaveSuccess", this.unit);
        });
      }
    },
  },
  data() {
    return {
      unit: {},
    };
  },
};
</script>
<style scoped></style>
