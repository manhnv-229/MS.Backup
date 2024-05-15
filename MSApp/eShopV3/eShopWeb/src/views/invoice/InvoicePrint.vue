<template>
  <m-dialog @onClose="onClose">
    <template v-slot:content>
      <div v-show="!showPrintPreview" id="invoice" class="invoice">
        <!-- HEADER -->
        <div class="invoice__header">
          <div class="header__left">
            <div class="org-info">
              <div class="org__name">
                {{ invoice.Organization?.OrganizationName }}
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
          <div class="invoice__title">HÓA ĐƠN THANH TOÁN</div>
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
            :data="invoice.InvoiceDetail"
            size="small"
            empty-text="Không có sản phẩm/ dịch vụ"
            width="100%"
            height="100%"
          >
            <m-column type="index" label="#" width="40px" />
            <m-column prop="ProductName" label="SP/dịch vụ" width="130px">
              <template #default="scope">
                <div
                  class="flex"
                  :class="{
                    'is-delete':
                      scope.row.EntityState == Enum.EntityState.DELETE,
                  }"
                >
                  <span>{{ scope.row.Product?.ProductName }}</span>
                </div>
              </template>
            </m-column>
            <m-column prop="Quantity" label="SL" align="right"></m-column>
            <m-column label="Đơn giá" align="right" width="100px">
              <template #default="scope">
                <div class="unit-price">
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
                <div>{{ commonJs.formatMoney(scope.row.TotalMoney) }}</div>
              </template>
            </m-column>
          </m-table>
        </div>
        <!-- THÔNG TIN TIỀN -->
        <div class="invoice__money">
          <div class="money">
            <div class="money__title">Tổng tiền:</div>
            <div class="money__text">
              {{ commonJs.formatMoney(invoice.TotalMoney) }}
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
              {{ commonJs.formatDateWithStringVN(invoice.InvoiceDate) }}
            </div>
            <div class="invoice__employee">
              <div class="employee__text">NHÂN VIÊN BÁN HÀNG</div>
              <div class="employee__name">{{ invoice.Employee?.FullName }}</div>
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
          <label>Số hóa đơn:</label> {{ invoice.InvoiceCode }}
        </div>
        <div class="p-invoice__date">
          <label>Ngày:</label>
          {{ commonJs.formatDateTime(invoice.InvoiceDate) }}
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
            v-for="(p, index) in invoice.InvoiceDetail"
            :key="index"
          >
            <div class="product__info">
              <div class="product__name">{{ p.Product?.ProductName }}</div>
            </div>
            <div class="product__total-money">
              <div class="product__price">
                {{ commonJs.formatMoney(p.Product?.UnitPrice) }} x
                {{ p.Quantity }}
              </div>
              <div class="product__total">
                {{ commonJs.formatMoney(p.TotalMoney) }}
              </div>
            </div>
          </div>
        </div>
        <div class="p-total-money">
          <div class="p-money__label">TỔNG:</div>
          <div class="p-money__value">
            {{ commonJs.formatMoney(invoice.TotalMoney) }}
          </div>
        </div>
        <div class="p-person">
          <div class="p-customer"></div>
          <div class="p-employee">
            <div class="p-employee__title">Nhân viên bán hàng:</div>
            <div class="p-employee__name">{{ invoice.Employee?.FullName }}</div>
          </div>
        </div>
        <div class="p-moreinfo">
          Quý khách vui lòng kiểm tra kỹ hóa đơn trước khi thanh toán.<br />
          Trân trọng cám ơn quý khách đã sử dụng dịch vụ!
        </div>

        <div class="p-author">Mạnh Software - www.nmanh.com</div>
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
  name: "InvoicePrint",
  emits: [],
  props: {
    id: String,
  },
  created() {
    this.maxios.get(`invoices/${this.id}`).then((res) => {
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
        this.$htmlToPaper("print-element");
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
@import url(invoice.css);
</style>
