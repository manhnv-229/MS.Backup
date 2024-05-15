<template>
  <m-dialog title="Chọn sản phẩm" @onClose="$emit('onClose')">
    <template v-slot:content>
      <div class="products">
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
                url="/groupproducts"
                propValue="GroupProductId"
                propText="GroupProductName"
                class="mg-left-8"
                v-model="groupProductId"
              >
              </m-combobox>
            </div>
            <div class="toolbar__right"></div>
          </template>
          <!-- BẢNG DỮ LIỆU -->
          <template v-slot:container>
            <m-table
              ref="tbProducts"
              :data="productFilters"
              :fit="true"
              size="small"
              empty-text="Không có dữ liệu"
              width="100%"
              height="100%"
              @selection-change="onSelectProductChange"
            >
              <el-table-column type="selection" width="55" fixed="left" />
              <!-- <m-column prop="ProductCode" label="Mã SP" width="80px"></m-column> -->
              <m-column prop="ProductName" label="SP/dịch vụ" fixed="left" width="130px">
                <template #default="scope">
                  <div class="flex">
                    <span>{{ scope.row.ProductName }}</span>
                  </div>
                </template>
              </m-column>
              <m-column prop="Quantity" label="Đang có" fixed="right" align="right">
                <template #default="scope"> {{ scope.row.Quantity }} </template>
              </m-column>
              <m-column label="Đơn giá" align="right" fixed="right" width="100px">
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
      <button class="btn btn--default mg-left-10" @click="onSaveSelectedProduct">
        Chọn
      </button>
    </template>
  </m-dialog>
</template>
<script>
export default {
  name: "SelectProduct",
  emits: ["getSelectionProducts", "onClose"],
  props: [],
  created() {
    this.maxios.get(`products/list-order`).then((res) => {
      this.products = res;
      this.productFilters = this.products;
    });
  },
  methods: {
    onSaveSelectedProduct() {
      var productSelecteds = this.$refs.tbProducts.getSelectionRows();
      this.$emit("getSelectionProducts", productSelecteds);
      this.$emit("onClose");
    },
    onSelectProductChange(product) {
      console.log(`product`, product);
    },
  },

  watch: {
    keySearch: function (newValue) {
      this.productFilters = this.products.filter((item) => {
        var productNameNotAlias = this.commonJs.change_alias(item.ProductName).toLowerCase();
        var keySearchNotAlias = this.commonJs.change_alias(newValue).toLowerCase();
        return (
          productNameNotAlias.includes(keySearchNotAlias) &&
          (!this.groupProductId || item.GroupProductId == this.groupProductId)
        );
      });
    },
    groupProductId: function (newValue) {
      console.log(newValue);
      this.productFilters = this.products.filter((item) => {
        var productNameNotAlias = this.commonJs.change_alias(item.ProductName).toLowerCase();
        var keySearchNotAlias = this.commonJs.change_alias(this.keySearch).toLowerCase();
        return (
          productNameNotAlias.includes(keySearchNotAlias) &&
          (!newValue || item.GroupProductId == newValue)
        );
      });
    },
  },
  data() {
    return {
      products: [],
      productFilters: [],
      keySearch: "",
      groupProductId: null,
    };
  },
};
</script>
<style scoped>
.products {
  height: 400px;
}
</style>
