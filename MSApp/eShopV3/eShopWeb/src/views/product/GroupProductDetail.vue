<template>
  <m-dialog title="Thông tin đơn vị tính" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Tên nhóm Sản phẩm, Dịch vụ"
            ref="txtGroupProductName"
            v-model="groupProduct.GroupProductName"
            :required="true"
            :isDisabled="false"
            :isFocus="true"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Thuộc nhóm"
            v-model="groupProduct.ParentId"
            url="/groupproducts"
            propText="GroupProductName"
            propValue="GroupProductId"
          ></m-combobox>
        </div>
      </div>
      <div class="m-row">
        <m-text-area label="Mô tả" v-model="groupProduct.Description"></m-text-area>
      </div>
      <div class="m-row">
          <div class="m-col">
            <m-checkbox id="chkIsSystem" :disabled="!isAdministrator" v-model="groupProduct.IsSystem" label="Đơn vị hệ thống"></m-checkbox>
          </div>
        </div>
    </template>
    <template v-slot:footer>
      <div class="footer-button">
        <button class="btn btn--default mg-left-10" @click="onSave">Lưu</button>
        <button class="btn btn--close" @click="onClose">Hủy</button>
      </div>
    </template>
  </m-dialog>
</template>
<script>
import Enum from "@/scripts/enum";
export default {
  name: "GroupProductDetail",
  emits: ["onClose"],
  props: ["id"],
  computed: {
    formMode: function () {
      if (this.id) {
        return Enum.FormMode.UPDATE;
      } else {
        return Enum.FormMode.ADD;
      }
    },
      isAdministrator: function(){
        var role = localStorage.getItem("userRoleValue");
        if (role == Enum.Role.Administrator) {
          return true;
        }else{
          return false;
        }
      }
  },
  created() {
    if (this.id) {
      this.maxios.get(`groupproducts/${this.id}`).then((res) => {
        this.groupProduct = res;
      });
    }
  },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`groupProducts`, this.groupProduct).then(() => {
          this.$emit("onClose");
        });
      } else {
        this.maxios.put(`groupProducts/${this.id}`, this.groupProduct).then(() => {
          this.$emit("onClose");
        });
      }
    },
  },
  data() {
    return {
      groupProduct: {},
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
