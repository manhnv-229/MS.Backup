<template>
  <m-dialog
    title="Báo cáo chi tiết"
    @onClose="onClose"
    width="100%"
    height="100%"
  >
    <template #content>
      <div class="m-report">
        <div class="report-info">
          <div class="m-row">
            <div class="m-col">
              <m-combobox
                label="Sản phẩm"
                propValue="InventoryItemId"
                :allowAllSelect="true"
                :firstSelected="false"
                :dataInput="inventories"
                v-model="itemSelectedId"
                propText="InventoryItemName"
                @onChangeValueSelected="loadData('inventory')"
                class="mg-left-8"
              >
              </m-combobox>
            </div>
            <!-- <div class="m-col">
                            <m-combobox label="Mục thống kê" url="/Dictionarys/report-types" propValue="Value"
                                :firstSelected="true" v-model="reportType" propText="Text" class="mg-left-8">
                            </m-combobox>
                        </div> -->
            <div class="m-col">
              <m-combobox
                label="Khách hàng"
                cls="combobox--custom"
                propValue="CustomerId"
                :dataInput="customers"
                :firstSelected="true"
                :allowAllSelect="true"
                v-model="reportInfo.CustomerId"
                propText="FullName"
                class="mg-left-8"
                @onChangeValueSelected="loadData('customer')"
              >
              </m-combobox>
            </div>
            <div class="m-col">
              <m-combobox
                label="Nhân viên bán hàng"
                cls="combobox--custom"
                propValue="EmployeeId"
                :dataInput="employees"
                :firstSelected="true"
                :allowAllSelect="true"
                :isDisabled="true"
                v-model="reportInfo.EmployeeId"
                propText="FullName"
                class="mg-left-8"
                @onChangeValueSelected="loadData('employee')"
              >
              </m-combobox>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-combobox
                label="Thời gian thống kê"
                :dataInput="timeRanges"
                propValue="Value"
                :firstSelected="true"
                v-model="reportInfo.TimeRange.Value"
                propText="Text"
                class="mg-left-8"
                @onChangeValueSelected="loadData('timerange')"
              >
              </m-combobox>
            </div>
            <div class="m-col">
              <label for="">Thời gian bắt đầu:</label>
              <el-date-picker
                placeholder="Chọn thời gian bắt đầu"
                v-model="reportInfo.TimeRange.StartDateTime"
                :disabled="disabledChangeTime"
                type="datetime"
                format="DD-MM-YYYY HH:mm:ss"
              />
            </div>
            <div class="m-col">
              <label for="">Thời gian kết thúc:</label>
              <el-date-picker
                placeholder="Chọn thời gian kết thúc"
                v-model="reportInfo.TimeRange.EndDateTime"
                :disabled="disabledChangeTime"
                type="datetime"
                format="DD-MM-YYYY HH:mm:ss"
              />
            </div>
          </div>
        </div>
        <div class="report-detail">
          <m-table
            ref="tbProfits"
            :data="reports"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
            :summary-method="getSummaries"
            show-summary
          >
            <m-column type="index" label="#" width="40px"></m-column>
            <!-- <m-column prop="InventoryItemCategoryName" label="Nhóm hàng hoá"></m-column>
                    <m-column prop="InventoryItemName" label="Hàng hoá">
                        <template #default="scope">
                            <a class="inventory-item--link-reference" @click="onSelectedItem(scope.row)">{{
                                scope.row.InventoryItemName }}</a>
                        </template>
                    </m-column> -->
            <m-column prop="InventoryItemName" label="Hàng hoá">
              <template #default="scope">
                <el-tooltip
                  class="box-item"
                  effect="customized"
                  :content="`${scope.row.InventoryItemName}`"
                  placement="top"
                >
                  {{ scope.row.InventoryItemName }}
                </el-tooltip>
              </template>
            </m-column>
            <m-column prop="RefNo" label="Hoá đơn tham chiếu" width="150">
              <template #default="scope">
                <a
                  class="inventory-item--link-reference"
                  @click="onShowOrderDetail(scope.row)"
                  >{{ scope.row.RefNo }}</a
                >
              </template>
            </m-column>
            <m-column prop="RefDate" label="Ngày thực hiện" align="center">
              <template #default="scope">
                <div
                  v-html="commonJs.formatDateTimeHTML(scope.row.RefDate)"
                ></div>
              </template>
            </m-column>
            <m-column prop="CustomerName" label="Khách hàng"></m-column>
            <m-column prop="Quantity" label="Số lượng bán" align="right">
            </m-column>
            <m-column
              prop="TotalCostPrice"
              label="Tổng tiền nhập"
              align="right"
            >
              <template #default="scope">
                <b
                  ><span>{{
                    commonJs.formatMoney(scope.row.TotalCostPrice)
                  }}</span></b
                >
              </template>
            </m-column>
            <m-column
              prop="TotalUnitPrice"
              label="Tổng tiền bán theo đơn giá"
              align="right"
            >
              <template #default="scope">
                <b
                  ><span
                    :class="{
                      'color--red':
                        scope.row.TotalUnitPrice < scope.row.TotalCostPrice,
                    }"
                    >{{ commonJs.formatMoney(scope.row.TotalUnitPrice) }}</span
                  ></b
                >
              </template>
            </m-column>
            <m-column
              prop="TotalAmount"
              label="Tổng tiền bán thực tế"
              align="right"
            >
              <template #default="scope">
                <b
                  ><span
                    :class="{
                      'color--red':
                        scope.row.TotalAmount < scope.row.TotalCostPrice,
                    }"
                    >{{ commonJs.formatMoney(scope.row.TotalAmount) }}</span
                  ></b
                >
              </template>
            </m-column>
            <m-column prop="TotalProfitAmount" label="Lợi nhuận" align="right">
              <template #default="scope">
                <b
                  ><span>{{
                    commonJs.formatMoney(scope.row.TotalProfitAmount)
                  }}</span></b
                >
              </template>
            </m-column>
          </m-table>
        </div>
      </div>
    </template>
    <template #footer>
      <div class="report__footer">
        <div class="report-money">
          <div>
            <label>Doanh thu:</label>
            {{ commonJs.formatMoney(total.TotalAmount) }}
          </div>
          <div>
            <label>Lợi nhuận:</label>
            {{ commonJs.formatMoney(total.TotalProfitAmount) }}
          </div>
        </div>
        <button class="btn btn--close" @click="onClose">
          <i class="icofont-ui-close" data-v-7b46c65e=""></i> Đóng
        </button>
      </div>
    </template>
  </m-dialog>
  <OrderDetail
    v-if="showOrderDetail"
    @onClose="showOrderDetail = false"
  ></OrderDetail>
