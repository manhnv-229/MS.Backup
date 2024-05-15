<template>
  <div
    class="slot__item"
    :class="{
      '--free': slot.SlotStatus === Enum.SlotStatus.FREE,
      '--busy gradient-border': slot.SlotStatus === Enum.SlotStatus.BUSY,
      '--waiting': slot.SlotStatus === Enum.SlotStatus.WAITING,
      '--locking': slot.SlotStatus === Enum.SlotStatus.LOCKING,
    }"
  >
    <div class="slot__header">
      <div class="--left">
        <div
          v-if="slot.BilledByHours === true"
          class="slot__biller--time icon icon-16 icon--time-red"
        ></div>
        <div class="slot__name">{{ slot.SlotName }}</div>
      </div>
      <div class="--right">
        <div
          v-if="
            slot.BilledByHours === true &&
            slot.SlotStatus === Enum.SlotStatus.BUSY
          "
          ref="slotTimeUsed"
          class="slot__time--used"
        >
          {{ timeInfoString }}
        </div>
        <div v-else class="slot__status-info">
          <span v-if="slot.SlotStatus === Enum.SlotStatus.BUSY"
            >Đang sử dụng</span
          >
          <span v-else-if="slot.SlotStatus === Enum.SlotStatus.WAITING"
            >Có khách đặt</span
          >
          <span v-else-if="slot.SlotStatus === Enum.SlotStatus.LOCKING"
            >Có khách đặt</span
          >
          <span v-else>(Trống)</span>
        </div>
      </div>
    </div>
    <div class="slot__container">
      <div
        v-if="slot.SlotStatus === Enum.SlotStatus.FREE"
        class="slot__action-free"
      >
        <button
          class="slot__button--open icon icon-20 icon--play"
          @click="onOpenSlot(slot)"
        >
          Sử dụng
        </button>
        <button
          class="slot__button--pre-order icon icon-20 icon--pre-order"
          disabled
        >
          Đặt chỗ
        </button>
      </div>

      <div v-else class="slot__info">
        <div>Khách hàng: {{ slot.OrdererName }}</div>
        <div class="slot__biller-by-hours">
          <div class="slot__time--start">
            Giờ vào: {{ commonJs.formatDateTimeDDMMYYHHMMSS(slot.TimeStart) }}
          </div>
        </div>
        <!-- <div class="bill__money">Tạm tính: <b>1.450.000 đ</b></div> -->
      </div>
    </div>
    <div v-if="slot.SlotStatus !== Enum.SlotStatus.FREE" class="slot__action">
      <button
        class="slot__button--pay icon icon-24 icon--purchase-order"
        title="THANH TOÁN"
        @click="onPayAndCloseSlot"
      ></button>
      <button
        class="slot__button--pay icon icon-24 icon--cancel-order"
        title="HUỶ ORDER"
        @click="onCancelOrder"
      ></button>
      <button
        v-if="slot.SlotStatus === Enum.SlotStatus.MERGING"
        class="slot__button--add icon icon-24 icon--separate"
        title="TÁCH BÀN"
        disabled
      ></button>
      <button
        v-else
        class="slot__button--add icon icon-24 icon--merge"
        title="GHÉP BÀN"
        disabled
      ></button>

      <button
        class="slot__button--add icon icon-24 icon--add-to-order"
        title="THÊM DỊCH VỤ"
        @click="onShowSelectItem"
      ></button>
    </div>
  </div>
  <!-- <MenuSelection></MenuSelection> -->
