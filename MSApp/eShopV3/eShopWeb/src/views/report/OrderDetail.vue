<template>
  <m-dialog @onClose="onClose">
    <template v-slot:content>
      <div v-if="!showPrintPreview" id="invoice" class="invoice">
        <!-- HEADER -->
        <div class="invoice__header">
          <div class="header__left">
            <div class="org-info">
              <div class="org__name">
                <span>
                  {{ invoice.Organization?.OrganizationName }}
                </span>
              </div>
              <div class="org__address">
                <div class="info__icon"><i class="icofont-home"></i></div>
                <div class="info__text">
                  {{ invoice.Organization?.Address }}
                </div>
              </div>
              <div class="org__mobile">
                <div class="info__icon">
                  <i class="icofont-ui-cell-phone"></i>
                </div>
                <div class="info__text">
                  {{ invoice.Organization?.PhoneNumber }}
                </div>
              </div>
            </div>
          </div>
          <div class="header__right">
            <div class="org__description">
              {{ invoice.Organization?.Description }}
            </div>
          </div>
        </div>
        <!-- NAME -->
        <div class="invoice__name">
          <div class="invoice__title">HÓA ĐƠN BÁN HÀNG</div>
          <div class="invoice__refno">(Số: {{ invoice.RefNo }})</div>
          <div class="customer">
            <div class="customer__name">
              Tên khách hàng: {{ invoice.CustomerName }}
            </div>
            <div class="customer__address">
              Địa chỉ/SĐT: {{ invoice.CustomerAddress }}
            </div>
          </div>
        </div>
        <!-- DANH SÁCH SẢN PHẨM -->
        <div class="invoice__products">
          <m-table
            ref="tbProducts"
            :data="invoice.RefDetail"
            size="small"
            :class-name="'abc'"
            empty-text="Không có sản phẩm/ dịch vụ"
            width="100%"
            height="100%"
          >
            <m-column type="index" label="#" width="40px" />
            <m-column
              prop="InventoryItemName"
              label="SP/dịch vụ"
              class-name="m-cell--custom"
            >
              <template #default="scope">
                <div
                  class="flex"
                  :class="{
                    'is-delete':
                      scope.row.EntityState == Enum.EntityState.DELETE,
                  }"
                >
                  <div
                    :class="{
                      'item--selected':
                        itemReport.InventoryItemId ==
                        scope.row?.InventoryItemId,
                    }"
                  >
                    {{ scope.row?.InventoryItemName }}
                  </div>
                </div>
              </template>
            </m-column>
            <m-column prop="Quantity" label="SL" align="right" width="50px">
              <template #default="scope">
                <span
                  :class="{
                    'item--selected':
                      itemReport.InventoryItemId == scope.row?.InventoryItemId,
                  }"
                >
                  {{ scope.row.Quantity }}
                </span>
              </template>
            </m-column>
            <m-column label="Đơn giá" align="right" width="100px">
              <template #default="scope">
                <div
                  class="unit-price"
                  :class="{
                    'item--selected':
                      itemReport.InventoryItemId == scope.row?.InventoryItemId,
                  }"
                >
                  {{ commonJs.formatMoney(scope.row.UnitPrice) }}
                </div>
              </template>
            </m-column>
            <m-column
              label="Thành tiền"
              align="right"
              fixed="right"
              width="120px"
            >
              <template #default="scope">
                <div
                  :class="{
                    'item--selected':
                      itemReport.InventoryItemId == scope.row?.InventoryItemId,
                  }"
                >
                  {{ commonJs.formatMoney(scope.row.TotalAmount) }}
                </div>
              </template>
            </m-column>
          </m-table>
        </div>
        <!-- THÔNG TIN TIỀN -->
        <div class="invoice__money">
          <div class="money">
            <div class="money__title">Tổng tiền:</div>
            <div class="money__text">
              {{ commonJs.formatMoney(invoice.TotalAmount) }}
            </div>
          </div>
        </div>

        <!-- FOOTER -->
        <div class="invoice__footer">
          <div class="footer__left">
            <label for="">KHÁCH HÀNG</label>
            <div class="employee__name">{{ invoice.CustomerName }}</div>
          </div>
          <div class="footer__right">
            <div class="invoice__date">
              {{ commonJs.formatDateWithStringVN(invoice.RefDate) }}
            </div>
            <div class="invoice__employee">
              <div class="employee__text">NHÂN VIÊN BÁN HÀNG</div>
              <div class="employee__name">{{ invoice.EmployeeName }}</div>
            </div>
          </div>
        </div>
      </div>
      <!-- BẢN ĐỂ IN RA HÓA ĐƠN GIẤY -->
      <div v-if="showPrintPreview" id="print-element">
        <div class="p-org">
          <div class="p-org__name">
            {{ invoice.Organization?.OrganizationName }}
          </div>
          <div class="p-org__description">
            {{ invoice.Organization?.Description }}
          </div>
          <div class="p-org__address">{{ invoice.Organization?.Address }}</div>
          <div class="p-org__phone">
            <i class="icofont-ui-cell-phone"></i>:
            {{ invoice.Organization?.PhoneNumber }}
          </div>
        </div>
        <div class="p__title">HÓA ĐƠN BÁN HÀNG</div>
        <div class="p-invoice__code">
          <label>Số hóa đơn:</label> {{ invoice.RefNo }}
        </div>
        <div class="p-invoice__date">
          <label>Ngày:</label>
          {{ commonJs.formatDateTime(invoice.RefDate) }}
        </div>
        <div class="p-customer__name">
          <label>Khách hàng:</label> {{ invoice.CustomerName }}
        </div>
        <div class="p-customer__address">
          <label>Địa chỉ/ĐT:</label> {{ invoice.CustomerAddress }}
        </div>
        <div class="p-products">
          <div
            class="p-product"
            v-for="(p, index) in invoice.RefDetail"
            :key="index"
          >
            <div class="product__info">
              <div class="product__name">{{ p.InventoryItemName }}</div>
            </div>
            <div class="product__total-money">
              <div class="product__price">
                {{ commonJs.formatMoney(p.UnitPrice) }} x
                {{ p.Quantity }}
              </div>
              <div class="product__total">
                {{ commonJs.formatMoney(p.TotalAmount) }}
              </div>
            </div>
          </div>
        </div>
        <div class="p-total-money">
          <div class="p-money__label">TỔNG:</div>
          <div class="p-money__value">
            {{ commonJs.formatMoney(invoice.TotalAmount) }}
          </div>
        </div>
        <div class="p-person">
          <div class="p-customer"></div>
          <div class="p-employee">
            <div class="p-employee__title">Nhân viên bán hàng:</div>
            <div class="p-employee__name">{{ invoice?.EmployeeName }}</div>
          </div>
        </div>
        <div class="p-moreinfo">
          Quý khách vui lòng kiểm tra kỹ hóa đơn trước khi thanh toán.<br />
          Trân trọng cám ơn quý khách đã sử dụng dịch vụ!
        </div>

        <div class="p-author">Mạnh Hào Mart - www.manhhaomart.com</div>
      </div>
    </template>
    <template v-slot:footer>
      <div class="invoice__button">
        <button id="btn-close" class="btn btn--close" @click="onClose">
          <i class="icofont-ui-close"></i> Đóng
        </button>
        <button class="btn btn--default mg-left-10" @click="onPrint">
          <i class="icofont-printer"></i> In hóa đơn
        </button>
      </div>
    </template>
  </m-dialog>
