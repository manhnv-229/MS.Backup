<template>
  <m-dialog title="Thông tin dịch vụ" @onClose="onClose">
    <template v-slot:content>
      <div class="service-form">
        <div>
          <a class="service__img" @click="onClickSelectImg">
            <img
              v-if="imgFullPath"
              :src="imgFullPath"
              :alt="service.ServiceName"
            />
            <img
              v-else
              :src="require('@/assets/imgs/service/picture-default-80.png')"
              :alt="service.ServiceName"
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
        <div class="service__info">
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Tên dịch vụ"
                ref="txtInventoryName"
                v-model="service.ServiceName"
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
                label="Mã dịch vụ"
                ref="txtInventoryCode"
                v-model="service.ServiceCode"
                :required="true"
                :disabled="mode == Enum.FormMode.VIEW"
              >
              </m-input>
            </div>
            <div class="m-col">
              <m-combobox
                label="Nhóm dịch vụ"
                ref="cbxServiceGroup"
                v-model="service.ServiceGroupId"
                url="/service-groups"
                propText="ServiceGroupName"
                propValue="ServiceGroupId"
                :showAddButton="mode !== Enum.FormMode.VIEW"
                :isDisabled="mode == Enum.FormMode.VIEW"
                @onClickExtButton="onInventoryCategoryAdd"
              ></m-combobox>
            </div>
          </div>
          <!-- <div class="m-row">
            <div class="m-col">
              <m-checkbox
                label="Tính phí theo thời gian sử dụng"
                v-model="service.ChargeByTime"
              ></m-checkbox>
            </div>
          </div> -->
          <div class="m-row">
            <div class="m-col">
              <m-combobox
                label="Tính phí theo"
                ref="cbxChargeType"
                v-model="service.ChargeType"
                url="/dictionarys/charge-types"
                propText="Text"
                propValue="Value"
                :required="true"
                :isDisabled="mode == Enum.FormMode.VIEW"
                :firstSelected="true"
              ></m-combobox>
            </div>
            <div class="m-col"></div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <div
                class="combobox-ext"
                v-if="service.ChargeType === Enum.ChargeType.QUANTITY"
              >
                <m-combobox
                  label="Đơn vị tính"
                  ref="cbxUnit"
                  v-model="service.UnitId"
                  url="/units"
                  propText="UnitName"
                  propValue="UnitId"
                  :required="true"
                  :isDisabled="mode == Enum.FormMode.VIEW"
                  :showAddButton="mode !== Enum.FormMode.VIEW"
                  @onClickExtButton="onUnitAdd"
                ></m-combobox>
              </div>
              <m-input
                v-else
                label="Thời gian tính tiền 1 lượt"
                ref="txtInventoryCode"
                v-model="service.UnitTime"
                format="money"
                :required="true"
                :disabled="mode == Enum.FormMode.VIEW"
                :onlyNumberChar="true"
                align="right"
                :moneyCode="timeCode"
              >
              </m-input>
            </div>
            <div class="m-col"></div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Giá vốn/chi phí thuê"
                ref="txtInventoryCode"
                v-model="service.CostPrice"
                format="money"
                :required="true"
                :disabled="mode == Enum.FormMode.VIEW"
                :onlyNumberChar="true"
                :moneyCode="moneyCode"
                align="right"
              >
              </m-input>
            </div>
            <div class="m-col">
              <m-input
                label="Đơn giá bán dịch vụ"
                ref="txtInventoryCode"
                v-model="service.UnitPrice"
                format="money"
                @onBlur="onAfterChangeUnitPrice"
                :required="true"
                :disabled="mode == Enum.FormMode.VIEW"
                :onlyNumberChar="true"
                align="right"
                :moneyCode="moneyCode"
              >
              </m-input>
            </div>
          </div>

          <!-- <div class="m-row" v-if="!service.ChargeByTime">
            <div class="m-col">
              <div class="combobox-ext">
                <m-combobox
                  label="Đơn vị tính"
                  ref="cbxUnit"
                  v-model="service.UnitId"
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
              <m-combobox
                label="Kho"
                ref="cbxStock"
                v-model="service.StockId"
                url="/stocks"
                propText="StockName"
                propValue="StockId"
                :showAddButton="true"
                @onClickExtButton="onStockAdd"
              ></m-combobox>
            </div>
          </div> -->
          <div class="m-row">
            <m-text-area
              label="Mô tả"
              v-model="service.Description"
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
  <ServiceGroupDetail
    v-if="showInventoryCategoryFormAdd"
    @onClose="showInventoryCategoryFormAdd = false"
    @onSaveSuccess="onAddServiceGroupSuccess"
  ></ServiceGroupDetail>
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
import ServiceGroupDetail from "./ServiceGroupItemDetail.vue";
import StockDetail from "../stock/StockDetail.vue";
export default {
  name: "InventoryDetail",
  components: { UnitDetail, ServiceGroupDetail, StockDetail },
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
    timeCode: function(){
      switch (this.service.ChargeType) {
        case this.Enum.ChargeType.MINUTES:
          return `phút`;
        case this.Enum.ChargeType.HOURS:
          return `giờ`;
        case this.Enum.ChargeType.DAYS:
          return `ngày`;
        case this.Enum.ChargeType.MONTHS:
          return `tháng`;
        case this.Enum.ChargeType.YEARS:
          return `năm`;
        default:
          return ``;
      }
    },
    moneyCode: function () {
      switch (this.service.ChargeType) {
        case this.Enum.ChargeType.MINUTES:
          return `đ/${
            this.service.UnitTime > 1 ? this.service.UnitTime : ""
          } phút`;
        case this.Enum.ChargeType.HOURS:
          return `đ/${
            this.service.UnitTime > 1 ? this.service.UnitTime : ""
          } giờ`;
        case this.Enum.ChargeType.DAYS:
          return `đ/${
            this.service.UnitTime > 1 ? this.service.UnitTime : ""
          } ngày`;
        case this.Enum.ChargeType.MONTHS:
          return `đ/${
            this.service.UnitTime > 1 ? this.service.UnitTime : ""
          } tháng`;
        case this.Enum.ChargeType.YEARS:
          return `đ/${
            this.service.UnitTime > 1 ? this.service.UnitTime : ""
          } năm`;
        default:
          return `đ`;
      }
    },
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
        return this.service.ImgPath;
      } else {
        const serverFileUrl = VUE_APP_ServerFileUrl;
        let path = `${serverFileUrl}${this.service.ImgPath}`;
        return path;
      }
    },
  },
  created() {
    if (this.id && this.id !== "create") {
      this.maxios.get(`services/${this.id}`).then((res) => {
        this.service = res;
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
      if (!this.service.MarketPrice) {
        this.service.MarketPrice = value;
      }
    },
    onClose() {
      const currentPath = this.$router.currentRoute.value.path;
      if (currentPath.includes("/services/")) {
        router.push("/services");
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
      formData.append("service", JSON.stringify(me.service));

      // Lưu dữ liệu:
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`services`, formData).then(() => {
          router.push("/services");
          this.$emitter.emit("reloadData");
        });
      } else {
        this.maxios.put(`services/${this.id}`, formData).then(() => {
          router.push("/services");
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
        this.service.ImgPath = URL.createObjectURL(file);
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
    async onAddServiceGroupSuccess(ic) {
      await this.$refs.cbxServiceGroup.reloadDataAndSetSelect(
        ic.ServiceGroupName
      );
    },
    async onAddUnitSuccess(unit) {
      await this.$refs.cbxUnit.reloadDataAndSetSelect(unit.UnitName);
    },
    getNewCode() {
      if (
        this.service.ServiceName !== "" &&
        this.service.ServiceName !== null &&
        this.service.ServiceName !== undefined
      ) {
        var me = this;
        if (this.formMode == Enum.FormMode.ADD) {
          this.maxios
            .get(`/services/new-code?str=${this.service.ServiceName}`)
            .then((res) => {
              me.service.ServiceCode = res;
            });
        }
      }
    },
  },
  data() {
    return {
      service: { ChargeType: Enum.ChargeType.QUANTITY, ChargeByTime: false },
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

.service-form {
  display: flex;
  column-gap: 10px;
}

.service__img {
  width: 150px;
  min-height: 100px;
  border: 1px dotted #ccc;
  margin-top: 16px;
  cursor: pointer;
  display: block;
}

.service__img img {
  width: 100%;
  cursor: pointer;
}
</style>
