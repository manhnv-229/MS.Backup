<template>
  <MDialogDragable
    class="menu-selection"
    :overlay="true"
    :showHeader="false"
    :showFooter="false"
    :dragable="false"
    @onClose="onClose"
  >
    <template #content>
      <div class="menu__container">
        <div class="menu__list">
          <div class="menu__toolbar">
            <m-input-search
              placeholder="Tìm kiếm"
              v-model="keyFilter"
            ></m-input-search>
            <button
              class="toolbar__button --food icon--food"
              @click="onChangeItemType(Enum.OrderItemListType.DEFAULT)"
            >
              Thực đơn
            </button>
            <button
              class="toolbar__button --service icon--heart"
              @click="onChangeItemType(Enum.OrderItemListType.SERVICE)"
            >
              Dịch vụ
            </button>
            <button
              class="toolbar__button --other icon--girl"
              @click="onChangeItemType(Enum.OrderItemListType.OTHER)"
            >
              Khác
            </button>
          </div>
          <div class="menu__list-detail">
            <div
              class="menu-item-wrapper"
              v-show="orderItemListType === Enum.OrderItemListType.DEFAULT"
            >
              <div
                class="menu-item"
                v-for="(item, index) in itemsFilter"
                :key="item.InventoryItemId"
                @click="onSelectItem(item)"
              >
                <div class="menu-item__img">
                  <img :src="item.ImgFullPath" alt="" />
                </div>
                <div class="menu-item__name">{{ item.InventoryItemName }}</div>
                <div class="menu-item__price">
                  {{ commonJs.formatMoney(item.UnitPrice) }}
                </div>
              </div>
            </div>
            <div
              class="menu-item-wrapper"
              v-show="orderItemListType === Enum.OrderItemListType.SERVICE"
            >
              <div
                class="menu-item"
                v-for="(item, index) in servicesFilter"
                :key="item.ServiceId"
                @click="onSelectServiceItem(item)"
              >
                <div class="menu-item__img">
                  <img :src="item.ImgFullPath" alt="" />
                </div>
                <div class="menu-item__name">{{ item.ServiceName }}</div>
                <div class="menu-item__price">
                  {{ commonJs.formatMoney(item.UnitPrice)
                  }}<span v-if="item.BilledByHours"> /giờ</span>
                </div>
              </div>
            </div>
          </div>
          <div class="data-loading" v-if="loading">
            <div class="loading-wrapper">
              <div class="loading__icon">
                <i class="icofont-spinner-alt-1"></i>
              </div>
              <div class="loading__text">Đang xử lý, vui lòng đợi...</div>
            </div>
          </div>
        </div>
        <div class="menu__selected">
          <div class="invoice-title">CHI TIẾT HOÁ ĐƠN</div>
          <!-- Tính giờ slot -->
          <div class="sub-label">Tiền sử dụng Bàn/Phòng theo giờ:</div>
          <div class="item-group --by-hours">
            <SlotInvoiceItem
              v-for="(item, index) in slotNotDelete"
              :key="item.SlotId"
              :inputSlotInvoice="item"
            ></SlotInvoiceItem>
          </div>
          <!-- Tiền dịch vụ -->
          <div class="sub-label">Tiền sử dụng dịch vụ:</div>
          <div class="item-group --by-service">
            <ServiceItemSelected
              v-for="(item, index) in serviceNotDelete"
              :key="item.ServiceId"
              :itemInput="item"
              @onRemoveServiceItem="onRemoveServiceItem"
            ></ServiceItemSelected>
          </div>
          <!-- Chi tiết món -->
          <div class="sub-label">Dịch vụ ăn uống:</div>
          <div class="item-group --item-list">
            <ItemSelected
              v-for="(item, index) in inventoryNotDelete"
              :key="item.RefDetailId"
              :itemInput="item"
              @onRemoveItem="onRemoveInventory"
            >
            </ItemSelected>
          </div>
          <div class="invoice--total">
            <label> Tổng tiền: </label>
            <span class="total-amount">{{
              commonJs.formatMoney(totalMoneyComputed)
            }}</span>
          </div>
          <div class="menu__footer">
            <button class="menu__button--close" @click="onSaveAndClose">
              Lưu & Đóng
            </button>
            <button class="menu__button--save" @click="onPayOrder">
              Thanh toán
            </button>
          </div>
        </div>
      </div>
    </template>
  </MDialogDragable>
  <MenuSelectionItemQuantity
    v-if="showSelectionQuantity"
    :itemInput="currentItemSelected"
    @onConfirmQuantity="onConfirmSelectQuantity"
    @onClose="clearItemQuantitySelection"
  >
  </MenuSelectionItemQuantity>
