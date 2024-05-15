<template>
  <div class="menu-item--selected">
    <div class="item__name">{{ item.InventoryItemName }}</div>
    <div class="order-info">
      <div class="item__quantity">
        <span>SL:</span>
        <m-input
          class="input--table"
          width="80"
          :selectAllText="true"
          v-model="item.Quantity"
          :required="true"
          :isFocus="true"
          align="right"
          :onlyNumberChar="true"
        >
        </m-input>
      </div>
      <div class="item__price">
        x {{ commonJs.formatMoney(item.UnitPrice || 0) }}
      </div>
      <div class="item__total">
        {{ commonJs.formatMoney(item.TotalAmount || 0) }}
      </div>
      <button class="btn-item--delete icon icon-16 icon--remove" @click="onRemoveItem"></button>
    </div>
  </div>
</template>
<script>
export default {
  name: "ItemSelected",
  props: ["itemInput"],
  emits: ["onRemoveItem"],
  created() {
    this.item = this.itemInput;
  },
  watch: {
    "item.Quantity": function (newValue) {
      if (this.item.EntityState != this.Enum.EntityState.ADD) {
        this.item.EntityState = this.Enum.EntityState.UPDATE;
      }
      this.item.TotalAmount = (this.item.UnitPrice || 0) * (newValue || 0);
    },
    "item.UnitPrice": function (newValue) {
      if (this.item.EntityState != this.Enum.EntityState.ADD) {
        this.item.EntityState = this.Enum.EntityState.UPDATE;
      }
      this.item.TotalAmount = (this.item.Quantity || 0) * (newValue || 0);
    },
  },
  methods: {
    onRemoveItem(){
        this.$emit("onRemoveItem", this.item);
    }
  },
  data() {
    return {
      item: {},
    };
  },
};
</script>
<style scoped>
.item__quantity {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  column-gap: 4px;
}
</style>
