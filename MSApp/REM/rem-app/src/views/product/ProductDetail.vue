<template>
  <m-dialog title="Thông tin sản phẩm" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Mã sản phẩm/ dịch vụ"
            ref="txtProductCode"
            v-model="product.ProductCode"
            :validated="isValidated"
            :required="true"
            :isDisabled="false"
            :isFocus="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <m-input
            label="Tên sản phẩm/ dịch vụ"
            ref="txtProductName"
            v-model="product.ProductName"
            :validated="isValidated"
            :required="true"
            :isDisabled="false"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Nhóm sản phẩm/ dịch vụ"
            v-model="product.GroupProductId"
            url="/groupproducts"
            propText="GroupProductName"
            propValue="GroupProductId"
          ></m-combobox>
        </div>
        <div class="m-col"></div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Đơn giá"
            ref="txtProductCode"
            v-model="product.UnitPrice"
            format="money"
            :validated="isValidated"
            :required="true"
            :isDisabled="false"
            :onlyNumberChar="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <m-combobox
            label="Đơn vị tính"
            v-model="product.UnitId"
            url="/units"
            propText="UnitName"
            propValue="UnitId"
          ></m-combobox>
        </div>
      </div>
      <div class="m-row">
        <m-text-area label="Mô tả" v-model="product.Description"></m-text-area>
      </div>
    </template>
    <template v-slot:footer>
      <div class="footer-button">
        <button class="btn btn--default mg-left-10" @click="onSave"><i class="icofont-save"></i> Lưu</button>
        <button class="btn btn--close" @click="onClose"><i class="icofont-ui-close"></i> Hủy</button>
      </div>
      </template>
  </m-dialog>
</template>
<script>
import router from "../../router";
import Enum from "../../scripts/enum";
export default {
  name: "ProductDetail",
  emits: [],
  props: ["id"],
  computed: {
    formMode: function () {
      if (this.id) {
        return Enum.FormMode.UPDATE;
      } else {
        return Enum.FormMode.ADD;
      }
    },
  },
  created() {
    if (this.id) {
      this.maxios.get(`products/${this.id}`).then((res) => {
        this.product = res;
      });
    } else {
        this.maxios.get(`products/new-code`)
        .then(res=>{
            this.product.ProductCode = res;
        })
    }
  },
  methods: {
    onClose() {
      router.push("/products");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`products`, this.product).then(() => {
          router.push("/products");
        });
      } else {
        this.maxios.put(`products/${this.id}`, this.product).then(() => {
          router.push("/products");
        });
      }
    },
  },
  data() {
    return {
      product: {},
    };
  },
};
</script>
<style scoped>
.footer-button{
  display: flex;
  align-items: center;
  flex-direction: row-reverse;
  justify-content: flex-start;
}
</style>