</template>
<script>
import Enum from "@/scripts/enum";
import timeUtility from "../../scripts/time.js";
import MenuSelection from "./MenuSelection.vue";
import commonJs from "@/scripts/common.js";
export default {
  name: "SaleSlotItem",
  props: ["slotInput"],
  components: { MenuSelection },
  emits: ["onShowSelectItem", "onOpenSlot", "onPayAndCloseSlot"],
  computed: {},
  watch: {},
  created() {
    this.slot = this.slotInput || {};
    this.orderSlot.SlotId = this.slotInput.SlotId;
    if (this.slotInput.RefId) {
      this.orderSlot.RefId = this.slotInput.RefId;
    }
    // const slotTimeUsed = this.$refs.slotTimeUsed;
    this.orderSlot.Slots.push(this.slot);
    if (this.slot.TimeStart) {
      const time = timeUtility.setCalculatorTime(
        this.slot.TimeStart,
        1000,
        (timeInfo, timeInfoString, intervalId) => {
          this.timeInfoString = timeInfoString;
          this.intervalId = intervalId;
        }
      );
    }
    // Gán mặc định các thông tin:
    if (!this.orderSlot.SlotInvoice) {
      this.orderSlot.SlotInvoice = [];
    }
    if (!this.orderSlot.RefDetail) {
      this.orderSlot.RefDetail = [];
    }
    if (!this.orderSlot.ServiceInvoice) {
      this.orderSlot.ServiceInvoice = [];
    }

    this.hubConnection.on(
      `ProcessAfterCreateSlotOrderSuccess_${this.slot.SlotId}`,
      this.processAfterCreateSlotOrderSuccess
    );

    this.hubConnection.on(
      `ProcessAfterUpdateOrderSuccess_${this.slot.SlotId}`,
      this.processAfterUpdateOrderSuccess
    );

    this.hubConnection.on(
      `ProcessAfterDeleteOrCancelOrderSuccess_${this.slot.SlotId}`,
      this.processAfterDeleteOrCancelOrderSuccess
    );

    this.hubConnection.on(
      `ProcessAfterPayCompleteOrderSuccess_${this.slot.SlotId}`,
      this.processAfterPayCompleteOrderSuccess
    );
  },
  methods: {
    onShowSelectItem() {
      this.$emit("onShowSelectItem", this.orderSlot);
    },
    onOpenSlot(slot) {
      // Kiểm tra xem bàn/phòng có tính phí theo giờ không, hiển thị thông tin chi tiết cho phép người dùng có thể chuyển tính theo giờ hoặc không.
      //---> việc này sẽ liên quan đến tính phí trong hóa đơn.
      // this.commonJs.showConfirm(
      //   `Bắt đầu sử dụng <b>[${slot.SlotName}]<b>?`,
      //   () => {
      //     this.maxios.post(`/orders`, this.orderSlot);
      //   }
      // );
      this.$emit("onOpenSlot", this.orderSlot);
    },
    processAfterCreateSlotOrderSuccess(newSlot, orderSlot, user) {
      const oldObj = { ...this.slot };
      console.log("old:", oldObj);
      this.commonJs.mapObjectPropertiesValue(newSlot, this.slot);
      this.commonJs.mapObjectPropertiesValue(orderSlot, this.orderSlot);
      console.log("new:", this.slot);
      if (this.slot.TimeStart) {
        const time = timeUtility.setCalculatorTime(
          this.slot.TimeStart,
          1000,
          (timeInfo, timeInfoString, intervalId) => {
            this.timeInfoString = timeInfoString;
            this.intervalId = intervalId;
          }
        );
      }
      // Cập nhật lại các thông tin của slot:
      for (const key in newSlot) {
        if (Object.hasOwnProperty.call(this.slot, key)) {
          this.slot[key] = newSlot[key];
        }
      }
      this.commonJs.showToast(
        `[${newSlot.SlotName}] vừa mới được sử dụng!`,
        this.Enum.MsgType.Info
      );
      const timeString = this.commonJs.formatDateTimeDDMMYYHHMMSS(new Date());
      let activityContent = `${timeString}: </br>[${newSlot?.SlotName}] được ${user.FullName} mở`;
      this.$emitter.emit("AddActivity", activityContent);
    },

    processAfterUpdateOrderSuccess(slotUpdated, orderSlot, user) {
      this.commonJs.mapObjectPropertiesValue(slotUpdated, this.slot);
      this.commonJs.mapObjectPropertiesValue(orderSlot, this.orderSlot);
      const timeString = this.commonJs.formatDateTimeDDMMYYHHMMSS(new Date());
      let activityContent = `${timeString}: </br>[${slotReset?.SlotName}]được ${user.FullName} cập nhật!`;
      this.$emitter.emit("AddActivity", activityContent);
    },

    processAfterDeleteOrCancelOrderSuccess(slotReset, orderSlot, user) {
      console.log(slotReset);
      timeUtility.stopTimeCalculator(this.intervalId);
      this.commonJs.mapObjectPropertiesValue(slotReset, this.slot);
      this.commonJs.mapObjectPropertiesValue(orderSlot, this.orderSlot);
      const timeString = this.commonJs.formatDateTimeDDMMYYHHMMSS(new Date());
      let activityContent = `${timeString}: </br>[${slotReset?.SlotName}]được ${user.FullName} đóng lại`;
      this.$emitter.emit("AddActivity", activityContent);
    },
    processAfterPayCompleteOrderSuccess(slotReset, orderSlot, user) {
      console.log(orderSlot);
      timeUtility.stopTimeCalculator(this.intervalId);
      this.commonJs.mapObjectPropertiesValue(slotReset, this.slot);
      this.commonJs.mapObjectPropertiesValue(orderSlot, this.orderSlot);
      const timeString = this.commonJs.formatDateTimeDDMMYYHHMMSS(new Date());
      let activityContent = `${timeString}: </br>[${slotReset?.SlotName}]được ${user.FullName} thanh toán`;
      this.$emitter.emit("AddActivity", activityContent);
    },
    onPayAndCloseSlot() {
      this.$emit("onPayAndCloseSlot", this.orderSlot);
    },
    onCancelOrder() {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn ngưng sử dụng <b>[${this.slot.SlotName}]</b> mà không thực hiện thanh toán không?`,
        () => {
          this.orderSlot.InvoiceSlotActionType =
            Enum.InvoiceSlotActionType.DELETE;
          this.maxios.put(`/orders/${this.orderSlot.RefId}`, this.orderSlot);
        }
      );
    },
  },
  data() {
    return {
      slot: {},
      intervalId: null,
      timeInfoString: "",
      orderSlot: { Slots: [] },
    };
  },
};
</script>
<style scoped>
.slot__item:has(.slot__action-free) .slot__header .--left {
  background-color: #c9c9c9;
  box-shadow: -2px 0px 0 0px #c9c9c9;
}
.slot__action-free button {
  width: 120px;
  height: 40px;
  border: unset;
  background-color: #fff;
  border-radius: 4px;
  font-weight: 700;
  cursor: pointer;
}
.slot__action-free button:hover {
  outline: 4px dotted #42a5f5;
}

.slot__container {
  padding: 12px;
  position: relative;
  box-sizing: border-box;
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.slot__action {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding: 12px;
  gap: 8px;
}

.slot__action button {
  border: unset;
  background-color: unset;
  width: 32px;
  height: 32px;
  background-repeat: no-repeat;
  background-position: center;
  background-size: 24px;
  cursor: pointer;
  border-radius: 4px;
  background-color: #fff;
  box-shadow: 0 0 10px #dedede;
  cursor: pointer;
}

.slot__action-free {
  display: flex;
  align-items: center;
  column-gap: 10px;
  justify-content: center;
  position: absolute;
  top: 0;
  left: 0;
  bottom: 0;
  right: 0;
  background-color: #0000006c;
}

.slot__action button:hover {
  outline: 4px dotted var(--color-primary);
}

.slot__button--pay {
}

.slot__button--add {
}

.slot__item.--busy .slot__header {
}

.slot__header {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  border-radius: 4px 4px 0 0;
  padding: 10px;
  box-sizing: border-box;
  font-size: 18px;
}

.slot__header .--left {
  height: 100%;
  position: relative;
  background-color: #fff;
  padding: 0 10px;
  display: flex;
  align-items: center;
  justify-content: flex-start;
  gap: 4px;
  flex: 1;
  box-sizing: border-box;
}

.slot__name {
  font-weight: 700;
}

.slot__header .--right {
  color: #fff;
  padding: 4px;
  flex: 1;
  height: 100%;
  box-sizing: border-box;
}

.slot__status-info {
  color: #fff;
  text-align: center;
}
.slot__time--used {
  font-weight: 700;
  text-align: center;
}

.slot__biller--time {
  width: 20px;
  height: 20px;
}

.slot__button--pre-order,
.slot__button--open {
  padding-left: 40px;
  background-position: 12px center;
  text-align: left;
}

.bill--by-hour {
  padding: 10px;
  border-radius: 4px;
  box-shadow: 0 0 5px #dedede;
  margin-top: 10px;
}

.bill__price {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  row-gap: 12px;
}

.slot__item.--busy .slot__header .--right {
  background-image: linear-gradient(#000000, #28a745);
}
</style>
