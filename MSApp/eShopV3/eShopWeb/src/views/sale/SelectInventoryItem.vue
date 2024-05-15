<template>
  <m-dialog title="Chọn sản phẩm" @onClose="$emit('onClose')">
    <template v-slot:content>
      <div class="inventory-items">
        <m-table-layout>
          <!-- TOOLBAR -->
          <template v-slot:toolbar>
            <div class="toolbar__left">
              <m-input-search
                width="300px"
                v-model="keySearch"
                placeholder="Tìm kiếm"
              ></m-input-search>
              <m-combobox
                url="/InventoryItemCategories"
                propValue="InventoryItemCategoryId"
                propText="InventoryItemCategoryName"
                class="mg-left-8"
                v-model="inventoryItemCategoryId"
              >
              </m-combobox>
            </div>
            <div class="toolbar__right"></div>
          </template>
          <!-- BẢNG DỮ LIỆU -->
          <template v-slot:container>
            <m-table
              ref="tbInventoryItems"
              :data="inventoryItemsFilter"
              :fit="true"
              size="small"
              empty-text="Không có dữ liệu"
              height="100%"
              @selection-change="onSelectInventoryItemChange"
            >
              <el-table-column type="selection" width="55" fixed="left" />
              <m-column
                prop="InventoryItemCode"
                label="Mã SP"
                width="80px"
              ></m-column>
              <m-column prop="InventoryItemName" label="SP/dịch vụ">
                <template #default="scope">
                  <div class="flex">
                    <!-- <span>{{ scope.row.InventoryItemName }}</span> -->
                    <el-tooltip
                      class="box-item"
                      effect="customized"
                      :content="scope.row.InventoryItemName"
                      placement="top"
                    >
                      {{ scope.row.InventoryItemName }}
                    </el-tooltip>
                  </div>
                </template>
              </m-column>
              <m-column
                prop="Quantity"
                label="Đang có"
                fixed="right"
                align="right"
              >
                <template #default="scope"> {{ scope.row.Quantity }} </template>
              </m-column>
              <m-column
                label="Đơn giá"
                align="right"
                fixed="right"
                width="100px"
              >
                <template #default="scope">
                  <div class="unit-price">
                    <!-- <span>1 x</span> -->
                    <el-tag>
                      {{ commonJs.formatMoney(scope.row.UnitPrice) }}
                    </el-tag>
                  </div>
                </template>
              </m-column>
            </m-table>
          </template>
        </m-table-layout>
      </div>
    </template>
    <template v-slot:footer>
      <button class="btn btn--close" @click="$emit('onClose')">Hủy</button>
      <button
        class="btn btn--default mg-left-10"
        @click="onSaveSelectedInventoryItem"
      >
        Chọn
      </button>
    </template>
  </m-dialog>
</template>
<script>
import { mapGetters } from "vuex";
export default {
  name: "SelectInventoryItem",
  emits: ["getSelectionInventoryItems", "onClose"],
  props: [],
  computed: {
    ...mapGetters([
            "inventories"
        ]),
  },
  created() {
    this.maxios.get(`inventories/list-order`).then((res) => {
      this.inventoryItems = res;
      this.inventoryItemsFilter = this.inventoryItems;
    });
  },
  methods: {
    onSaveSelectedInventoryItem() {
      var productSelecteds = this.$refs.tbInventoryItems.getSelectionRows();
      this.$emit("getSelectionInventoryItems", productSelecteds);
      this.$emit("onClose");
    },
    onSelectInventoryItemChange(product) {
      console.log(`product`, product);
    },
  },

  watch: {
    keySearch: function (newValue) {
      this.inventoryItemsFilter = this.inventoryItems.filter((item) => {
        let productNameNotAlias = this.commonJs
          .change_alias(item.InventoryItemName)
          .toLowerCase();
        let keySearchNotAlias = this.commonJs
          .change_alias(newValue)
          .toLowerCase();
        return (
          (productNameNotAlias.includes(keySearchNotAlias) &&
            (!this.inventoryItemCategoryId ||
              item.GroupInventoryItemId == this.inventoryItemCategoryId)) ||
          item.Barcode.includes(keySearchNotAlias) ||
          item.InventoryItemCode.includes(keySearchNotAlias)
        );
      });
    },
    inventoryItemCategoryId: function (newValue) {
      this.inventoryItemsFilter = this.inventoryItems.filter((item) => {
        var productNameNotAlias = this.commonJs
          .change_alias(item.InventoryItemName)
          .toLowerCase();
        var keySearchNotAlias = this.commonJs
          .change_alias(this.keySearch)
          .toLowerCase();
        return (
          productNameNotAlias.includes(keySearchNotAlias) &&
          (!newValue || item.GroupInventoryItemId == newValue)
        );
      });
    },
  },
  data() {
    return {
      inventoryItems: [],
      inventoryItemsFilter: [],
      keySearch: "",
      inventoryItemCategoryId: null,
    };
  },
};
</script>
<style scoped>
.inventory-items {
  height: 400px;
}
</style>
