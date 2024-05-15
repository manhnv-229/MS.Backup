<template>
  <div class="invoice-detail">
    <div class="invoice-detail__header">
      <button class="btn--close-top" @click="onClose"><i class="icofont-ui-close"></i></button>
      <div class="invoice-detail__title">Thông tin hóa đơn</div>
    </div>
    <div class="invoice-detail__container">
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Mã hóa đơn"
            ref="txtInvoiceCode"
            v-model="invoice.InvoiceCode"
            :validated="isValidated"
            :required="true"
            :isDisabled="false"
            :isFocus="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <label for="">Ngày lập:</label>
          <el-date-picker
            v-model="invoice.InvoiceDate"
            type="datetime"
            format="DD/MM/YYYY (hh:mm:ss)"
          />
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Khách hàng"
            v-model="invoice.CustomerId"
            url="/customers"
            propText="FullName"
            @onChangeText="onInputCustomerName"
            @onSelected="onSelectCustomer"
            propValue="CustomerId"
          ></m-combobox>
        </div>
        <div class="m-col">
          <m-input
            label="SĐT/ Địa chỉ khách"
            ref="txtInvoiceCode"
            v-model="invoice.CustomerAddress"
            :validated="isValidated"
            :required="false"
            :isDisabled="false"
            :onlyNumberChar="true"
            type="tel"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Nhân viên thực hiện"
            :required="true"
            v-model="invoice.EmployeeId"
            url="/employees"
            propText="FullName"
            propValue="EmployeeId"
          ></m-combobox>
        </div>
        <div class="m-col"></div>
      </div>
      <!-- <div class="m-row">
          <m-text-area label="Mô tả" v-model="invoice.Description"></m-text-area>
        </div> -->
      <div class="m-row mg-top-0">
        <div class="products">
          <m-table-layout>
            <!-- TOOLBAR -->
            <!-- <template v-slot:toolbar>
                <div class="toolbar__left">
                  <m-input-search width="300px" placeholder="Tìm kiếm"></m-input-search>
                </div>
                <div class="toolbar__right"></div>
              </template> -->
            <!-- BẢNG DỮ LIỆU -->
            <template v-slot:container>
              <m-table
                ref="tbProducts"
                :data="products"
                size="small"
                empty-text="Không có sản phẩm/ dịch vụ"
                width="100%"
                height="100%"
                @cell-click="onCellClick"
              >
                <!-- <m-column type="index" /> -->
                <m-column prop="ProductCode" label="Mã SP" width="80px">
                  <template #default="scope">
                    <div
                      class="flex"
                      :class="{
                        'is-delete':
                          scope.row.EntityState == Enum.EntityState.DELETE,
                      }"
                    >
                      <span>{{ scope.row.Product?.ProductCode }}</span>
                    </div>
                  </template>
                </m-column>
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
                <m-column prop="Quantity" label="SL" align="right">
                  <template #default="scope">
                    <m-input
                      :ref="`txtMoney_${scope.row.ProductCode}`"
                      v-model="scope.row.Quantity"
                      :required="false"
                      :editOnClick="true"
                      :disabled="false"
                      :onlyNumberChar="true"
                      @onChange="onChangeQuantity"
                      @onBlur="onQuantityChanged"
                      align="right"
                    ></m-input>
                  </template>
                </m-column>
                <m-column label="Đơn giá" align="right" width="100px">
                  <template #default="scope">
                    <div class="unit-price">
                      <!-- <span>1 x</span> -->
                      <el-tag>
                        {{ commonJs.formatMoney(scope.row.UnitPrice) }}
                      </el-tag>
                    </div>
                  </template>
                </m-column>
                <m-column label="Thành tiền" align="right" width="120px">
                  <template #default="scope">
                    <m-input
                      format="money"
                      :ref="`txtMoney_${scope.row.ProductCode}`"
                      v-model="scope.row.TotalMoney"
                      :class="{
                        'is-delete':
                          scope.row.EntityState == Enum.EntityState.DELETE,
                      }"
                      :required="false"
                      :editOnClick="true"
                      :disabled="false"
                      align="right"
                    ></m-input>
                    <!-- <div>{{ commonJs.formatMoney(scope.row.UnitPrice) }}</div> -->
                  </template>
                </m-column>
                <m-column width="80px" fixed="right" align="center">
                  <template #header>
                    <button
                      id="btnAddProduct"
                      class="btn btn--nobg"
                      @click="onShowSelectProducts"
                    >
                      <span class="font-icon--flex"
                        ><i class="icofont-ui-add"></i
                        ><i class="icofont-cart-alt"></i
                      ></span>
                    </button>
                  </template>
                  <template #default="scope">
                    <div class="button-column">
                      <button
                        v-if="scope.row.EntityState != Enum.EntityState.DELETE"
                        class="btn--table-mini --color-edit"
                        :title="scope.row.ProductName"
                        @click="onUpdateProductInCart(scope.row)"
                      >
                        <i class="icofont-ui-edit"></i>
                      </button>
                      <button
                        v-if="scope.row.EntityState != Enum.EntityState.DELETE"
                        class="btn--table-mini --color-red"
                        :title="scope.row.ProductName"
                        @click="onDeleteProductInCart(scope.row)"
                      >
                        <i class="icofont-ui-delete"></i>
                      </button>
                      <button
                        v-if="scope.row.EntityState == Enum.EntityState.DELETE"
                        class="btn--table-mini --color-edit"
                        :title="scope.row.ProductName"
                        @click="onUndoDeleteProductInCart(scope.row)"
                      >
                        <i class="icofont-undo"></i>
                      </button>
                    </div>
                  </template>
                </m-column>
              </m-table>
            </template>
          </m-table-layout>
        </div>
      </div>
      <div class="footer--custom">
        <div class="invoice__money">
          <label class="money-label"><i class="icofont-pay"></i> </label>
          <span class="money-total">{{ totalMoneyComputed.text }}</span>
        </div>
        <div class="invoice__button">
          <button id="btn-close" class="btn btn--close" @click="onClose">
            Hủy
          </button>
          <button class="btn btn--default mg-left-10" @click="onSave">
            Lưu
          </button>
        </div>
      </div>
    </div>
  </div>
  <select-product
    v-if="showSelectProducts"
    @getSelectionProducts="getSelectionProducts"
    @onClose="showSelectProducts = false"
  ></select-product>
