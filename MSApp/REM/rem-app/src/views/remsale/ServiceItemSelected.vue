<template>
  <div
    class="menu-item--selected"
  >
    <div class="item__name">({{ item.Quantity }}) {{ item.ServiceName }}</div>
    <div class="order-info">
      <div class="service-info">
        <!-- Thông tin giá và số lượng dịch vụ -->
        <div class="service-info__price">
          <div class="item__quantity"></div>
          <div class="item__price">
            {{ commonJs.formatMoney(item.PriceByHour || 0) }}
          </div>
          <span>x</span>
          <div class="item__total-time">
            <m-input
              class="input--table"
              width="80"
              :selectAllText="true"
              v-model="item.TotalTime"
              :required="false"
              :isFocus="true"
              align="right"
              format="money"
              :onlyNumberChar="true"
              moneyCode="giờ"
            >
            </m-input>
          </div>
        </div>
        <!-- Thông tin thời gian sử dụng -->
        <!-- <div class="service-info__time">
                    <MDateTimePicker></MDateTimePicker>
                  </div> -->
      </div>
      <div class="item__total">{{ commonJs.formatMoney(item.TotalAmount) }}</div>
      <button class="btn-item--delete icon icon-16 icon--remove" @click="onRemoveServiceItem"></button>
    </div>
  </div>
</template>
<script>
import Enum from '@/scripts/enum';

export default {
  name: "ServiceItemSelected",
  props: ["itemInput"],
  emits: ["onRemoveServiceItem"],
  created() {
    this.item = this.itemInput;
  },
  watch: {
    "item.TotalTime": function (newValue) {
      if (!this.item.EntityState != Enum.EntityState.ADD) {
        this.item.EntityState == Enum.EntityState.UPDATE;
      }
      this.item.TotalAmount = (this.item.PriceByHour || 0) * (newValue || 0);
    },
    "item.PriceByHour": function (newValue) {
      this.item.TotalAmount = (this.item.TotalTime || 0) * (newValue || 0);
    },
  },
  methods: {
    onRemoveServiceItem(){
      this.$emit("onRemoveServiceItem",this.item);
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
