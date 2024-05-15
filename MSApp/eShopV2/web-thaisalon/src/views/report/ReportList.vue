<template>
  <m-page title="Báo cáo/ Thống kê">
    <template v-slot:container>
      <m-table-layout>
        <!-- TOOLBAR -->
        <template v-slot:toolbar>
          <div class="toolbar__left">
            <div class="statistic-toolbar">
              <m-combobox
                label="Thời gian thống kê"
                url="/Dictionarys/time-range"
                propValue="Value"
                :firstSelected="true"
                v-model="timeRange"
                propText="Text"
                class="mg-left-8"
              >
              </m-combobox>
              <m-combobox
                label="Nhóm thống kê"
                url="/groupproducts"
                propValue="GroupProductId"
                propText="GroupProductName"
                class="mg-left-8"
              >
              </m-combobox>
            </div>
          </div>
          <div class="toolbar__right">
            <div class="toolbar__right-custom">
              <button class="btn btn-refresh" @click="loadData()">
                <i class="icofont-refresh"></i>
              </button>
            </div>
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->
        <template v-slot:container>
          <m-table
            ref="tbCustomers"
            :data="reports"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column type="index" label="#" width="40px"></m-column>
            <m-column prop="FullName" label="Nhân viên"></m-column>
            <m-column prop="GroupProductName" label="Nhóm SP/DV"></m-column>
            <m-column prop="TotalMoneys" label="Số tiền" align="right">
                <template #default="scope">
                   <b><span>{{commonJs.formatMoney(scope.row.TotalMoneys)}}</span></b>
                </template>
            </m-column>
          </m-table>
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
    <template v-slot:footer> </template>
  </m-page>
</template>
<script>
import Enum from "@/scripts/enum";
export default {
  name: "ReportList",
  emits: [],
  props: [],
  created() {},
  watch: {
    timeRange: function (newValue) {
      var date = new Date();
      switch (newValue) {
        case Enum.TimeRanges.Today:
          this.timeInfo = this.commonJs.getRangeDateTimeInDay(date);
          break;
        case Enum.TimeRanges.Yesterday:
          this.timeInfo = this.commonJs.getRangeDateTimeInYesterday(date);
          break;
        case Enum.TimeRanges.ThisWeek:
          this.timeInfo = this.commonJs.getRangeDateTimeWeekOfDate(date);
          break;
        case Enum.TimeRanges.ThisMonth:
          this.timeInfo = this.commonJs.getRangeDateTimeMonthOfDate(date);
          break;
        case Enum.TimeRanges.ThisQuarter:
          this.timeInfo = this.commonJs.getRangeDateTimeQuarterOfDate(date);
          break;
        case Enum.TimeRanges.ThisYear:
          this.timeInfo = this.commonJs.getRangeDateTimeYearOfDate(date);
          break;
        default:
          this.timeInfo = this.commonJs.getRangeDateTimeYearOfDate(date);
          break;
      }
      this.loadData();
    },
  },
  methods: {
    loadData() {
        var startDate = this.commonJs.convertDateTimeToStringDateTime(this.timeInfo.StartDateTime);
        var endDate = this.commonJs.convertDateTimeToStringDateTime(this.timeInfo.EndDateTime);
        this.maxios.get(`employees/statistics/invoice-timerange?startDate=${startDate}&endDate=${endDate}`)
        .then(res=>{
            this.reports = res;
        })
    },
  },
  data() {
    return {
      reports: [],
      timeRange: null,
      timeInfo: {},
    };
  },
};
</script>
<style scoped>
.statistic-toolbar {
  display: flex;
}

.toolbar__right-custom{
    height: 100%;
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
}
</style>
