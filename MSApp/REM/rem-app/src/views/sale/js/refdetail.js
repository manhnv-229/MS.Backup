import router from "../../../router";
import Enum from "../../../scripts/enum";
import SelectInventoryItem from "../SelectInventoryItem.vue";
import BarcodeScan from "../BarcodeScan.vue";
import CustomerForm from "../../customers/CustomerDetail.vue";
import InventoryItemDetail from "@/views/inventory/InventoryItemDetail.vue";
import { mapGetters } from "vuex";
import { GET_ALL_CUSTOMER } from "../../../store/actions/entity";

import $ from "jquery";
export default {
  name: "RefDetail",
  components: {
    SelectInventoryItem,
    BarcodeScan,
    CustomerForm,
    InventoryItemDetail,
  },
  emits: ["update:isReload"],
  props: {
    id: String,
    isReload: Boolean,
  },

  created() {
    // document.addEventListener("keypress",function(){
    //   console.log("presss!!!", event);
    // })
    document.addEventListener("touchstart", null, { passive: true });
    // this.maxios.get(`products`).then((res) => {
    //   this.products = res;
    // });
    // Lấy các phương thức thanh toán:
    // this.maxios.get(`/dictionarys/payment-types`).then((res) => {
    //   this.invoicePayments = res;
    // });
    this.invoicePayments = this.paymentTypes;
    if (this.id) {
      this.maxios.get(`refs/${this.id}`).then((res) => {
        this.invoice = res;
        this.products = res.RefDetail;
        this.servicesData = this.invoice.ServiceInvoice;
        this.items = [...this.servicesData, ...this.products];
        if (this.invoice.DebitDetail?.length > 0) {
          this.allowDebit = true;
          this.debitDetail = this.invoice.DebitDetail[0];
        } else {
          this.allowDebit = false;
          // this.invoice.DebitDetail = [
          //   { PaymentType: Enum.PaymentType.CASH, Amount: 0 },
          // ];
        }
      });
    } else {
      // Đặt mặc định nhân viên thực hiện là nhân viên hiện tại:
      var employeeId = localStorage.getItem("employeeId");
      this.invoice.EmployeeId = employeeId;

      // Mặc định là phiếu bán hàng (phiếu xuất hàng):
      this.invoice.RefType = Enum.RefType.OUT_WAREHOUSE;

      // Mặc định là ngày hiện tại
      this.invoice.RefDate = new Date();

      // Mặc định khoản thanh toán là tiền mặt
      this.invoice.InvoicePayment = [
        { PaymentType: Enum.PaymentType.CASH, Amount: 0 },
      ];
      //   this.maxios.get(`refs/new-code`).then((res) => {
      //     this.invoice.RefNo = res;
      //     this.invoice.RefDate = new Date();
      //   });
    }
    this.$nextTick(() => {
      this.$refs.txtBarCode.setFocus();
    });
  },
  beforeUnmount() {
    $(document).off("onbarcodescaned", this.onScaner);
  },
  computed: {
    ...mapGetters(["routerStore", "paymentTypes", "customers", "employees"]),
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
          total += eval((product.UnitPrice || 0) * (product.Quantity || 0));
        }
      }
      for (const service of this.servicesData) {
        if (service.EntityState != Enum.EntityState.DELETE) {
          total += eval((service.UnitPrice || 0) * (service.Quantity || 0));
        }
      }
      var obj = {
        value: total,
        text: this.commonJs.formatMoney(total),
      };
      return obj;
    },
    totalAcrtualAmountComputed() {
      const amount = this.totalMoneyComputed?.value ?? 0;
      return Math.floor(amount);
    },
    totalAmoutByPaymentTypeComputed() {
      let totalAmout = 0;
      if (this.invoice?.InvoicePayment) {
        for (const payment of this.invoice.InvoicePayment) {
          const amount = Number.parseFloat(payment.Amount) ?? 0;
          totalAmout += amount;
        }
      }
      return totalAmout;
    },
    totalAmountHasNotCashComputed() {
      let totalAmountNotCash = 0;
      for (const payment of this.invoice.InvoicePayment) {
        if (payment.PaymentType != Enum.PaymentType.CASH) {
          const amount = Number.parseFloat(payment.Amount) ?? 0;
          totalAmountNotCash += amount;
        }
      }
      return totalAmountNotCash;
    },
    totalDebitComputed() {
      let debitDetail = this.debitDetail;
      if (debitDetail && debitDetail.Amount) {
        return debitDetail.Amount;
      } else {
        return 0;
      }
    },
  },
  watch: {
    paymentTypes: function (newValue) {
      this.invoicePayments = newValue;
    },
    // Tổng tiền thay đổi thì tính lại các phương thức thanh toán:
    // - Nếu có thanh toán bằng tiền mặt thì cộng thêm vào tiền mặt.
    // - Nếu không có thanh toán bằng tiền mặt thì cộng vào khoản phải trả
    /* eslint-disable */
    totalMoneyComputed: {
      handler: function (newValue) {
        this.invoice.TotalAmount = newValue.value ?? 0;
        this.invoice.ActualReceiveAmount = this.totalAcrtualAmountComputed;
      },
      deep: true,
    },
  },
  methods: {
    onFocusInQuantity(record) {
      this.itemEditting = record;
    },
    onFocusInUnitPrice(record) {
      this.itemEditting = record;
    },
    onFocusInMoney(record) {
      this.itemEditting = record;
    },
    onClickItem(item) {
      this.inventoryItemSelected = item;
      this.showInventoryItemDetail = true;
    },
    onCloseInventoryItemDetailForm() {
      this.inventoryItemSelected = {};
      this.showInventoryItemDetail = false;
    },
    onCustomerAdd() {
      this.showCustomerAddForm = true;
    },
    async onAddNewCustomerSuccess(customer) {
      await this.$store.dispatch(GET_ALL_CUSTOMER);
      await this.$refs.cbxCustomer.reloadDataAndSetSelect(customer.FullName);
    },
    // eslint-disable-next-line
    onAddOrDeletePaymentMethod(index, payment) {
      if (index == 0) {
        // Nếu tổng tiền của các hình thức thanh toán nhỏ hơn tổng tiền hóa đơn thì mới cho phép thêm hình thức thanh toán:
        if (
          this.totalAmoutByPaymentTypeComputed >= this.totalMoneyComputed.value
        ) {
          return;
        }
        const paymentTypeFirstNotSelect = this.invoicePayments.find(
          (p) => !p.DisableSelect
        );

        // Tính tiền cho hình thức thanh toán mới:
        const amount =
          this.totalMoneyComputed.value - this.totalAmoutByPaymentTypeComputed;

        if (paymentTypeFirstNotSelect) {
          const newInvoicePayment = {
            PaymentType: paymentTypeFirstNotSelect.Value,
            Amount: amount,
          };
          this.invoice.InvoicePayment.push(newInvoicePayment);
        }
      } else {
        // Cập nhật lại trạng thái của phương thức thanh toán để có thể lựa chọn được.
        const currentPaymentType = this.invoicePayments.find(
          (p) => p.Value == payment.PaymentType
        );
        if (currentPaymentType) {
          currentPaymentType.DisableSelect = false;
        }
        // Dồn tiền cho tiền mặt hoặc dồn vào khoản đầu tiền nếu không có tiền mặt:
        let cashPayment = this.getCashPayment();
        if (!cashPayment || payment.PaymentType == Enum.PaymentType.CASH) {
          this.invoice.InvoicePayment.splice(index, 1);
          this.invoice.InvoicePayment[0].Amount =
            Number.parseFloat(this.invoice.InvoicePayment[0].Amount) +
            Number.parseFloat(payment.Amount);
        } else {
          cashPayment.Amount =
            Number.parseFloat(cashPayment.Amount) +
            Number.parseFloat(payment.Amount);
        }
      }
      // Cuối cùng là tính lại tiền trả lại cho khách:
      this.invoice.ChangeAmount = 0;
    },
    onSelectedPayment(value, text, payment) {
      if (payment) {
        payment.DisableSelect = true;
      }
    },
    onClickAllowDebitCheckbox(currentCheck) {
      if (!currentCheck && !this.invoice.CustomerId) {
        event.preventDefault();
        this.commonJs.showConfirm(
          "Vui lòng chọn khách hàng trước khi ghi công nợ."
        );
      } else if (!currentCheck && !this.totalAcrtualAmountComputed) {
        event.preventDefault();
        this.commonJs.showConfirm(
          "Không thể ghi nợ khi số tiền thanh toán bằng 0."
        );
      }
    },
    onChangeAllowDebit(newValue) {
      const changeAmount = this.invoice.ChangeAmount;
      // Nếu chuyển cho phép ghi nợ, kiểm tra số tiền trả cho khách:
      // TH1: Khi số tiền trả lại bằng 0 thì chuyển toàn bộ số tiền thanh toán thành khoản nợ:
      // TH2: Tiền trả âm -> Khách nợ.
      // TH3: Tiền trả dương -> Nợ của khách.
      if (newValue) {
        // TH1: Khi số tiền trả lại bằng 0 thì chuyển toàn bộ số tiền thanh toán thành khoản nợ của khách:
        if (!changeAmount && this.totalAcrtualAmountComputed) {
          this.debitDetail.DebitType = Enum.DebitType.Receivable;
          this.debitDetail.Amount = -this.totalAcrtualAmountComputed; // Lấy số âm để xác định khoản này khách nợ.
          // Reset tất cả các khoản thanh toán, để mặc định 1 khoản là tiền mặt với số tiền bằng 0:
          this.invoice.InvoicePayment = [
            { PaymentType: Enum.PaymentType.CASH, Amount: 0 },
          ];
          return;
        }
        // TH2: Tiền trả âm -> Khách nợ.
        if (changeAmount && changeAmount < 0) {
          this.debitDetail.DebitType = Enum.DebitType.Receivable;
          this.debitDetail.Amount = changeAmount;
        } else {
          // TH3: Tiền trả dương -> Nợ của khách.
          this.debitDetail.DebitType = Enum.DebitType.Payable;
          this.debitDetail.Amount = changeAmount;
        }
        this.invoice.ChangeAmount = 0;
      } else {
        const cashPayment = this.getCashPayment();
        const debitAmount = this.debitDetail.Amount;
        this.debitDetail = {};
        // TH bỏ ghi nợ thì tính tổng lại các khoản tiền:
        // TH1: Có khoản thanh toán bằng tiền mặt -> dồn nợ vào tiền mặt
        if (cashPayment) {
          cashPayment.Amount =
            Number.parseFloat(cashPayment.Amount) +
            Number.parseFloat(Math.abs(debitAmount));
        } else {
          // TH2: Không có khoản tiền mặt nào, chuyển hết thành tiền trả lại dù âm hay dương.
          if (!this.invoice.ChangeAmount) {
            this.invoice.ChangeAmount = 0;
          }
          this.invoice.ChangeAmount += debitAmount;
        }
      }
    },

    onChangePaymentValueSelected(newValue, oldValue) {
      const oldPaymentSelected = this.invoicePayments.find(
        (p) => p.Value == oldValue
      );
      if (oldPaymentSelected) {
        oldPaymentSelected.DisableSelect = false;
      }
    },
    // onChangePaymentAmount(value) {
    //   console.log(value);
    //   this.invoice.ChangeAmount =
    //     this.totalAmoutByPaymentType - this.totalMoneyComputed.value;
    // },
    onSave() {
      this.invoice.RefDetail = this.products;
      this.invoice.ServiceInvoices = this.servicesData;
      // Loại bỏ các khoản ghi nợ có số tiền ghi nợ =0 (nếu có)
      if (this.invoice.DebitDetail) {
        const debitList = [];
        for (const item of this.invoice.DebitDetail) {
          if (!item.Amount) {
            debitList.push(item);
          }
        }
        this.invoice.DebitDetail = debitList;
      }
      // Gán thông tin nợ:
      if (this.debitDetail && this.debitDetail.Amount) {
        this.invoice.DebitDetail.push(this.debitDetail);
        this.invoice.PaymentStatus = Enum.PaymentStatus.PENDING; // Nợ hoặc chưa thanh toán
      } else {
        this.invoice.PaymentStatus = Enum.PaymentStatus.PAID; // Đã thanh toán
      }

      // gọi api tùy theo mode form:
      if (this.formMode == this.Enum.FormMode.ADD) {
        this.maxios.post("/refs", this.invoice).then(() => {
          this.$router.push("/refs");
        });
      } else if (this.formMode == this.Enum.FormMode.UPDATE) {
        this.maxios.put(`/refs/${this.id}`, this.invoice).then(() => {
          router.push("/refs");
        });
      }
    },
    getCashPayment() {
      const cash = this.invoice.InvoicePayment.find(
        (p) => p.PaymentType == Enum.PaymentType.CASH
      );
      return cash;
    },
    onSelectItemFilter(item) {
      item.Type = "inventory";
      item.Code = item.InventoryItemCode;
      item.Name = item.InventoryItemName;
      item.EmployeeIds = [this.invoice.EmployeeId];
      this.onChangeEmployeeService(item);
      this.processItemSelected(item);
    },

    onSelectServiceFilter(service) {
      // Gán mặc định khi chọn:
      service.Code = service.ServiceCode;
      service.Name = service.ServiceName;
      service.Type = "service";
      service.QuantityOrder = 1;
      service.TotalAmount = service.UnitPrice * service.QuantityOrder;
      service.EmployeeIds = [this.invoice.EmployeeId];
      this.onChangeEmployeeService(service);
      this.processServiceSelected(service);
    },

    getSelectionInventoryItems(products) {
      // Chuyển tất cả các sản phẩm đã chọn thành đối tượng RefDetail:
      for (const product of products) {
        this.processItemSelected(product);
      }
    },

    processItemSelected(product) {
      product.RefId = this.invoice.RefId;

      // Update lại tổng tiền cho phương thức thanh toán là tiền mặt hoặc phương thức đầu đầu tiên nếu không có tiền mặt:
      const totalAmount = product.TotalAmount;
      const cash = this.invoice.InvoicePayment.find(
        (p) => p.PaymentType == Enum.PaymentType.CASH
      );
      if (cash) {
        cash.Amount += totalAmount;
      } else {
        this.invoice.InvoicePayment[0].Amount += totalAmount;
      }

      // Kiểm tra xem trong danh sách sản phẩm đã có sản phẩm đã chọn hay chưa,
      // --> nếu có thì cập nhật số lượng, nếu không thì bổ sung vào danh sách
      var productExist = this.commonJs.getObjectExistInArray(
        this.products,
        product,
        "InventoryItemId"
      );
      if (productExist) {
        productExist.Quantity =
          eval(productExist.Quantity) + eval(product.QuantityOrder); // eval để xử lý vấn đề input nhận số là chuỗi.
        // Cập nhật lại tổng số tiền theo số lượng thực tế:
        productExist.TotalAmount =
          productExist.Quantity * productExist.UnitPrice;

        if (this.formMode == Enum.FormMode.ADD) {
          productExist.EntityState = Enum.EntityState.ADD;
        } else {
          productExist.EntityState = Enum.EntityState.UPDATE;
        }
      } else {
        // Nếu đang ở form sửa mà sản phẩm chọn không có trong giỏ hàng thì cập nhật ID khóa ngoại là thằng cha thôi
        product.InventoryItem = {
          EntityState: Enum.EntityState.ADD,
          InventoryItemCode: product.InventoryItemCode,
          InventoryItemName: product.InventoryItemName,
        };
        product.EntityState = Enum.EntityState.ADD;
        product.Quantity = eval(product.QuantityOrder);
        this.products.push(product);
        this.items = [...this.servicesData, ...this.products];
      }
    },
    processServiceSelected(service) {
      console.log(service);
      service.RefId = this.invoice.RefId;
      // Update lại tổng tiền cho phương thức thanh toán là tiền mặt hoặc phương thức đầu đầu tiên nếu không có tiền mặt:
      const totalAmount = service.TotalAmount;
      const cash = this.invoice.InvoicePayment.find(
        (p) => p.PaymentType == Enum.PaymentType.CASH
      );
      if (cash) {
        cash.Amount += totalAmount;
      } else {
        this.invoice.InvoicePayment[0].Amount += totalAmount;
      }

      // Kiểm tra xem trong danh sách dịch vụ đã có dịch vụ đã chọn hay chưa,
      // --> nếu có thì cập nhật số lượng, nếu không thì bổ sung vào danh sách
      var serviceExist = this.commonJs.getObjectExistInArray(
        this.servicesData,
        service,
        "ServiceId"
      );
      if (serviceExist) {
        serviceExist.Quantity =
          eval(serviceExist.Quantity) + eval(service.QuantityOrder); // eval để xử lý vấn đề input nhận số là chuỗi.
        // Cập nhật lại tổng số tiền theo số lượng thực tế:
        serviceExist.TotalAmount =
          serviceExist.Quantity * serviceExist.UnitPrice;

        if (this.formMode == Enum.FormMode.ADD) {
          serviceExist.EntityState = Enum.EntityState.ADD;
        } else {
          serviceExist.EntityState = Enum.EntityState.UPDATE;
        }
      } else {
        // Nếu đang ở form sửa mà dịch vụ chọn không có trong giỏ hàng thì cập nhật ID khóa ngoại là thằng cha thôi
        service.InventoryItem = {
          EntityState: Enum.EntityState.ADD,
          ServiceCode: service.ServiceCode,
          ServiceName: service.ServiceName,
        };
        service.EntityState = Enum.EntityState.ADD;
        service.Quantity = eval(service.QuantityOrder);
        this.servicesData.push(service);
        this.items = [...this.servicesData, ...this.products];
      }
    },
    onClose() {
      router.push(this.routerStore?.History?.from?.path || `/`);
    },
    onInputCustomerName(text) {
      this.invoice.CustomerName = text;
    },
    onSelectCustomer(value, text, customer) {
      if (customer) {
        this.invoice.CustomerAddress = customer.Mobile || customer.Address;
      }
    },

    onUnitPriceChanged(newUnitPrice) {
      // Cập nhật trạng thái của Entity:
      this.itemEditting.EntityState = Enum.EntityState.UPDATE;

      // Đầu tiên cập nhật tổng số tiền của sản phẩm:
      this.itemEditting.TotalAmount = newUnitPrice * this.itemEditting.Quantity;
      this.processAfterChangePriceOrQuantity();
    },
    onAmountPaymentItemChanged(value) {
      // Lấy tổng tiền cần thanh toán:
      const totalMoneyArtual = this.totalAcrtualAmountComputed ?? 0;

      // Tổng tiền khách thanh toán theo các hình thức:
      const totalAmoutByPaymentType = this.totalAmoutByPaymentTypeComputed;

      // Tiền ghi nợ  nếu có: lưu ý nếu đã có tiền ghi nợ thì không tự động bỏ đi, giữ nguyên.
      let totalDebit = this.totalDebitComputed ?? 0;

      // Tổng tiền cần trả lại cho khách hàng:
      const changeAmount =
        totalAmoutByPaymentType -
        totalMoneyArtual -
        (this.debitDetail?.Amount ?? 0);

      // Gán lại thông tin tiền cần trả lại cho khách:
      this.invoice.ChangeAmount = changeAmount;
    },
    /**
     * Xử lý khi thay đổi số lượng sản phẩm.
     * - Cập nhật tiền thông tin tiền của hoá đơn và của sản phẩm
     * @param {Number} quantity số lượng
     */
    onQuantityChanged(quantity) {
      // Cập nhật trạng thái của Entity:
      this.itemEditting.EntityState = Enum.EntityState.UPDATE;

      // Đầu tiên cập nhật tổng số tiền của sản phẩm:
      this.itemEditting.TotalAmount = quantity * this.itemEditting.UnitPrice;
      this.processAfterChangePriceOrQuantity();
    },

    processAfterChangePriceOrQuantity() {
      // Cần lấy ra các tổng tiền trước để làm việc:
      // Tổng tiền hiện tại thực tế thanh toán:
      const totalMoney = this.totalAcrtualAmountComputed ?? 0;
      let cashPayment = this.getCashPayment();

      // Tiền ghi nợ  nếu có: lưu ý nếu đã có tiền ghi nợ thì không tự động bỏ đi, giữ nguyên.
      let totalDebit = Math.abs(this.totalDebitComputed) ?? 0;

      //=> số tiền cần thanh toán
      let totalAmountNeedPay = totalMoney;

      // Từ tổng số tiền cân đối lại các khoản thanh toán theo từng hình thức
      //- Nếu thanh toán tiền mặt thì dồn hết vào tiền mặt.
      // - Nếu không có thanh toán tiền mặt thì dồn hết vào loại hình thanh toán đầu tiên trong danh sách:
      // 1. TH 1- Tổng tiền thanh toán nhỏ hơn tiền nợ => tiền nợ = tiền thanh toán. Khách không trả đồng nào cả.
      if (totalDebit >= totalAmountNeedPay) {
        totalDebit = totalAmountNeedPay;
        // => không thanh toán theo hình thức nào cả, cho mặc định 1 hình thức thanh toán là tiền mặt nhưng số tiền là 0.
        if (!cashPayment) {
          cashPayment = { PaymentType: Enum.PaymentType.CASH, Amount: 0 };
        } else {
          cashPayment.Amount = 0;
        }
        this.invoice.InvoicePayment = [cashPayment];
      } else {
        // 2. TH 2: Tổng tiền thanh toán > tiền nợ => Số tiền khách cần trả = tổng tiền thanh toán - tiền nợ. Số tiền nợ giữ nguyên.
        totalAmountNeedPay = totalMoney - totalDebit;
        // TH1: Có tiền mặt thì dồn vào tiền mặt
        // TH2: Không thanh toán bằng tiền mặt thì dồn hết vào tiền trả lại:
        if (cashPayment) {
          cashPayment.Amount =
            totalAmountNeedPay - this.totalAmountHasNotCashComputed;
        } else {
          if (!this.invoice.ChangeAmount) {
            this.invoice.ChangeAmount = 0;
          }
          this.invoice.ChangeAmount =
            this.totalAmoutByPaymentTypeComputed - totalAmountNeedPay;
        }
      }
      // Tính lại khoản trả lại:
      this.invoice.ChangeAmount = 0;
    },

    /* eslint-disable */
    onCellClick(row, column, cell, event) {
      this.itemEditting = row;
      console.log("cell click: ", row);
    },
    onRowClick(row, column, event) {
      console.log("row click: ", row);
    },

    onShowSelectInventoryItems() {
      this.showSelectInventoryItems = true;
    },

    /**---------------------------------------------------------
     * Xóa sản phẩm khỏi giỏ hàng
     * @param {Object} invoiceDetail thông tin chi tiết giỏ hàng
     */
    onDeleteInventoryItemInCart(invoiceDetail) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${invoiceDetail.Quantity}] <b>${invoiceDetail?.InventoryItemName}</b> khỏi giỏ hàng?`,
        () => {
          // Nếu form hoặc đối tượng đang ở trạng thái thêm mới thì xóa đi không cần nghĩ,
          //->> Nếu không thì cập nhật trạng thái cho đối tượng là Delete
          // -> Sau đó phía BackEnd sẽ dựa vào trạng thái này để thực hiện action tương ứng.
          if (
            this.formMode == Enum.FormMode.ADD ||
            invoiceDetail.EntityState == Enum.EntityState.ADD
          ) {
            var indexOfItem = this.products.findIndex(
              (i) => i.InventoryItemId == invoiceDetail.InventoryItemId
            );
            this.products.splice(indexOfItem, 1);
          } else {
            invoiceDetail.EntityState = Enum.EntityState.DELETE;
          }
          // Tính lại giá:
          this.processAfterChangePriceOrQuantity();
        }
      );
    },
    onUpdateInventoryItemInCart(product) {},
    onUndoDeleteInventoryItemInCart(invoiceDetail) {
      invoiceDetail.EntityState = Enum.EntityState.UPDATE;
    },
    onChangeEmployeeService(record) {
      console.log("row:", record);
      var ids = record.EmployeeIds;
      record.ServiceInvoiceEmployee = [];
      for (const id of ids) {
        const emp = this.employees.find((item) => item.EmployeeId === id);
        const newItem = {
          EmployeeId: emp.EmployeeId,
        };
        record.ServiceInvoiceEmployee.push(newItem);
      }
      console.log(record.ServiceInvoiceEmployee);
    },
    updateServiceInvoiceEmployee(employeeId, record) {},
  },
  data() {
    return {
      invoice: {
        ServiceInvoices: [],
        RefDetail: [],
        InvoicePayment: [{ PaymentType: 1 }],
        DebitDetail: [],
      },
      debitDetail: {}, // Thông tin ghi nợ
      isValidated: false,
      items: [],
      products: [],
      servicesData: [],
      itemEditting: null,
      invoicePayments: [],
      input: "",
      showSelectInventoryItems: false,
      showCustomerAddForm: false,
      showInventoryItemDetail: false,
      inventoryItemSelected: {},
      allowDebit: false,
    };
  },
};
