<template>
  <div class="invoice-detail">
    <button class="btn--close-top" @click="onClose">
      <i class="icofont-ui-close"></i>
    </button>
    <!-- <div class="invoice-detail__header">
     
      <div class="invoice-detail__title">Thông tin hóa đơn</div>
    </div> -->
    <div class="invoice-detail__container">
      <div class="order__left">
        <div class="order-info">
          <div class="item-barcode">
            <BarcodeScan
              placeholder="Nhập mã vạch, mã SKU hoặc tên sản phẩm"
              ref="txtBarCode"
              @onSelectItem="onSelectItemFilter"
            ></BarcodeScan>
          </div>
          <m-combobox
            label="Nhân viên bán hàng"
            :required="true"
            v-model="invoice.EmployeeId"
            :dataInput="employees"
            propText="FullName"
            propValue="EmployeeId"
          ></m-combobox>
        </div>
        <!-- <div class="employee-info">
          <div class="m-row">
            <div class="m-col"></div>
            <div class="m-col"></div>
          </div>
        </div> -->
        <div class="item-list">
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
                  ref="tbInventoryItems"
                  :data="products"
                  size="small"
                  empty-text="Không có sản phẩm/ dịch vụ"
                  width="100%"
                  height="100%"
                  @cell-click="onCellClick"
                >
                  <m-column label="#" type="index" />
                  <m-column label="Hình ảnh" width="100" align="left">
                    <template #default="scope">
                      <div class="inventory-img">
                        <img :src="scope.row.ImgFullPath" alt="" />
                      </div>
                    </template>
                  </m-column>
                  <m-column prop="InventoryItemCode" label="Mã SP">
                    <template #default="scope">
                      <div
                        class="flex"
                        :class="{
                          'is-delete':
                            scope.row.EntityState == Enum.EntityState.DELETE,
                        }"
                      >
                        <span>{{ scope.row.InventoryItemCode }}</span>
                      </div>
                    </template>
                  </m-column>
                  <m-column prop="InventoryItemName" label="Tên hàng hóa">
                    <template #default="scope">
                      <div
                        class="flex"
                        :class="{
                          'is-delete':
                            scope.row.EntityState == Enum.EntityState.DELETE,
                        }"
                      >
                        <el-tooltip
                          class="box-item"
                          effect="customized"
                          :content="scope.row.InventoryItemName"
                          placement="top"
                        >
                          <a
                            class="link-item"
                            @click="onClickItem(scope.row)"
                            >{{ scope.row.InventoryItemName }}</a
                          >
                        </el-tooltip>
                      </div>
                    </template>
                  </m-column>
                  <m-column
                    prop="Quantity"
                    label="SL"
                    width="100"
                    align="right"
                  >
                    <template #default="scope">
                      <m-input
                        :ref="`txtMoney_${scope.row.InventoryItemCode}`"
                        v-model="scope.row.Quantity"
                        class="input--table"
                        :required="false"
                        :editOnClick="true"
                        :disabled="false"
                        :onlyNumberChar="true"
                        @onChange="onQuantityChanged"
                        @onFocusIn="onFocusInQuantity(scope.row)"
                        align="right"
                      ></m-input>
                    </template>
                  </m-column>
                  <m-column label="Đơn giá" align="right" width="100px">
                    <template #default="scope">
                      <div class="unit-price">
                        <m-input
                          :ref="`txtMoney_${scope.row.InventoryItemCode}`"
                          v-model="scope.row.UnitPrice"
                          format="money"
                          class="input--table"
                          :required="true"
                          :editOnClick="true"
                          :disabled="false"
                          :onlyNumberChar="true"
                          @onChange="onUnitPriceChanged"
                          @onFocusIn="onFocusInUnitPrice(scope.row)"
                          align="right"
                        ></m-input>
                        <!-- <el-tag>
                          {{ commonJs.formatMoney(scope.row.UnitPrice) }}
                        </el-tag> -->
                      </div>
                    </template>
                  </m-column>
                  <m-column label="Thành tiền" align="right" width="120px">
                    <template #default="scope">
                      <!-- <m-input
                        format="money"
                        :ref="`txtMoney_${scope.row.InventoryItemCode}`"
                        v-model="scope.row.TotalAmount"
                        class="input--table"
                        :class="{
                          'is-delete':
                            scope.row.EntityState == Enum.EntityState.DELETE,
                        }"
                        @onFocusIn="onFocusInMoney(scope.row)"
                        :required="false"
                        :editOnClick="true"
                        :disabled="false"
                        align="right"
                      ></m-input> -->
                      <el-tag>
                        {{ commonJs.formatMoney(scope.row.TotalAmount) }}
                      </el-tag>
                      <!-- <div>{{ commonJs.formatMoney(scope.row.UnitPrice) }}</div> -->
                    </template>
                  </m-column>
                  <m-column width="80px" fixed="right" align="center">
                    <template #header>
                      <button
                        id="btnAddInventoryItem"
                        class="btn btn--nobg"
                        @click="onShowSelectInventoryItems"
                      >
                        <span class="font-icon--flex"
                          ><i class="icofont-ui-add"></i
                          ><i class="icofont-cart-alt"></i
                        ></span>
                      </button>
                    </template>
                    <template #default="scope">
                      <div class="button-column">
                        <!-- <button
                            v-if="
                              scope.row.EntityState != Enum.EntityState.DELETE
                            "
                            class="btn--table-mini --color-edit"
                            :title="scope.row.InventoryItemName"
                            @click="onUpdateInventoryItemInCart(scope.row)"
                          >
                            <i class="icofont-ui-edit"></i>
                          </button> -->
                        <button
                          v-if="
                            scope.row.EntityState != Enum.EntityState.DELETE
                          "
                          class="btn--table-mini --color-red"
                          :title="scope.row.InventoryItemName"
                          @click="onDeleteInventoryItemInCart(scope.row)"
                        >
                          <i class="icofont-close"></i>
                        </button>
                        <button
                          v-if="
                            scope.row.EntityState == Enum.EntityState.DELETE
                          "
                          class="btn--table-mini --color-edit"
                          :title="scope.row.InventoryItemName"
                          @click="onUndoDeleteInventoryItemInCart(scope.row)"
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
      </div>
      <!-- THÔNG TIN HOÁ ĐƠN -->
      <div class="order__right">
        <div class="customer-info">
          <div class="m-row">
            <div class="m-col">
              <label for="">Ngày lập:</label>
              <el-date-picker
                v-model="invoice.RefDate"
                type="datetime"
                format="DD/MM/YYYY (hh:mm:ss)"
              />
            </div>
            <div class="m-col"></div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-combobox
                label="Khách hàng"
                v-model="invoice.CustomerId"
                :dataInput="customers"
                propValue="CustomerId"
                propText="FullName"
                @onChangeText="onInputCustomerName"
                @onSelected="onSelectCustomer"
                :showAddButton="true"
                @onClickExtButton="onCustomerAdd"
                ref="cbxCustomer"
              ></m-combobox>
            </div>
            <div class="m-col">
              <m-input
                label="SĐT/ Địa chỉ khách"
                ref="txtRefNo"
                v-model="invoice.CustomerAddress"
                :required="false"
                :isDisabled="false"
                :onlyNumberChar="true"
                type="tel"
              >
              </m-input>
            </div>
          </div>
        </div>
        <!-- THÔNG TIN THANH TOÁN -->
        <div class="amount-info">
          <div class="invoice__money">
            <label class="money-label"
              ><i class="icofont-pay"></i> Tổng tiền:
            </label>
            <span class="money-total">{{ totalMoneyComputed.text }}</span>
          </div>
          <div class="money-acrtual">
            <label for="">Tiền khách phải trả:</label>
            <div class="amount-acrtual">
              <el-tag>{{
                commonJs.formatMoney(invoice.ActualReceiveAmount)
              }}</el-tag>
            </div>
          </div>
          <div class="money__remain">
            <label for="money-remain">Các khoản thu từ khách:</label>
            <div class="payment-list">
              <div
                class="payment-item"
                v-for="(payment, index) in invoice.InvoicePayment"
                :key="payment"
              >
                <m-combobox
                  v-model="payment.PaymentType"
                  propValue="Value"
                  propText="Text"
                  cls="m-combobox--custom"
                  :dataInput="invoicePayments"
                  :iconExtCls="{ 'icon--delete': index != 0 }"
                  :showAddButton="true"
                  :ref="`cbxPayment_${index}`"
                  @onSelected="onSelectedPayment"
                  @onClickExtButton="onAddOrDeletePaymentMethod(index, payment)"
                  @onChangeValueSelected="onChangePaymentValueSelected"
                ></m-combobox>
                <div class="payment-value">
                  <m-input
                    cls="minput--custom"
                    format="money"
                    :ref="`cbxPaymentAmount_${index}`"
                    :onlyNumberChar="true"
                    v-model="payment.Amount"
                    @onChange="onAmountPaymentItemChanged"
                    align="right"
                  ></m-input>
                </div>
              </div>
            </div>
          </div>
          <div class="change-amount">
            <label for="">Tiền trả lại khách:</label>
            <div class="money-change">
              {{ commonJs.formatMoney(invoice.ChangeAmount) }}
            </div>
          </div>
          <div class="change-action">
            <m-checkbox
              label="Ghi nợ"
              :disabled="
                !this.totalAcrtualAmountComputed && !debitDetail.Amount
              "
              v-model="allowDebit"
              @onClick="onClickAllowDebitCheckbox"
              @onChange="onChangeAllowDebit"
            ></m-checkbox>
            <div class="money-debit">
              {{ commonJs.formatMoney(Math.abs(debitDetail.Amount)) }}
              <div class="debit-info" v-if="debitDetail.Amount">
                <span v-if="debitDetail.DebitType == Enum.DebitType.Payable"
                  >(Nợ khách)</span
                ><span v-else>(Khách ghi nợ)</span>
              </div>
            </div>
          </div>
          <div>
            <m-input
              cls="minput--custom"
              v-model="invoice.JournalMemo"
              placeholder="Ghi chú..."
            ></m-input>
          </div>
        </div>
        <div class="invoice__button">
          <button id="btnCancel" class="btn btn--close" @click="onClose">
            Hủy
          </button>
          <button
            id="btnSave"
            class="btn btn--default mg-left-10"
            @click="onSave"
          >
            Lưu & Thanh toán
          </button>
        </div>
      </div>
      <!-- <div class="m-row">
            <m-text-area label="Mô tả" v-model="invoice.Description"></m-text-area>
          </div> -->
    </div>
  </div>
  <SelectInventoryItem
    v-if="showSelectInventoryItems"
    @getSelectionInventoryItems="getSelectionInventoryItems"
    @onClose="showSelectInventoryItems = false"
  ></SelectInventoryItem>
  <CustomerForm
    v-if="showCustomerAddForm"
    @onClose="showCustomerAddForm = false"
    @onSaveSuccess="onAddNewCustomerSuccess"
  ></CustomerForm>
  <InventoryItemDetail
    :id="inventoryItemSelected.InventoryItemId"
    :mode="Enum.FormMode.VIEW"
    v-if="showInventoryItemDetail"
    @onClose="onCloseInventoryItemDetailForm"
  ></InventoryItemDetail>
</template>
<script>
import refdetail from "./js/refdetail";
export default refdetail;
</script>
<style lang="css" scoped>
@import "./detail.css";
</style>
