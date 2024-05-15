<template>
  <m-dialog title="Thông tin Phòng/Bàn" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Mã Phòng/Bàn"
            ref="txtSlotName"
            v-model="slot.SlotCode"
            :required="true"
            :isDisabled="false"
            :isFocus="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <m-input
            label="Tên Phòng/Bàn"
            ref="txtSlotName"
            v-model="slot.SlotName"
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
            v-model="slot.BranchId"
            :dataInput="branchs"
            propText="BranchName"
            propValue="BranchId"
            :required="true"
            :firstSelected="true"
            @onChangeValueSelected="onChangeBranch"
          ></m-combobox>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Thuộc khu vực/nhóm"
            v-model="slot.SlotGroupId"
            :dataInput="slotGroupsFilter"
            propText="SlotGroupName"
            propValue="SlotGroupId"
            :required="true"
            @onChangeValueSelected="onChangeGroup"
          ></m-combobox>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-checkbox
            id="chkBilledByHours"
            :disabled="!isAdministrator"
            v-model="slot.BilledByHours"
            label="Có tính tiền theo giờ"
          ></m-checkbox>
        </div>
      </div>
      <div class="m-row" v-if="slot.BilledByHours">
        <div class="m-col">
          <m-input
            label="Giá theo giờ"
            ref="txtSlotGroupName"
            v-model="slot.PriceByHour"
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
        <m-text-area label="Mô tả" v-model="slot.Description"></m-text-area>
      </div>
      <div class="m-row" v-if="formMode == Enum.FormMode.UPDATE">
        <div class="m-col">
          <m-checkbox
            id="chkInactive"
            :disabled="!isAdministrator"
            v-model="slot.Inactive"
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
import { mapGetters, mapState } from "vuex";
export default {
  name: "SlotDetail",
  emits: ["onClose"],
  props: ["id"],
  computed: {
    ...mapGetters(["slotGroups", "branchs"]),
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
    this.slotGroupsFilter = this.slotGroups;
    if (this.id) {
      this.maxios.get(`slots/${this.id}`).then((res) => {
        this.slot = res;
      });
    } else {
      this.maxios.get(`slots/new-code`).then((res) => {
        this.slot.SlotCode = res;
      });
    }
  },
  methods: {
    /**
     * Thay đổi chi nhánh thì load các nhóm tương ứng
     * @param {*} branchId
     * Author: NVMANH (27/03/2024)
     */
    onChangeBranch(branchId) {
      if (branchId) {
        this.slotGroupsFilter = this.slotGroups.filter(
          (item) => item.BranchId == branchId
        );
      } else {
        this.slotGroupsFilter = [...this.slotGroups];
      }
    },

    /**
     * Thay đổi nhóm thì lấy thông tin liên quan đến tình tiền theo giờ hoặc không
     * @param {*} newValue
     * Author: NVMANH (27/03/2024)
     */
    onChangeGroup(groupId) {
      // Lấy thông tin khu vực/ nhóm đã chọn:
      const group = this.slotGroupsFilter.find(
        (item) => item.SlotGroupId === groupId
      );
      if (group) {
        this.slot.BilledByHours = group.BilledByHours;
        this.slot.PriceByHour = group.PriceByHour;
      } else {
        this.slot.BilledByHours = false;
        this.slot.PriceByHour = null;
      }
    },
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`slots`, this.slot).then(() => {
          this.$emit("onClose");
        });
      } else {
        this.maxios.patch(`slots/${this.id}`, this.slot).then(() => {
          this.$emit("onClose");
        });
      }
    },
  },
  data() {
    return {
      slot: { BilledByHours: false },
      slotGroupsFilter: [],
    };
  },
};
</script>
<style scoped></style>
