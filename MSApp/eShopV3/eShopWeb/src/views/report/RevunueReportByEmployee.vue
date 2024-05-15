<template>
  <div class="report-detail">
    <div class="report-table">
      <m-table
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
              ><span>{{ commonJs.formatMoney(scope.row.TotalAmount) }}</span></b
            >
          </template>
        </m-column>
      </m-table>
    </div>
    <div class="total">
      <div class="--left"></div>
      <div class="--right report-money">
        <div>
          <label>Doanh thu:</label> {{ commonJs.formatMoney(totalAmount) }}
        </div>
        <!-- <div>
        <label>Lợi nhuận:</label>
        {{ commonJs.formatMoney(totalProfitAmount) }}
      </div> -->
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: "RevenueReportByEmployee",
  inject: ["reportParam", "timeRangeInfo", "isFirstLoad"],
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
    totalAmount: function () {
      let amount = 0;
      if (this.reports) {
        for (const item of this.reports) {
          amount += item.TotalAmount;
        }
      }
      return amount;
    },
    totalProfitAmount: function () {
      let amount = 0;
      if (this.reports) {
        for (const item of this.reports) {
          amount += item.TotalProfitAmount;
        }
      }
      return amount;
    },
  },
  watch: {
    reportParam: {
      handler: function () {
        this.loadData();
      },
      deep: true,
    },
  },
  methods: {
    loadData() {
      const reportParam = this.reportParam;
      const startDate = this.commonJs.convertDateTimeToStringDateTime(
        reportParam.TimeRange?.StartDateTime
      );
      const endDate = this.commonJs.convertDateTimeToStringDateTime(
        reportParam.TimeRange?.EndDateTime
      );
      this.maxios
        .get(
          `employees/statistics/invoice-timerange?startDate=${
            startDate ?? ""
          }&endDate=${endDate ?? ""}&categoryId=${this.categoryId ?? ""}`
        )
        .then((res) => {
          this.reports = res;
          this.$emitter.emit("updateFirstLoad");
        });
    },
  },
  data() {
    return {
      reports: [],
    };
  },
};
</script>
<style scoped></style>