</template>
<script>
import Enum from "@/scripts/enum";
import { mapGetters } from "vuex";
import OrderDetail from "./OrderDetail.vue";
import { computed } from "@vue/reactivity";
export default {
  name: "SaleReportByItem",
  components: { OrderDetail },
  emits: ["onClose"],
  props: ["InventoryReportInput"],
  inject: ["reportParam", "timeRanges"],
  created() {
    // this.reportInfo = JSON.parse(JSON.stringify(this.reportParam));
    this.itemSelectedId = this.InventoryReportInput.InventoryItemId;
    // Lấy dữ liệu:
    this.loadData();
  },
  computed: {
    ...mapGetters(["inventories", "customers", "employees"]),
    total: function () {
      const totalInfo = {
        TotalAmount: 0,
        TotalUnitPrice: 0,
        TotalCostPrice: 0,
        TotalProfitAmount: 0,
        TotalQuantity: 0,
      };
      if (this.reports) {
        for (const item of this.reports) {
          totalInfo.TotalAmount += item.TotalAmount;
          totalInfo.TotalUnitPrice += item.TotalUnitPrice;
          totalInfo.TotalCostPrice += item.TotalCostPrice;
          totalInfo.TotalProfitAmount += item.TotalProfitAmount;
          totalInfo.TotalQuantity += item.Quantity;
        }
      }
      return totalInfo;
    },
  },
  watch: {
    // 'reportInfo.CustomerId': function () {
    //     this.loadData();
    // },
    "reportInfo.TimeRange.Value": function (newValue) {
      var date = new Date();
      let timeRangeInfo = {};
      switch (newValue) {
        case Enum.TimeRanges.Today:
          timeRangeInfo = this.commonJs.getRangeDateTimeInDay(date);
          break;
        case Enum.TimeRanges.Yesterday:
          timeRangeInfo = this.commonJs.getRangeDateTimeInYesterday(date);
          break;
        case Enum.TimeRanges.ThisWeek:
          timeRangeInfo = this.commonJs.getRangeDateTimeWeekOfDate(date);
          break;
        case Enum.TimeRanges.ThisMonth:
          timeRangeInfo = this.commonJs.getRangeDateTimeMonthOfDate(date);
          break;
        case Enum.TimeRanges.ThisQuarter:
          timeRangeInfo = this.commonJs.getRangeDateTimeQuarterOfDate(date);
          break;
        case Enum.TimeRanges.ThisYear:
          timeRangeInfo = this.commonJs.getRangeDateTimeYearOfDate(date);
          break;
        default:
          timeRangeInfo = this.commonJs.getRangeDateTimeYearOfDate(date);
          break;
      }
      this.reportInfo.TimeRange.StartDateTime = timeRangeInfo.StartDateTime;
      this.reportInfo.TimeRange.EndDateTime = timeRangeInfo.EndDateTime;
    },
  },
  methods: {
    loadData() {
      var startDate = this.commonJs.convertDateTimeToStringDateTime(
        this.reportInfo?.TimeRange?.StartDateTime
      );
      var endDate = this.commonJs.convertDateTimeToStringDateTime(
        this.reportInfo?.TimeRange?.EndDateTime
      );
      const itemId = this.itemSelectedId ?? "";
      const categoryId = this.categoryId ?? "";
      const customerId = this.reportInfo.CustomerId ?? "";
      this.maxios
        .get(
          `reports/sale?startDate=${startDate}&endDate=${endDate}&categoryId=${categoryId}&inventoryItemId=${itemId}&customerId=${customerId}`
        )
        .then((res) => {
          this.reports = res;
        });
    },
    getSummaries(param) {
      const columns = param.columns;
      const sums = [];

      columns.forEach((column, index) => {
        if (index === 1) {
          sums[index] = "Tổng";
          return;
        }
        const propName = column.property;
        switch (propName) {
          case "TotalCostPrice":
            sums[index] = this.commonJs.formatMoney(this.total.TotalCostPrice);
            break;
          case "TotalUnitPrice":
            sums[index] = this.commonJs.formatMoney(this.total.TotalUnitPrice);
            break;
          case "TotalProfitAmount":
            sums[index] = this.commonJs.formatMoney(
              this.total.TotalProfitAmount
            );
            break;
          case "TotalAmount":
            sums[index] = this.commonJs.formatMoney(this.total.TotalAmount);
            break;
          case "Quantity":
            sums[index] = `SL: ${this.total.TotalQuantity}`;
            break;
          default:
            sums[index] = "";
            break;
        }
      });
      return sums;
    },
    onShowOrderDetail(item) {
      this.refIdSelected = item.RefId;
      this.showOrderDetail = true;
      this.itemSelectedForReportOrderDetail = item;
    },
    onClose() {
      this.$emit("onClose");
      event.stopPropagation();
    },
  },
  provide() {
    return {
      refIdSelected: computed(() => this.refIdSelected),
      itemReport: computed(() => this.itemSelectedForReportOrderDetail),
    };
  },
  data() {
    return {
      reports: [],
      reportInfo: JSON.parse(JSON.stringify(this.reportParam)),
      reportTimeParam: {},
      disabledChangeTime: false,
      itemSelectedId: null,
      showOrderDetail: false,
      refIdSelected: null,
      itemSelectedForReportOrderDetail: null,
      isFirstLoad: false,
    };
  },
};
</script>
<style scoped>
.m-report {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  height: 100%;
  row-gap: 16px;
  box-sizing: border-box;
}

.m-report > * {
  width: 100%;
}

.report-info {
  display: flex;
  flex-direction: column;
  column-gap: 10px;
  box-shadow: 0px 0px 10px #dedede;
  padding: 12px;
}

.report-detail {
  position: relative;
  flex: 1;
  overflow-y: auto;
}

.combobox--custom {
  margin-left: 0px !important;
}
</style>
