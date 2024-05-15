<template>
  <MDialogDragable
    class=""
    :overlay="true"
    :showHeader="true"
    :showFooter="true"
    :dragable="false"
    @onClose="onClose"
  >
    <template #header> {{ slot?.SlotName }}</template>
    <template #content>
      <div class="order-info-detail">
        <div class="m-row">
          <m-combobox
            label="Khách hàng"
            v-model="orderSlot.CustomerId"
            :dataInput="customers"
            propValue="CustomerId"
            propText="FullName"
            @onChangeText="onInputCustomerName"
            @onSelected="onSelectCustomer"
            :showAddButton="true"
            @onClickExtButton="onCustomerAdd"
            ref="cbxCustomer"
          ></m-combobox>
        </div>
        <div class="bill--by-hour">
          <m-checkbox
            label="Thu phí theo giờ"
            v-model="slot.BilledByHours"
            :disabled="false"
          ></m-checkbox>
          <div class="bill__price" v-if="slot.BilledByHours">
            <m-input
              label="Đơn giá"
              :selectAllText="true"
              v-model="slot.PriceByHour"
              :required="false"
              :isFocus="true"
              align="right"
              format="money"
              :onlyNumberChar="true"
              moneyCode="/giờ"
            >
            </m-input>
            <div class="bill__time-start">
              <label for="">Thời gian bắt đầu sử dụng</label>
              <el-date-picker
                v-model="slot.TimeStart"
                type="datetime"
                placeholder="Chọn giờ bắt đầu tính tiền"
                format="YYYY/MM/DD HH:mm:ss"
              />
            </div>
          </div>
        </div>
      </div>
    </template>
    <template #footer>
      <div class="slot__footer m-row flex">
        <m-button-icon cls="btn--close" iconCls="icon--cancel-order"
          >Hủy bỏ</m-button-icon
        >
        <m-button-icon
          cls="btn--save btn--default"
          iconCls="icon--confirm"
          @click="onCreateOrder"
          >Bắt đầu sử dụng</m-button-icon
        >
      </div>
    </template>
  </MDialogDragable>
  <CustomerForm
    v-if="showCustomerAddForm"
    @onClose="showCustomerAddForm = false"
    @onSaveSuccess="onAddNewCustomerSuccess"
  ></CustomerForm>
</template>
<script>
import { mapGetters } from "vuex";
import CustomerForm from "@/views/customers/CustomerDetail.vue";
export default {
  name: "SaleSlotOpenForm",
  props: ["invoiceSlotInput"],
  emits: ["onClose"],
  components: { CustomerForm },
  computed: {
    ...mapGetters(["routerStore", "paymentTypes", "customers", "employees"]),
  },
  created() {
    const slotId = this.invoiceSlotInput.SlotId;
    const currentSlot = this.invoiceSlotInput.Slots.find(
      (item) => item.SlotId === slotId
    );
    this.slotFactory = currentSlot;
    this.slot = {...currentSlot} || {};
    this.orderSlot = this.invoiceSlotInput || {};
    // Thực hiện kiểm tra thông tin xem slot có tính phí theo giờ không?
    if (currentSlot?.BilledByHours === true) {
      // Gán ngày giờ hiện tại là mặc định
      currentSlot.TimeStart = new Date();
      if (!this.orderSlot.SlotInvoice) {
        this.orderSlot.SlotInvoice = [];
      }
      this.orderSlot.SlotInvoice.push(currentSlot);
    }
  },
  methods: {
    onCreateOrder() {
      // Cập nhật các thay đổi cho slotFactory:
      this.commonJs.mapObjectPropertiesValue(this.slot, this.slotFactory);
      this.maxios.post(`/orders`, this.orderSlot)
      .then(()=>{
        this.$emit("onClose");
      });
    },
    onInputCustomerName(text) {
      this.orderSlot.CustomerName = text;
      this.slot.CustomerName = text;
    },
    onSelectCustomer(value, text, customer) {
      console.log(text);
      if (customer) {
        this.orderSlot.CustomerName = customer.FullName;
        this.slot.CustomerName = customer.FullName;
        this.orderSlot.CustomerAddress = customer.Mobile || customer.Address;
        this.slot.CustomerAddress = customer.Mobile || customer.Address;
      }
    },
    onCustomerAdd() {
      this.showCustomerAddForm = true;
    },
    async onAddNewCustomerSuccess(customer) {
      await this.$store.dispatch(GET_ALL_CUSTOMER);
      await this.$refs.cbxCustomer.reloadDataAndSetSelect(customer.FullName);
    },

    onClose() {
      this.$emit("onClose");
    },
  },
  data() {
    return {
      slotFactory:{},
      slot: {},
      orderSlot: { Slots: [], SlotInvoice: [] },
      showCustomerAddForm: false,
    };
  },
};
</script>
<style scoped>
.order-info-detail {
  display: flex;
  flex-direction: column;
  align-items: center;
  row-gap: 10px;
}
.slot__footer {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  column-gap: 8px;
}

.bill--by-hour,
.bill__price {
  width: 100%;
  display: flex;
  flex-direction: column;
  row-gap: 10px;
}
</style>
