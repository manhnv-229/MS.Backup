<template>
  <div class="report-detail" v-loading="loadingData">
    <div class="report-table">
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
        <m-column
          prop="InventoryItemCategoryName"
          label="Nhóm hàng hoá"
        ></m-column>
        <m-column prop="InventoryItemName" label="Hàng hoá">
          <template #default="scope">
            <a
              class="inventory-item--link-reference"
              @click.prevent="onSelectedItem(scope.row)"
            >
              <el-tooltip
                class="box-item"
                effect="customized"
                :content="`${scope.row.InventoryItemName}`"
                placement="top"
              >
                {{ scope.row.InventoryItemName }}
              </el-tooltip>
            </a>
          </template>
        </m-column>
        <m-column prop="TotalQuantity" label="Số lượng bán" align="right">
        </m-column>
        <m-column prop="TotalCostPrice" label="Tổng tiền nhập" align="right">
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
    <div class="total">
      <div class="--left"></div>
      <div class="--right report-money">
        <div>
          <label>Doanh thu:</label>
          {{ commonJs.formatMoney(total.TotalAmount) }}
        </div>
        <div>
          <label>Lợi nhuận:</label>
          {{ commonJs.formatMoney(total.TotalProfitAmount) }}
        </div>
      </div>
    </div>
  </div>
  <SaleReportByItem
    v-if="showSaleReportByItem"
    :InventoryReportInput="inventorySelected"
    @onClose="showSaleReportByItem = false"
  ></SaleReportByItem>
</template>
<script>
/* eslint-disable */
import { computed } from "@vue/reactivity";
import SaleReportByItem from "./SaleReportByItem.vue";
import commonJs from "../../scripts/common";
export default {
  name: "RevenueReport",
  emits: [],
  props: [],
  inject: ["reportParam", "timeRangeInfo", "isFirstLoad"],
  components: { SaleReportByItem },
  created() {
    if (this.reportParam.TimeRange) {
      this.loadData();
    }
    this.$emitter.on("loadDataReport", this.loadData);
  },
  beforeUnmount() {
    this.$emitter.off("loadDataReport", this.loadData);
  },
  computed: {
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
          totalInfo.TotalQuantity += item.TotalQuantity;
        }
      }
      return totalInfo;
    },
  },

  watch: {
    reportParam: {
      handler: function (newValue) {
        this.loadData();
      },
      deep: true,
    },
  },
  methods: {
    async loadData() {
      this.loadingData = true;
      const reportParam = this.reportParam;
      const startDate = this.commonJs.formatDateTimeYYMMDDHHMMSS(
        reportParam.TimeRange?.StartDateTime
      );
      const endDate = this.commonJs.formatDateTimeYYMMDDHHMMSS(
        reportParam.TimeRange?.EndDateTime
      );
      await this.maxios
        .get(
          `reports/profit?startDate=${startDate}&endDate=${endDate}&categoryId=${
            reportParam?.CategoryId ?? ""
          }`,
          null,
          true
        )
        .then((res) => {
          this.reports = res;
          this.$emitter.emit("updateFirstLoad");
          this.$nextTick(function () {
            this.loadingData = false;
          });
        });
    },
    onSelectedItem(item) {
      this.inventorySelected = item;
      this.showSaleReportByItem = true;
    },
    onShowOrderDetail(item) {
      this.refIdSelected = item.RefId;
      this.showOrderDetail = true;
      this.itemSelectedForReportOrderDetail = item;
    },
    getSummaries(param) {
      const columns = param.columns;
      const data = param.data;
      const sums = [];

      columns.forEach((column, index) => {
        if (index === 1) {
          sums[index] = "Tổng";
          return;
        }
        const propName = column.property;
        switch (propName) {
          case "TotalCostPrice":
            sums[index] = commonJs.formatMoney(this.total.TotalCostPrice);
            break;
          case "TotalUnitPrice":
            sums[index] = commonJs.formatMoney(this.total.TotalUnitPrice);
            break;
          case "TotalProfitAmount":
            sums[index] = commonJs.formatMoney(this.total.TotalProfitAmount);
            break;
          case "TotalAmount":
            sums[index] = commonJs.formatMoney(this.total.TotalAmount);
            break;
          case "TotalQuantity":
            sums[index] = this.total.TotalQuantity;
            break;
          default:
            sums[index] = "";
            break;
        }

        // const values = data.map((item) => Number(item[column.property]));
        // if (!values.every((value) => Number.isNaN(value))) {
        //   let currentValue = `${values.reduce((prev, curr) => {
        //     const value = Number(curr);
        //     if (!Number.isNaN(value)) {
        //       return prev + curr;
        //     } else {
        //       return prev;
        //     }
        //   }, 0)}`;
        //   if (
        //     propName == "TotalCostPrice" ||
        //     propName == "TotalUnitPrice" ||
        //     propName == "TotalProfitAmount" ||
        //     propName == "TotalAmount"
        //   ) {
        //     currentValue = commonJs.formatMoney(currentValue);
        //   }
        //   sums[index] = currentValue;
        // } else {
        //   sums[index] = "";
        // }
      });
      return sums;
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
      changeCount: 0,
      reloadData: false,
      showOrderDetail: false,
      showSaleReportByItem: false,
      inventorySelected: null,
      refIdSelected: null,
      itemSelectedForReportOrderDetail: null,
      loadingData: false,
    };
  },
};
</script>
<style scoped>
@import url(./report.css);
</style>
