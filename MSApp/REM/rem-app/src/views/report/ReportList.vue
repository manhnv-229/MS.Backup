<template>
  <m-page title="Báo cáo/ Thống kê">
    <template v-slot:container>
      <m-table-layout>
        <!-- TOOLBAR -->
        <template v-slot:toolbar>
          <div class="report-toolbar">
            <div class="report__tab">
              <router-link to="/reports/revenue" class="tab__item">
                Doanh thu, Lợi nhuận
              </router-link>
              <router-link to="/reports/revenue-employee" class="tab__item"
                >Doanh thu theo nhân viên</router-link
              >
              <router-link to="/reports/revenue-customer" class="tab__item"
                >Doanh thu theo khách hàng</router-link
              >
              <router-link to="/reports/expenses" class="tab__item"
                >Chi phí</router-link
              >
            </div>
            <div class="report__option">
              <div class="toolbar__left">
                <div class="statistic-toolbar">
                  <!-- <m-combobox
                    label="Mục thống kê"
                    url="/Dictionarys/report-types"
                    propValue="Value"
                    :firstSelected="true"
                    :allowAllSelect="true"
                    v-model="reportType"
                    propText="Text"
                    class="mg-left-8"
                  >
                  </m-combobox> -->
                  <div class="m-row">
                    <div class="m-col">
                      <m-combobox
                        label="Thời gian thống kê"
                        propValue="Value"
                        :firstSelected="true"
                        :dataInput="timeRanges"
                        v-model="timeRangeOption.Value"
                        propText="Text"
                        class="mg-left-8"
                      >
                      </m-combobox>
                    </div>
                    <div class="m-col">
                      <m-combobox
                        v-model="reportParam.CategoryId"
                        :firstSelected="true"
                        :allowAllSelect="true"
                        :dataInput="categories"
                        label="Nhóm hàng hoá"
                        propValue="InventoryItemCategoryId"
                        propText="InventoryItemCategoryName"
                        class="mg-left-8"
                      >
                      </m-combobox>
                    </div>
                    <div class="m-col">
                      <m-combobox
                        label="Sản phẩm"
                        propValue="InventoryItemId"
                        :allowAllSelect="true"
                        :firstSelected="true"
                        :dataInput="inventories"
                        v-model="reportParam.InventoryItemId"
                        propText="InventoryItemName"
                        class="mg-left-8"
                      ></m-combobox>
                    </div>
                  </div>
                </div>
                <div
                  class="report__time-option"
                  v-if="timeRangeOption.Value == this.Enum.TimeRanges.Custom"
                >
                  <div class="m-row">
                    <!-- <div class="m-col">
                      <m-combobox
                        label="Thời gian thống kê"
                        url="/Dictionarys/time-range"
                        propValue="Value"
                        :firstSelected="true"
                        v-model="timeRangeOption.Value"
                        propText="Text"
                        class="mg-left-8"
                      >
                      </m-combobox>
                    </div> -->
                    <div class="m-col">
                      <label for="">Thời gian bắt đầu:</label>
                      <el-date-picker
                        placeholder="Chọn thời gian bắt đầu"
                        v-model="timeRangeOption.StartDateTime"
                        :disabled="disabledChangeTime"
                        type="datetime"
                        format="DD-MM-YYYY HH:mm:ss"
                      />
                    </div>
                    <div class="m-col">
                      <label for="">Thời gian kết thúc:</label>
                      <el-date-picker
                        placeholder="Chọn thời gian kết thúc"
                        v-model="timeRangeOption.EndDateTime"
                        :disabled="disabledChangeTime"
                        type="datetime"
                        format="DD-MM-YYYY HH:mm:ss"
                      />
                    </div>
                    <div class="m-col"></div>
                  </div>
                </div>
              </div>
              <div class="toolbar__right">
                <div class="toolbar__right-custom">
                  <button class="btn btn-refresh" @click="loadReport()">
                    <i class="icofont-refresh"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->
        <template v-slot:container>
          <router-view name="ReportRouterView"></router-view>
          <!-- Doanh thu -->
          <!-- <m-table
            v-if="reportType == Enum.ReportType.REVENUE"
            ref="tbCustomers"
            :data="reports"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column type="index" label="#" width="40px"></m-column>
            <m-column prop="FullName" label="Nhân viên"></m-column>
            <m-column
              prop="InventoryItemCategoryName"
              label="Nhóm SP/DV"
            ></m-column>
            <m-column prop="TotalAmount" label="Số tiền" align="right">
              <template #default="scope">
                <b
                  ><span>{{
                    commonJs.formatMoney(scope.row.TotalAmount)
                  }}</span></b
                >
              </template>
            </m-column>
          </m-table> -->
          <!-- Lợi nhuận -->
          <!-- <m-table
            v-else-if="
              reportType == Enum.ReportType.PROFIT || reportType == null
            "
            ref="tbProfits"
            :data="reports"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column type="index" label="#" width="40px"></m-column>
            <m-column
              prop="InventoryItemCategoryName"
              label="Nhóm hàng hoá"
            ></m-column>
            <m-column prop="InventoryItemName" label="Hàng hoá">
              <template #default="scope">
                <a
                  class="inventory-item--link-reference"
                  @click="onSelectedItem(scope.row)"
                  >{{ scope.row.InventoryItemName }}</a
                >
              </template>
            </m-column>
            <m-column prop="TotalQuantity" label="Số lượng bán" align="right">
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
          </m-table> -->
          <!-- <div v-else style="padding: 16px; font-size: 20px">
            Thống kê chi phí hiện chưa khả dụng. Vui lòng thử lại sau.
          </div> -->
        </template>
        <!-- PHÂN TRANG -->
        <template v-slot:footer>
          <!-- <m-paging
            :totalRecords="totalRecords"
            :pageSizeData="[10, 20, 50, 100, 200]"
            v-model:limit="limit"
            v-model:offset="offset"
            @onChangePagesize="loadData"
            @onPrevPage="loadData"
            @onNextPage="loadData"
            @onInitPaging="onInitPaging"
          ></m-paging> -->
        </template>
      </m-table-layout>
    </template>
    <template v-slot:footer>
      <!-- <div class="total">
        <div class="--left"></div>
        <div class="--right report-money">
          <div>
            <label>Doanh thu:</label> {{ commonJs.formatMoney(totalAmount) }}
          </div>
          <div>
            <label>Lợi nhuận:</label>
            {{ commonJs.formatMoney(totalProfitAmount) }}
          </div>
        </div>
      </div> -->
    </template>
  </m-page>
  <SaleReportByItem
    v-if="showSaleReportByItem"
    :InventoryReportInput="inventorySelected"
    @onClose="showSaleReportByItem = false"
  ></SaleReportByItem>
