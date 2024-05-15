<template>
  <div class="menu-item--selected" v-if="slotInvoice.BilledByHours">
    <div class="item__name">{{ slotInvoice.SlotName }}</div>
    <div class="item__time-info">
      <div class="item__time-start">
        <label>Giờ bắt đầu:</label>
        <div class="item__time-start--string">
          {{ commonJs.formatDateTimeDDMMYYHHMMSS(slotInvoice.TimeStart) }}
          <button class="btn-change-time icon icon--edit" title="Thay đổi thời gian" @click="showChangeStartTimeDialog = true"></button>
        </div>
      </div>
    </div>
    <div class="item__money-info">
      <div class="item__price">
        {{ commonJs.formatMoney(slotInvoice.PriceByHour || 0) }}
      </div>
      <span>x   {{ timeInfoString }}</span>
      <div class="item__total">
        {{ commonJs.formatMoney(slotInvoice.TotalAmount) }}
      </div>
    </div>
  </div>
  <MDialogDragable
    class=""
    id="dlgChangeStartTime"
    :overlay="true"
    :showHeader="true"
    :showFooter="true"
    :dragable="true"
    v-if="showChangeStartTimeDialog"
    @onClose="showChangeStartTimeDialog = false"
  >
    <template #header>
      <!-- <div>Thay đổi thời gian sử dụng {{ slotInvoice.SlotName }}</div> -->
    </template>
    <template #content>
      <div style="padding: 12px">
        <el-date-picker
          v-model="timeStartOriginal"
          type="datetime"
          placeholder="Chọn giờ bắt đầu tính tiền"
          format="YYYY/MM/DD HH:mm:ss"
        />
      </div>
    </template>
    <template #footer>
      <div style="display: flex; align-items: center; gap: 4px">
        <m-button-icon
          text="Huỷ bỏ"
          cls="btn--close"
          iconCls="icon--close-white"
          @click="showChangeStartTimeDialog = false"
        ></m-button-icon>
        <m-button-icon
          text="Xác nhận"
          cls="btn--default"
          iconCls="icon--confirm"
          @click="onChangeTimeStart"
        ></m-button-icon>
      </div>
    </template>
  </MDialogDragable>
</template>
<script>
import timeUtility from "../../scripts/time.js";
import { calculatorMoneyByHour } from "@/scripts/helper.js";
export default {
  name: "SlotInvoiceITem",
  props: ["inputSlotInvoice"],
  created() {
    this.slotInvoice = this.inputSlotInvoice;
    this.timeStartOriginal = this.slotInvoice.TimeStart;
  },
  watch: {
    "slotInvoice.TimeStart": function (newValue) {
      timeUtility.stopTimeCalculator(this.intervalId);
      if (this.slotInvoice.BilledByHours && newValue) {
        const time = timeUtility.setCalculatorTime(
          newValue,
          1000,
          (timeInfo, timeInfoString, intervalId) => {
            this.timeInfoString = timeInfoString;
            this.intervalId = intervalId;
            this.timeInfo = timeInfo;
          }
        );
      } else {
        return "";
      }
    },
    timeInfo: {
      handler: function (newValue) {
        // Tính tổng số giờ:
        let hours =
          (newValue.Days || 0) * 24 +
          (newValue.Hours || 0) +
          (newValue.Minutes || 0) / 60;
        let total = calculatorMoneyByHour(this.slotInvoice.PriceByHour, hours);
        this.slotInvoice.TotalAmount = total;
      },
      deep: true,
    },
  },
  methods: {
    onChangeTimeStart() {
      this.slotInvoice.TimeStart = this.timeStartOriginal;
      this.showChangeStartTimeDialog = false;
    },
  },
  data() {
    return {
      slotInvoice: {},
      timeInfoString: "",
      intervalId: null,
      timeInfo: {},
      showChangeStartTimeDialog: false,
      timeStartOriginal: new Date(),
    };
  },
};
</script>
<style scoped>
.item__money-info {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.item__time-info {
  width: 100%;
  display: flex;
  flex-direction: column;
  row-gap: 4px;
  align-items: flex-start;
  justify-content: space-between;
}
.item__time-start {
  display: flex;
  align-items: center;
  width: 100%;
  position: relative;
}

.item__time-start label {
  flex: 0 0 100px;
}

.el-input__wrapper {
  height: 20px !important;
}

.menu-item--selected .mdialog-wrapper {
  width: 200px !important;
}
.item__time-start--string{
    position: relative;
}
.btn-change-time {
  width: 12px;
  height: 12px;
  position: absolute;
  top: -5;
  right: -20px;
  background-size: contain;
  border: unset;
  background-color: unset;
  cursor: pointer;
}
</style>
