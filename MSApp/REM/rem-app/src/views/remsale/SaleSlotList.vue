<template>
  <div class="slots">
    <div class="slot__group" v-show="!collapseGroup">
      <div
        class="group-item"
        :class="{ '--active': groupSelectedId == null }"
        @click="onChangeGroup(null)"
      >
        <div class="group__title --all">Tất cả vị trí</div>
      </div>
      <div
        class="group-item"
        v-for="group in groups"
        :key="group.SlotGroupId"
        :class="{ '--active': groupSelectedId == group.SlotGroupId }"
        @click="onChangeGroup(group)"
      >
        <div class="group__title">
          {{ group.SlotGroupName }}
        </div>
      </div>
    </div>
    <div class="slot__container">
      <div class="slot__toolbar">
        <button
          class="group__button"
          :class="{ '--collapse': collapseGroup }"
          @click="collapseGroup = !collapseGroup"
        ></button>
        <div class="toolbar__left">
          <button
            class="toolbar__button --free"
            :class="{ '--checked': hasFree }"
            @click="onChangeSlotStatusFilter(Enum.SlotStatus.FREE)"
          ></button>
          <button
            class="toolbar__button --busy"
            :class="{ '--checked': hasBusy }"
            @click="onChangeSlotStatusFilter(Enum.SlotStatus.BUSY)"
          ></button>
          <button
            class="toolbar__button --waiting"
            :class="{ '--checked': hasWaiting }"
            @click="onChangeSlotStatusFilter(Enum.SlotStatus.WAITING)"
          ></button>
          <button
            class="toolbar__button --merging"
            :class="{ '--checked': hasMering }"
            @click="onChangeSlotStatusFilter(Enum.SlotStatus.MERGING)"
          ></button>
          <button
            class="toolbar__button --locking"
            :class="{ '--checked': hasLocking }"
            @click="onChangeSlotStatusFilter(Enum.SlotStatus.LOCKING)"
          ></button>
        </div>
        <div class="toolbar__right"></div>
      </div>
      <div class="slot__list">
        <SaleSlotItem
          :slotInput="item"
          v-show="slotStatusFilter.includes(item.SlotStatus)"
          v-for="item in slotList"
          :key="item.SlotId"
          @onShowSelectItem="onShowSelectItem"
          @onOpenSlot="onOpenSlot"
          @onPayAndCloseSlot="onPayAndCloseSlot"
        ></SaleSlotItem>
      </div>
    </div>
  </div>
  <!-- <m-page title="Hóa đơn bán hàng/ dịch vụ" :rightSlot="true">
    <template v-slot:container> dsfdfdsadasds </template>
</m-page> -->
  <MenuSelection
    v-if="showMenuSelection"
    :invoiceSlotInput="orderOfSlotSelected"
    @onSaveAndClose="onSaveAndCloseMenuSelection"
    @onClose="onCloseMenuSelection"
    @onPayOrder="onPayOrder"
  >
  </MenuSelection>
  <SaleSlotOpenForm
    v-if="showSlotOpenForm"
    :invoiceSlotInput="orderOfSlotSelected"
    @onClose="showSlotOpenForm = false"
  ></SaleSlotOpenForm>
  <InvoicePrint
    @onEditOrder="onEditOrder"
    @onClose="onCloseOrderDetail"
    :id="orderOfSlotSelected?.RefId"
    v-if="showOrderDetail"
  ></InvoicePrint>