</template>
<script>
export default {
  name: "ReportOrderDetail",
  emits: [],
  inject: ["refIdSelected", "itemReport"],
  created() {
    this.maxios.get(`refs/${this.refIdSelected}`).then((res) => {
      this.invoice = res;
      // this.$nextTick(function () {
      //   this.onPrint();
      // });
    });
  },
  methods: {
    onPrint() {
      this.showPrintPreview = true;
      this.$nextTick(function () {
        this.$htmlToPaper("print-element"); //invoice
        // this.$htmlToPaper("invoice"); //invoice
      });
    },

    onClose() {
      this.$emit("onClose");
    },
  },
  data() {
    return {
      showPrintPreview: false,
      invoice: {},
    };
  },
};
</script>
<style scoped>
.invoice {
  display: flex;
  flex-direction: column;
  padding: 32px 24px;
  width: 148mm;
  height: 210mm;
}

.org-info {
  flex-direction: column;
  background-color: #fff;
  font-weight: 400;
  align-items: flex-start;
  padding: 0;
  position: static;
  height: unset;
}

.org-info i {
  font-size: 16px;
}

.org__name {
  font-size: 16px;
  font-weight: 700;
  text-transform: uppercase;
  width: 100%;
  text-align: center;
}

.org__address,
.org__mobile {
  display: flex;
  column-gap: 4px;
  margin-top: 4px;
  align-items: flex-start;
}

.org__description {
  font-weight: 700;
  font-size: 20px;
  text-transform: uppercase;
}

.invoice__header {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  box-sizing: border-box;
  align-items: center;
}

.header__left {
  background-color: #fff;
  border: unset;
  flex: unset;
}

.header__right {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  flex: 1;
  height: 100%;
}

.invoice__title {
  text-transform: uppercase;
  font-weight: 700;
  font-size: 24px;
  text-align: center;
}

.invoice__refno {
  text-align: center;
  font-style: italic;
  margin-top: 10px;
}

.invoice__products {
  width: 100%;
  max-height: calc(100vh - 500px);
  flex: 1;
  overflow-y: auto;
}

.invoice__name {
  padding: 10px;
}

.invoice__date {
  font-style: italic;
  font-weight: 700;
}

.customer {
  margin-top: 15px;
  max-width: 400px;
}

.invoice__footer {
  display: flex;
  justify-content: space-between;
  padding: 10px 20px;
}

.footer__left {
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
}

.footer__right {
  text-align: center;
}

.employee__name {
  margin-top: 30px;
  font-weight: 700;
}

.money {
  display: flex;
  column-gap: 10px;
  align-items: center;
  width: 100%;
  justify-content: flex-end;
}

.money__title {
  text-decoration: underline;
  font-weight: 700;
}

.money__text {
  font-weight: 700;
  font-size: 20px;
}

.item--selected {
  background-color: yellow;
  padding: 0 4px;
}

@media (max-width: 720px) {
  .invoice__header > div {
    text-align: center;
    width: 100%;
  }

  .org__description {
    display: none;
  }
}

.el-table .cell {
  padding: 8px 4px !important;
  white-space: none !important;
  word-break: normal !important;
  /* width: fit-content !important; */
}
</style>
