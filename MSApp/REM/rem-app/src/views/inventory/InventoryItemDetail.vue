<template>
  <m-dialog title="Thông tin hàng hóa" @onClose="onClose">
    <template v-slot:content>
      <div class="inventory-form">
        <div>
          <a class="inventory__img" @click="onClickSelectImg">
            <img
              v-if="imgFullPath"
              :src="imgFullPath"
              :alt="inventory.InventoryItemName"
            />
            <img
              v-else
              :src="require('@/assets/imgs/products/no-image.png')"
              :alt="inventory.InventoryItemName"
            />
          </a>
          <input
            ref="fileInventiryImg"
            @change="fileImgOnChange"
            type="file"
            name="avatar"
            hidden
          />
        </div>
        <div class="inventory__info">
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Tên hàng hoá"
                ref="txtInventoryName"
                v-model="inventory.InventoryItemName"
                :required="true"
                :disabled="mode == Enum.FormMode.VIEW"
                @onBlur="getNewCode"
                :isFocus="true"
              >
              </m-input>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Mã vạch"
                ref="txtInventoryCode"
                v-model="inventory.Barcode"
                :required="true"
                :disabled="mode == Enum.FormMode.VIEW"
                :isFocus="false"
              >
              </m-input>
            </div>
            <div class="m-col"></div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Mã hàng hoá"
                ref="txtInventoryCode"
                v-model="inventory.InventoryItemCode"
                :required="true"
                :disabled="mode == Enum.FormMode.VIEW"
              >
              </m-input>
            </div>
            <div class="m-col">
              <m-combobox
                label="Nhóm hàng hoá"
                ref="cbxInventoryItemCategory"
                v-model="inventory.InventoryItemCategoryId"
                url="/inventoryItemCategories"
                propText="InventoryItemCategoryName"
                propValue="InventoryItemCategoryId"
                :showAddButton="mode !== Enum.FormMode.VIEW"
                :isDisabled="mode == Enum.FormMode.VIEW"
                @onClickExtButton="onInventoryCategoryAdd"
              ></m-combobox>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Giá vốn"
                ref="txtInventoryCode"
                v-model="inventory.CostPrice"
                format="money"
                :required="true"
                :disabled="mode == Enum.FormMode.VIEW"
                :onlyNumberChar="true"
              >
              </m-input>
            </div>
            <div class="m-col">
              <m-input
                label="Đơn giá bán"
                ref="txtInventoryCode"
                v-model="inventory.UnitPrice"
                format="money"
                @onBlur="onAfterChangeUnitPrice"
                :required="true"
                :disabled="mode == Enum.FormMode.VIEW"
                :onlyNumberChar="true"
              >
              </m-input>
            </div>
          </div>
          <!-- Giá thị trường -->
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Giá bán thị trường"
                ref="txtInventoryCode"
                v-model="inventory.MarketPrice"
                format="money"
                :required="false"
                :disabled="mode == Enum.FormMode.VIEW"
                :onlyNumberChar="true"
              >
              </m-input>
            </div>
            <div class="m-col"></div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <div class="combobox-ext">
                <m-combobox
                  label="Đơn vị tính"
                  ref="cbxUnit"
                  v-model="inventory.UnitId"
                  url="/units"
                  propText="UnitName"
                  propValue="UnitId"
                  :required="true"
                  :isDisabled="mode == Enum.FormMode.VIEW"
                  :showAddButton="mode !== Enum.FormMode.VIEW"
                  @onClickExtButton="onUnitAdd"
                ></m-combobox>
              </div>
            </div>
            <div class="m-col">
              <!-- <m-combobox
                label="Kho"
                ref="cbxStock"
                v-model="inventory.StockId"
                url="/stocks"
                propText="StockName"
                propValue="StockId"
                :showAddButton="true"
                @onClickExtButton="onStockAdd"
              ></m-combobox> -->
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Số lượng tồn kho ban đầu"
                ref="txtInventoryCode"
                v-model="inventory.Quantity"
                :required="false"
                :disabled="mode == Enum.FormMode.VIEW"
                :onlyNumberChar="true"
                align="right"
              >
              </m-input>
            </div>
            <div class="m-col"></div>
          </div>
          <div class="m-row">
            <m-text-area
              label="Mô tả"
              v-model="inventory.Description"
            ></m-text-area>
          </div>
        </div>
      </div>
    </template>
    <template v-slot:footer>
      <div class="footer-button">
        <button
          class="btn btn--default mg-left-10"
          @click="onSave"
          v-if="mode != Enum.FormMode.VIEW"
        >
          <i class="icofont-save"></i> Lưu
        </button>
        <button class="btn btn--close" @click="onClose">
          <i class="icofont-ui-close"></i> Hủy
        </button>
      </div>
    </template>
  </m-dialog>
  <UnitDetail
    v-if="showUnitFormAdd"
    @onClose="showUnitFormAdd = false"
    @onSaveSuccess="onAddUnitSuccess"
  ></UnitDetail>
  <InventoryItemCategoryDetail
    v-if="showInventoryCategoryFormAdd"
    @onClose="showInventoryCategoryFormAdd = false"
    @onSaveSuccess="onAddInventoryCategorySuccess"
  ></InventoryItemCategoryDetail>
  <StockDetail
    v-if="showStockFormAdd"
    @onClose="showStockFormAdd = false"
    @onSaveSuccess="onAddStockSuccess"
  >
  </StockDetail>