</template>
<script>
import commonJs from "@/scripts/common";
import { mapGetters, mapState } from "vuex";
import MenuSelectionItemQuantity from "./MenuSelectionItemQuantity.vue";
import ItemSelected from "./ItemSelected.vue";
import ServiceItemSelected from "./ServiceItemSelected.vue";
import debounce from "@/scripts/debounce";
import Enum from "@/scripts/enum";
import SlotInvoiceItem from "./SlotInvoiceItem.vue";
export default {
  name: "MenuSelection",
  components: {
    MenuSelectionItemQuantity,
    ItemSelected,
    ServiceItemSelected,
    SlotInvoiceItem,
  },
  emits: ["onSaveAndClose","onClose","onPayOrder"],
  props: ["invoiceSlotInput"],
  created() {
    if (this.invoiceSlotInput) {
      // Lấy thông tin chi tiết hoá đơn:
      const refId = this.invoiceSlotInput.RefId;
      this.maxios.get(`/orders/${refId}`).then((data) => {
        this.invoice = data;
        if (!this.invoice.RefDetail) {
          this.invoice.RefDetail = [];
        }
        if (!this.invoice.SlotInvoices) {
          this.invoice.SlotInvoices = [];
        }
        if (!this.invoice.ServiceInvoices) {
          this.invoice.ServiceInvoices = [];
        }
      });
    }
    this.loading = true;
    this.itemsFilter = this.inventories;
    this.servicesFilter = this.services;
    this.$nextTick(() => {
      this.loading = false;
    });
  },
  computed: {
    ...mapGetters([
      "slotGroups",
      "branchs",
      "slots",
      "inventories",
      "services",
    ]),
    inventoryNotDelete: function () {
      const list = this.invoice?.RefDetail?.filter(
        (item) => item.EntityState !== this.Enum.EntityState.DELETE
      );
      return list || [];
    },
    serviceNotDelete: function () {
      const list = this.invoice?.ServiceInvoices?.filter(
        (item) => item.EntityState !== this.Enum.EntityState.DELETE
      );
      return list || [];
    },
    slotNotDelete: function () {
      const list = this.invoice?.SlotInvoices?.filter(
        (item) => item.EntityState !== this.Enum.EntityState.DELETE
      );
      return list || [];
    },
    totalMoneyComputed: function () {
      // Tổng tiền sử dụng Slot:
      let totalSlot = 0;
      for (const slot of this.slotNotDelete) {
        totalSlot = Number(totalSlot) + Number(slot.TotalAmount);
      }
      // Tổng tiền dịch vụ:
      let totalService = 0;
      for (const service of this.serviceNotDelete) {
        totalService = Number(totalService) + Number(service.TotalAmount);
      }
      // Tổng tiền gọi đồ ăn, đồ uống:
      let totalInventory = 0;
      for (const inventory of this.inventoryNotDelete) {
        totalInventory = Number(totalInventory) + Number(inventory.TotalAmount);
      }
      return (totalSlot || 0) + (totalService || 0) + (totalInventory || 0);
    },
  },
  watch: {
    keyFilter: debounce(function (newValue) {
      this.loading = true;
      var key = this.commonJs.change_alias(newValue || "");
      switch (this.orderItemListType) {
        case Enum.OrderItemListType.SERVICE:
          let services = this.services.filter((item) =>
            this.commonJs.change_alias(item.ServiceName).includes(key || "")
          );
          this.servicesFilter = [...services];
          break;
        case Enum.OrderItemListType.OTHER:
        default:
          let items = this.inventories.filter((item) =>
            this.commonJs
              .change_alias(item.InventoryItemName)
              .includes(key || "")
          );
          this.itemsFilter = [...items];
          break;
      }
      this.$nextTick(() => {
        this.loading = false;
      });
    }, 500),
    inventories: {
      handler: function (newValue) {
        this.loading = true;
        this.itemsFilter = newValue;
        this.$nextTick(() => {
          this.loading = false;
        });
      },
      deep: true,
    },
    services: {
      handler: function (newValue) {
        this.loading = true;
        this.servicesFilter = newValue;
        this.$nextTick(() => {
          this.loading = false;
        });
      },
      deep: true,
    },
    // orderItemListType: function (newValue) {
    //   this.keyFilter = null;
    //   switch (newValue) {
    //     case Enum.OrderItemListType.SERVICE:
    //       this.itemsFilter = [...this.services];
    //       break;
    //     case Enum.OrderItemListType.OTHER:
    //     default:
    //       this.itemsFilter = [...this.inventories]
    //       break;
    //   }
    // }
  },
  methods: {
    onRemoveServiceItem(service) {
      const intemDelete = this.invoice.ServiceInvoices.find(
        (item) => item.ServiceId === service.ServiceId
      );
      if (intemDelete && intemDelete.EntityState == this.Enum.EntityState.ADD) {
        const index = this.invoice.ServiceInvoices.indexOf(service);
        this.invoice.RefDetail.splice(index, 1);
      } else {
        intemDelete.EntityState = this.Enum.EntityState.DELETE;
      }
    },
    onRemoveInventory(inventory) {
      console.log(1111);
      const intemDelete = this.invoice.RefDetail.find(
        (item) => item.InventoryItemId === inventory.InventoryItemId
      );
      if (intemDelete && intemDelete.EntityState == this.Enum.EntityState.ADD) {
        const index = this.invoice.RefDetail.indexOf(inventory);
        this.invoice.RefDetail.splice(index, 1);
      } else {
        intemDelete.EntityState = this.Enum.EntityState.DELETE;
      }
    },
    onPayOrder() {
      this.invoice.InvoiceSlotActionType = Enum.InvoiceSlotActionType.COMPLETE;
      this.commonJs.showConfirm(
        "Bạn chắc chắn muốn thực hiện thanh toán hoá đơn này",
        () => {
          this.maxios
            .put(`/orders/${this.invoice.RefId}`, this.invoice)
            .then((res) => {
              this.$emit("onPayOrder",this.invoice);
            });
        }
      );
    },
    onSaveAndClose() {
      this.invoice.InvoiceSlotActionType = Enum.InvoiceSlotActionType.UPDATE;
      this.maxios
        .put(`/orders/${this.invoice.RefId}`, this.invoice)
        .then((res) => {
          this.$emit("onSaveAndClose",this.invoice);
        });
    },
    onClose(){
      this.$emit("onClose");
    },
    onChangeItemType(type) {
      this.orderItemListType = type;
    },
    onClose() {
      this.$emit("onClose");
    },
    onSelectItem(item) {
      this.currentItemSelected = { ...item };
      this.showSelectionQuantity = true;
    },
    onSelectServiceItem(item) {
      this.currentItemSelected = { ...item };
      this.showSelectionQuantity = true;
    },
    onConfirmSelectQuantity(quantity) {
      this.currentItemSelected.Quantity = quantity;
      this.currentItemSelected.RefId = this.invoice.RefId;
      this.currentItemSelected.EntityState = Enum.EntityState.ADD;
      // this.currentItemSelected.TotalAmount = quantity * this.currentItemSelected.UnitPrice;
      // Xem item là dịch vụ, gọi món hay loại khác:
      if (this.orderItemListType === Enum.OrderItemListType.SERVICE) {
        this.currentItemSelected.PriceByHour = this.currentItemSelected.UnitPrice;
        // Kiểm tra xem item đã được chọn có trong danh sách chưa, có rồi thì tăng số lượng, chưa có thì thêm mới:
        const itemExits = this.invoice.ServiceInvoices.find(
          (i) => i.ServiceId === this.currentItemSelected.ServiceId
        );
        if (itemExits) {
          itemExits.Quantity = Number(itemExits.Quantity) + Number(quantity);
        } else {
          this.invoice.ServiceInvoices.push(this.currentItemSelected);
        }
      } else {
        // Kiểm tra xem item đã được chọn có trong danh sách chưa, có rồi thì tăng số lượng, chưa có thì thêm mới:
        const itemExits = this.invoice.RefDetail.find(
          (i) => i.InventoryItemId === this.currentItemSelected.InventoryItemId
        );
        if (itemExits) {
          itemExits.Quantity = Number(itemExits.Quantity) + Number(quantity);
        } else {
          this.invoice.RefDetail.push(this.currentItemSelected);
        }
      }

      // reset lại:
      this.clearItemQuantitySelection();
    },
    clearItemQuantitySelection() {
      // reset lại:
      this.currentItemSelected = null;
      this.showSelectionQuantity = false;
    },
  },
  data() {
    return {
      invoice: {
        RefDetail: [],
        SlotInvoices: [],
        ServiceInvoices: [],
      },
      currentItemSelected: {},
      showSelectionQuantity: false,
      keyFilter: "",
      itemsFilter: [],
      servicesFilter: [],
      orderItemListType: Enum.OrderItemListType.DEFAULT,
      loading: false,
    };
  },
};
</script>
<style>
@import url(./css/menu-selection.css);
</style>
