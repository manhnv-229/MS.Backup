<template>
  <m-dialog title="Thông tin Nhóm/Khu vực" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Thuộc chi nhánh"
            v-model="slotGroup.BranchId"
            url="/branchs"
            :required="true"
            :firstSelected="true"
            propText="BranchName"
            propValue="BranchId"
          ></m-combobox>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Tên Nhóm/Khu vực"
            ref="txtSlotGroupName"
            v-model="slotGroup.SlotGroupName"
            :required="true"
            :isDisabled="false"
          >
          </m-input>
        </div>
      </div>

      <div class="m-row">
        <div class="m-col">
          <m-checkbox
            id="chkBilledByHours"
            :disabled="!isAdministrator"
            v-model="slotGroup.BilledByHours"
            label="Có tính tiền theo giờ"
          ></m-checkbox>
        </div>
      </div>
      <div class="m-row" v-if="slotGroup.BilledByHours">
        <div class="m-col">
          <m-input
            label="Giá theo giờ"
            ref="txtSlotGroupName"
            v-model="slotGroup.PriceByHour"
            :isDisabled="false"
            format="money"
            moneyCode="đ"
            :onlyNumberChar="true"
            align="right"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <m-text-area
          label="Mô tả"
          v-model="slotGroup.Description"
        ></m-text-area>
      </div>
    </template>
    <template v-slot:footer>
      <button class="btn btn--close" @click="onClose">Hủy</button>
      <button class="btn btn--default mg-left-10" @click="onSave">Lưu</button>
    </template>
  </m-dialog>
</template>
<script>
import { GET_ALL_SLOT_GROUP } from "@/store/actions/entity";
import Enum from "../../scripts/enum";
export default {
  name: "SlotGroupDetail",
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
      if (role <= Enum.Role.SuperManagement) {
        return true;
      } else {
        return false;
      }
    },
  },
  created() {
    if (this.id) {
      this.maxios.get(`slot-groups/${this.id}`).then((res) => {
        this.slotGroup = res;
      });
    }
  },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`slot-groups`, this.slotGroup).then(() => {
          this.$emit("onClose");
          this.$emit("onSaveSuccess", this.slotGroup);
          this.$store.dispatch(GET_ALL_SLOT_GROUP);
        });
      } else {
        this.maxios.put(`slot-groups/${this.id}`, this.slotGroup).then(() => {
          this.$emit("onClose");
          this.$store.dispatch(GET_ALL_SLOT_GROUP);
        });
      }
    },
  },
  data() {
    return {
      slotGroup: { BilledByHours: false },
    };
  },
};
</script>
<style scoped></style>