</template>
<script>
import Enum from "../../scripts/enum";
import SaleReportByItem from "./SaleReportByItem.vue";
import { computed } from "@vue/reactivity";
import { mapGetters } from "vuex";
export default {
  name: "ReportList",
  components: {
    SaleReportByItem,
  },
  emits: [],
  props: [],
  async created() {
    const me = this;
    // load các dữ liệu cho combobox:
    await this.maxios.get("/Dictionarys/time-range").then((data) => {
      this.timeRanges = data;
    });
    await this.maxios.get("/InventoryItemCategories").then((data) => {
      this.categories = data;
    });

    this.$emitter.on("updateFirstLoad", function () {
      me.isFirstLoad = false;
    });
    // this.$emitter.on("updateReportData", function (data) {
    //   me.reports = data;
    // });
  },
  watch: {
    "timeRangeOption.Value": function (newValue) {
      var date = new Date();
      var timeInfo = {};
      switch (newValue) {
        case Enum.TimeRanges.Today:
          timeInfo = this.commonJs.getRangeDateTimeInDay(date);
          break;
        case Enum.TimeRanges.Yesterday:
          timeInfo = this.commonJs.getRangeDateTimeInYesterday(date);
          break;
        case Enum.TimeRanges.ThisWeek:
          timeInfo = this.commonJs.getRangeDateTimeWeekOfDate(date);
          break;
        case Enum.TimeRanges.ThisMonth:
          timeInfo = this.commonJs.getRangeDateTimeMonthOfDate(date);
          break;
        case Enum.TimeRanges.ThisQuarter:
          timeInfo = this.commonJs.getRangeDateTimeQuarterOfDate(date);
          break;
        case Enum.TimeRanges.ThisYear:
          timeInfo = this.commonJs.getRangeDateTimeYearOfDate(date);
          break;
        default:
          timeInfo = this.commonJs.getRangeDateTimeYearOfDate(date);
          break;
      }
      this.timeRangeOption.StartDateTime = timeInfo?.StartDateTime;
      this.timeRangeOption.EndDateTime = timeInfo?.EndDateTime;
      this.reportParam.TimeRange = this.timeRangeOption;
    },
  },
  computed: {
    ...mapGetters(["inventories"]),
    totalAmount: function () {
      let amount = 0;
      if (this.reports) {
        for (const item of this.reports) {
          switch (this.reportType) {
            case this.Enum.ReportType.REVENUE:
              amount += item.TotalAmount;
              break;
            case this.Enum.ReportType.PROFIT:
              amount += item.TotalProfitAmount;
              break;
            default:
              amount = 0;
              break;
          }
          // amount += item.TotalAmount;
        }
      }
      return amount;
    },
  },
  methods: {
    loadReport() {
      this.$emitter.emit("loadDataReport");
    },
    onSelectedItem(item) {
      this.inventorySelected = item;
      this.showSaleReportByItem = true;
    },
  },
  provide() {
    return {
      reportParam: computed(() => this.reportParam),
      isFirstLoad: computed(() => this.isFirstLoad),
      reloadExpense: computed(() => this.reloadExpense),
      timeRangeInfo: computed(() => this.timeRangeOption),
      timeRanges: computed(() => this.timeRanges),
    };
  },
  data() {
    return {
      reports: [],
      categories: [],
      timeRanges: [],
      timeRangeOption: {},
      reportParam: {
        CustomerId: null,
        EmployeeId: null,
      },
      categoryId: null,
      reportType: null,
      isFirstLoad: false,
      reloadExpense: false,
      inventorySelected: null,
      showSaleReportByItem: false,
      reportTabActive: "RevenueAndProfit",
      disabledChangeTime: false,
    };
  },
};
</script>
<style scoped>
@import url(./report.css);
.toolbar__left {
  display: flex;
  flex-direction: column;
  justify-content: center;
  row-gap: 10px;
  align-items: start;
  flex: 1;
}
.toolbar__right {
  height: 100%;
}
</style>
