<template>
    <m-dialog title="Nhóm hàng hoá" @onClose="onClose">
      <template v-slot:content>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Mã nhóm hàng hoá"
              ref="txtInventoryItemCategoryName"
              v-model="inventoryItemCategory.InventoryItemCategoryCode"
              :required="true"
              :isDisabled="false"
              :isFocus="true"
            >
            </m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Tên nhóm hàng hoá"
              ref="txtInventoryItemCategoryName"
              v-model="inventoryItemCategory.InventoryItemCategoryName"
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
              v-model="inventoryItemCategory.ParentId"
              url="/inventoryItemCategories"
              propText="InventoryItemCategoryName"
              propValue="InventoryItemCategoryId"
            ></m-combobox>
          </div>
        </div>
        <div class="m-row">
          <m-text-area label="Mô tả" v-model="inventoryItemCategory.Description"></m-text-area>
        </div>
        <div class="m-row">
            <div class="m-col">
              <m-checkbox id="chkIsSystem" :disabled="!isAdministrator" v-model="inventoryItemCategory.IsSystem" label="Đơn vị hệ thống"></m-checkbox>
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
    name: "InventoryItemCategoryDetail",
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
        this.maxios.get(`inventoryitemcategories/${this.id}`).then((res) => {
          this.inventoryItemCategory = res;
        });
      }
    },
    methods: {
      onClose() {
        this.$emit("onClose");
      },
      onSave() {
        if (this.formMode == Enum.FormMode.ADD) {
          this.maxios.post(`inventoryItemCategories`, this.inventoryItemCategory).then(() => {
            this.$emit("onClose");
            this.$emit("onSaveSuccess", this.inventoryItemCategory);
          });
        } else {
          this.maxios.put(`inventoryItemCategories/${this.id}`, this.inventoryItemCategory).then(() => {
            this.$emit("onClose");
          });
        }
      },
    },
    data() {
      return {
        inventoryItemCategory: {},
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
  