</template>
<script>
import router from "@/router";
import Enum from "@/scripts/enum";
import SelectProduct from "./SelectProduct.vue";
export default {
  name: "InvoiceDetail",
  components: { SelectProduct },
  emits: ["update:isReload"],
  props: {
    id: String,
    isReload: Boolean,
  },
  computed: {
    formMode: function () {
      if (this.id) {
        return Enum.FormMode.UPDATE;
      } else {
        return Enum.FormMode.ADD;
      }
    },
    totalMoneyComputed() {
      var total = 0;
      for (const product of this.products) {
        if (product.EntityState != Enum.EntityState.DELETE) {
          total += eval(product.TotalMoney || 0);
        }
      }
      var obj = {
        value: total,
        text: this.commonJs.formatMoney(total),
      };
      return obj;
    },
  },
  created() {
    document.addEventListener("touchstart", null, { passive: true });
    // this.maxios.get(`products`).then((res) => {
    //   this.products = res;
    // });
    if (this.id) {
      this.maxios.get(`invoices/${this.id}`).then((res) => {
        this.invoice = res;
        this.products = res.InvoiceDetail;
      });
    } else {
      // Đặt mặc định nhân viên thực hiện là nhân viên hiện tại:
      var employeeId = localStorage.getItem("employeeId");
      this.invoice.EmployeeId = employeeId;
      this.maxios.get(`invoices/new-code`).then((res) => {
        this.invoice.InvoiceCode = res;
        this.invoice.InvoiceDate = new Date();
      });
    }
  },
  watch: {
    input(newValue) {
      console.log(`input`, newValue);
    },
    totalMoneyComputed: {
      handler: function (newValue) {
        this.invoice.TotalMoney = newValue.value;
      },
      deep: true,
    },
  },
  methods: {
    onSave() {
      this.invoice.InvoiceDetail = this.products;
      if (this.formMode == this.Enum.FormMode.ADD) {
        this.maxios.post("/invoices", this.invoice).then(() => {
          this.$router.push("/invoices");
        });
      } else if (this.formMode == this.Enum.FormMode.UPDATE) {
        this.maxios.put(`/invoices/${this.id}`, this.invoice).then(() => {
          router.push("/invoices");
        });
      }
    },
    getSelectionProducts(products) {
      // Chuyển tất cả các sản phẩm đã chọn thành đối tượng InvoiceDetail:
      for (const product of products) {
        product.InvoiceId = this.invoice.InvoiceId;
        // Kiểm tra xem trong danh sách sản phẩm đã có sản phẩm đã chọn hay chưa,
        // --> nếu có thì cập nhật số lượng, nếu không thì bổ sung vào danh sách
        var productExist = this.commonJs.getObjectExistInArray(
          this.products,
          product,
          "ProductId"
        );
        if (productExist) {
          productExist.Quantity =
            eval(productExist.Quantity) + eval(product.QuantityOrder); // eval để xử lý vấn đề input nhận số là chuỗi.
          if (this.formMode == Enum.FormMode.ADD) {
            productExist.EntityState = Enum.EntityState.ADD;
          } else {
            productExist.EntityState = Enum.EntityState.UPDATE;
          }
        } else {
          // Nếu đang ở form sửa mà sản phẩm chọn không có trong giỏ hàng thì cập nhật ID khóa ngoại là thằng cha thôi
          product.Product = {
            EntityState: Enum.EntityState.ADD,
            ProductCode: product.ProductCode,
            ProductName: product.ProductName,
          };
          product.EntityState = Enum.EntityState.ADD;
          product.Quantity = eval(product.QuantityOrder);
          this.products.push(product);
        }
      }
      console.log(`products`, products);
    },
    onClose() {
      router.push(`/invoices`);
    },
    onInputCustomerName(text) {
      this.invoice.CustomerName = text;
    },
    onSelectCustomer(value, text, customer) {
      if (customer) {
        this.invoice.CustomerAddress = customer.Mobile || customer.Address;
      }
    },
    onChangeQuantity(quantity) {
      this.productEditting.TotalMoney =  quantity * this.productEditting.UnitPrice;
    },
    onQuantityChanged() {
      console.log("changed!!!");
      this.$nextTick(()=>{
        this.productEditting = null;
        console.log("item after: ", this.productEditting);
      })
    },
    /* eslint-disable */
    onCellClick(row, column, cell, event) {
      this.productEditting = row;
      console.log("item before: ", this.productEditting);
    },
    onShowSelectProducts() {
      this.showSelectProducts = true;
    },

    /**---------------------------------------------------------
     * Xóa sản phẩm khỏi giỏ hàng
     * @param {Object} invoiceDetail thông tin chi tiết giỏ hàng
     */
    onDeleteProductInCart(invoiceDetail) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa <b>${invoiceDetail.Product.ProductName}</b> khỏi giỏ hàng?`,
        () => {
          // Nếu form hoặc đối tượng đang ở trạng thái thêm mới thì xóa đi không cần nghĩ,
          //->> Nếu không thì cập nhật trạng thái cho đối tượng là Delete
          // -> Sau đó phía BackEnd sẽ dựa vào trạng thái này để thực hiện action tương ứng.
          if (
            this.formMode == Enum.FormMode.ADD ||
            invoiceDetail.EntityState == Enum.EntityState.ADD
          ) {
            var indexOfItem = this.products.findIndex(
              (i) => i.ProductId == invoiceDetail.ProductId
            );
            this.products.splice(indexOfItem, 1);
          } else {
            invoiceDetail.EntityState = Enum.EntityState.DELETE;
          }
        }
      );
    },
    onUpdateProductInCart(product) {},
    onUndoDeleteProductInCart(invoiceDetail) {
      invoiceDetail.EntityState = Enum.EntityState.UPDATE;
    },
  },
  data() {
    return {
      invoice: { InvoiceDetail: [] },
      isValidated: false,
      products: [],
      productEditting: null,
      input: "",
      showSelectProducts: false,
    };
  },
};
</script>
<style scoped>
.invoice-detail {
  position: relative;
  background-color: #fff;
  padding: 16px;
  border-radius: 4px;
}

.invoice-detail__header {
  display: flex;
  align-items: center;
  column-gap: 10px;
  margin-bottom: 24px;
}

.invoice-detail__title {
  font-size: 24px;
  font-weight: 700;
  border: 1px dotted #ccc;
  box-shadow: 0 0 3px #ccc;
  padding: 10px;
  flex: 1;
}

.btn--close-top {
  border:1px solid #ff0000;
  border-radius: 50%;
  position: absolute;
  width: 38px;
  height: 38px;
  top: -10px;
  right: -10px;
  font-size: 20px;
  padding: 0;
  color: #ff0000;
  background-color: #fff;
  cursor: pointer;
  box-shadow: 0px 0px 8px #6d6d6d;
}
.btn--close-top:hover {
  color: #bd0000;
}
.products {
  position: relative;
  width: 100%;
  min-height: 200px;
  max-height: 270px;
  flex: 1;
}

.unit-price {
  display: flex;
  align-items: center;
  justify-content: flex-end;
}
.font-icon--flex {
  display: flex;
  align-items: center;
  justify-content: center;
}
.font-icon--flex i:first-child {
  font-size: 13px;
  color: #00a43a;
}

#btnAddProduct:hover {
  border: 1px solid #00a43a;
}

.footer--custom {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-top: 16px;
}

.invoice__money {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.money-label {
  font-weight: 700;
  text-decoration: underline;
  font-size: 30px;
  color: #00a43a;
}

.money-total {
  font-size: 30px;
  color: #00a43a;
  font-weight: 700;
  margin-left: 10px;
}

.is-delete {
  color: #ff0000;
  text-decoration: line-through;
}
@media screen and (max-width: 400px) {
  #btn-close {
    display: none;
  }
}
</style>