</template>
<script>
import router from "../../router";
import Enum from "../../scripts/enum";
import UnitDetail from "../units/UnitDetail.vue";
import InventoryItemCategoryDetail from "./InventoryItemCategoryDetail.vue";
import StockDetail from "../stock/StockDetail.vue";
export default {
  name: "InventoryDetail",
  components: { UnitDetail, InventoryItemCategoryDetail, StockDetail },
  emits: ["onClose"],
  props: {
    id: {
      type: String,
      required: false,
    },
    mode: {
      type: Number,
      default: null,
      required: false,
    },
  },
  computed: {
    formMode: function () {
      if (this.mode === Enum.FormMode.VIEW) {
        return Enum.FormMode.VIEW;
      } else {
        if (this.id && this.id !== "create") {
          return Enum.FormMode.UPDATE;
        } else {
          return Enum.FormMode.ADD;
        }
      }
    },
    imgFullPath: function () {
      if (!this.id || this.imgChanged) {
        return this.inventory.ImgPath;
      } else {
        const serverFileUrl = VUE_APP_ServerFileUrl;
        let path = `${serverFileUrl}${this.inventory.ImgPath}`;
        return path;
      }
    },
  },
  created() {
    if (this.id && this.id !== "create") {
      this.maxios.get(`inventories/${this.id}`).then((res) => {
        this.inventory = res;
      });
    }
    // this.$watch(
    //   () => this.$route.query,
    //   (newQuery) => {
    //     console.log(newQuery);
    //   },
    // );
  },
  methods: {
    onAfterChangeUnitPrice(value) {
      if (!this.inventory.MarketPrice) {
        this.inventory.MarketPrice = value;
      }
    },
    onClose() {
      console.log(this.$router);
      const currentPath = this.$router.currentRoute.value.path;
      if (currentPath.includes("/inventories/")) {
        router.push({ name: "InventoryListRouter" });
      } else {
        this.$emit("onClose");
      }
    },
    onSave() {
      var me = this;
      // Thực hiện validate thông tin:
      // lấy avatar:
      var formData = new FormData();
      var file = this.$refs.fileInventiryImg.files[0];
      if (file) {
        formData.append("file", file);
      }
      formData.append("inventory", JSON.stringify(me.inventory));

      // Lưu dữ liệu:
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`inventories`, formData).then(() => {
          router.push({ name: "InventoryListRouter" });
          this.$emitter.emit("reloadData");
        });
      } else {
        this.maxios.put(`inventories/${this.id}`, formData).then(() => {
          router.push({ name: "InventoryListRouter" });
          this.$emitter.emit("reloadData");
        });
      }
    },
    onClickSelectImg() {
      this.$refs.fileInventiryImg.click();
    },
    fileImgOnChange(e) {
      const name = e.target.name,
        file = e.target.files[0];
      const hasName = ["avatar"].includes(name);
      if (hasName) {
        // this.avatar = file;
        this.inventory.ImgPath = URL.createObjectURL(file);
        this.imgChanged = true;
      }
      console.log(file);
    },
    onStockAdd() {
      this.showStockFormAdd = true;
    },
    onUnitAdd() {
      this.showUnitFormAdd = true;
    },
    onInventoryCategoryAdd() {
      this.showInventoryCategoryFormAdd = true;
    },
    async onAddStockSuccess(stock) {
      await this.$refs.cbxStock.reloadDataAndSetSelect(stock.StockName);
    },
    async onAddInventoryCategorySuccess(ic) {
      await this.$refs.cbxInventoryItemCategory.reloadDataAndSetSelect(
        ic.InventoryItemCategoryName
      );
    },
    async onAddUnitSuccess(unit) {
      await this.$refs.cbxUnit.reloadDataAndSetSelect(unit.UnitName);
    },
    getNewCode() {
      if (this.inventory.InventoryItemName) {
        var me = this;
        if (this.formMode == Enum.FormMode.ADD) {
          this.maxios
            .get(
              `/inventories/newCodeAuto?name=${this.inventory.InventoryItemName}`
            )
            .then((res) => {
              console.log(res);
              me.inventory.InventoryItemCode = res;
              me.inventory.Barcode = res;
            });
        }
      }
    },
  },
  data() {
    return {
      inventory: {},
      imgChanged: false,
      showUnitFormAdd: false,
      showStockFormAdd: false,
      showInventoryCategoryFormAdd: false,
    };
  },
};
</script>
<style scoped>
.footer-button {
  display: flex;
  align-items: center;
  flex-direction: row-reverse;
  justify-content: flex-start;
}

.inventory-form {
  display: flex;
  column-gap: 10px;
}

.inventory__img {
  width: 150px;
  min-height: 100px;
  border: 1px dotted #ccc;
  margin-top: 16px;
  cursor: pointer;
  display: block;
}

.inventory__img img {
  width: 100%;
  cursor: pointer;
}
</style>