</template>
<script>
import Enum from "@/scripts/enum";
import { mapGetters, mapState } from "vuex";
import SaleSlotItem from "./SaleSlotItem.vue";
import { GET_ALL_SLOT, GET_ALL_SLOT_GROUP } from "@/store/actions/entity";
import store from "@/store";
import MenuSelection from "./MenuSelection.vue";
import SaleSlotOpenForm from "./SaleSlotOpenForm.vue";
import InvoicePrint from "./InvoicePrint.vue";
export default {
  name: "SaleSlotList",
  components: { SaleSlotItem, MenuSelection, SaleSlotOpenForm, InvoicePrint },
  emits: [],
  props: {
    branchId: {
      required: false,
      default: null,
    },
  },
  computed: {
    ...mapGetters(["slotGroups", "branchs", "slots"]),
    hasFree: function () {
      return this.slotStatusFilter.includes(Enum.SlotStatus.FREE);
    },
    hasBusy: function () {
      return this.slotStatusFilter.includes(Enum.SlotStatus.BUSY);
    },
    hasWaiting: function () {
      return this.slotStatusFilter.includes(Enum.SlotStatus.WAITING);
    },
    hasMering: function () {
      return this.slotStatusFilter.includes(Enum.SlotStatus.MERGING);
    },
    hasLocking: function () {
      return this.slotStatusFilter.includes(Enum.SlotStatus.LOCKING);
    },
  },
  watch: {
    slots: {
      handler: function (newSlots) {
        console.log("slots change:", newSlots);
        let slots = [];
        if (newSlots && this.branchId) {
          slots = this.slots.filter((slot) => slot.BranchId === this.branchId);
          this.slotList = [...slots];
        } else if (newSlots && !this.branchId) {
          this.slotList = [...this.slots];
        } else {
          this.slotList = slots;
        }
      },
      deep: true,
    },
    slotGroups: function (newGroups) {
      let groups = [];
      if (newGroups && this.branchId) {
        groups = this.slotGroups.filter(
          (slot) => slot.BranchId === this.branchId,
        );
        this.groups = [...groups];
      } else if (newGroups && !this.branchId) {
        this.groups = [...this.slotGroups];
      } else {
        this.groups = groups;
      }
    },
    branchId: async function () {
      this.groupSelectedId = null;
      await this.loadGroups();
      await this.loadSlots();
      console.log("load slots 1!");
    },
    groupSelectedId: function (newValue) {
      this.loadSlots();
      console.log("load slots 2!");
    },
  },
  created() {
    this.$emitter.on("refreshData", this.refreshData);
    // Load dữ liệu:
    if (!this.slotGroups?.length) {
      store.dispatch(GET_ALL_SLOT_GROUP);
    } else {
      this.groups = [...this.slotGroups];
    }
    if (!this.slots?.length) {
      store.dispatch(GET_ALL_SLOT);
    } else {
      this.slotList = [...this.slots];
    }
  },
  beforeUnmount() {
    this.$emitter.off("refreshData", this.refreshData);
  },
  methods: {
    onCloseMenuSelection() {
      this.showMenuSelection = false;
      this.orderOfSlotSelected = {};
    },
    onEditOrder(order) {
      this.showOrderDetail = false;
      this.orderOfSlotSelected = order;
      this.showMenuSelection = true;
    },
    onPayOrder(order) {
      this.showMenuSelection = false;
      this.orderOfSlotSelected = order;
      this.showOrderDetail = true;
    },
    onSaveAndCloseMenuSelection(order) {
      this.showMenuSelection = false;
      console.log(order);
      console.log(this.orderOfSlotSelected);
      // Cập nhật thông tin slot hiện tại:
      // this.orderOfSlotSelected.RefDetail = order.RefDetail;
      // this.orderOfSlotSelected.SlotInvoice = order.SlotInvoice;
      // this.orderOfSlotSelected.ServiceInvoice = order.ServiceInvoice;
    },
    onCloseOrderDetail() {
      this.orderOfSlotSelected = {};
      this.showOrderDetail = false;
    },
    onPayAndCloseSlot(invoice) {
      this.orderOfSlotSelected = invoice;
      this.showOrderDetail = true;
    },
    onOpenSlot(orderOfSlot) {
      this.showSlotOpenForm = true;
      this.orderOfSlotSelected = orderOfSlot;
    },
    onShowSelectItem(orderOfSlot) {
      this.showMenuSelection = true;
      this.orderOfSlotSelected = orderOfSlot;
    },
    refreshData() {
      this.loadGroups();
      this.loadSlots();
      this.$nextTick(() => {
        this.commonJs.showToast("Đã làm mới dữ liệu!", Enum.MsgType.Success);
      });
    },
    async loadGroups() {
      if (this.branchId) {
        this.groups = [
          ...this.slotGroups.filter((item) => item.BranchId === this.branchId),
        ];
      } else {
        this.groups = [...this.slotGroups];
      }
    },
    async loadSlots() {
      if (!this.branchId && !this.groupSelectedId) {
        this.slotList = [...this.slots];
      } else {
        this.slotList = [
          ...this.slots.filter(
            (item) =>
              (item.SlotGroupId === this.groupSelectedId ||
                this.groupSelectedId === null) &&
              (item.BranchId === this.branchId || this.branchId === null),
          ),
        ];
      }
      console.log(this.slotList);
    },
    onChangeGroup(group) {
      this.groupSelectedId = group?.SlotGroupId;
    },
    onChangeSlotStatusFilter(status) {
      var index = this.slotStatusFilter.indexOf(status);
      if (index >= 0) {
        this.slotStatusFilter.splice(index, 1);
      } else {
        this.slotStatusFilter.push(status);
      }
    },
  },
  data() {
    return {
      slotList: [],
      groups: [],
      groupSelectedId: null,
      collapseGroup: false,
      slotStatusFilter: [
        Enum.SlotStatus.FREE,
        Enum.SlotStatus.BUSY,
        Enum.SlotStatus.WAITING,
        Enum.SlotStatus.MERGING,
        Enum.SlotStatus.LOCKING,
      ],
      showMenuSelection: false,
      orderOfSlotSelected: {},
      showSlotOpenForm: false,
      showOrderDetail: false,
    };
  },
};
</script>
<style scoped>
@import url(./css/style.css);
</style>
