<template>
    <div class="report-detail">
        <div class="report-table">
            <m-table ref="tbCustomers" :data="reports" empty-text="Không có dữ liệu" width="100%" height="100%"
                :summary-method="getSummaries" show-summary>
                <m-column type="index" label="#" width="40px"></m-column>
                <m-column prop="CustomerName" label="Khách hàng">
                    <template #default="scope">
                        <a class="inventory-item--link-reference" @click.prevent="onSelectedItem(scope.row)">
                            <span>{{ scope.row.CustomerName ?? '(Khách vãng lai)' }}</span>
                        </a>
                    </template>
                </m-column>

                <m-column prop="TotalAmountRefDetail" label="Chi tiêu" align="right" width="150px">
                    <template #default="scope">
                        <b><span>{{ commonJs.formatMoney(scope.row.TotalAmountRefDetail) }}</span></b>
                    </template>
                </m-column>
                <m-column prop="TotalProfitAmount" label="Lợi nhuận" align="right" width="150px">
                    <template #default="scope">
                        <b><span>{{ commonJs.formatMoney(scope.row.TotalProfitAmount) }}</span></b>
                    </template>
                </m-column>
                <m-column prop="TotalProfitAmount" label="Điểm tích luỹ" align="right" width="150px">
                    <template #default="scope">
                        <b><span>{{ scope.row.TotalProfitAmount }}</span></b>
                    </template>
                </m-column>
            </m-table>
        </div>
        <div class="total">
            <div class="--left"></div>
            <div class="--right report-money">
                <div>
                    <label>Doanh thu:</label> {{ commonJs.formatMoney(total?.TotalAmountRefDetail) }}
                </div>
                <div>
                    <label>Lợi nhuận:</label>
                    {{ commonJs.formatMoney(total?.TotalProfitAmount) }}
                </div>
            </div>
        </div>
    </div>
    <CustomerOrderList v-if="showCustomerOrderList" :input="itemSelected" @onClose="showCustomerOrderList = false">
    </CustomerOrderList>
</template>
<script>
import CustomerOrderList from './CustomerOrderList.vue'
export default {
    name: "RevenueReportByCustomer",
    components: { CustomerOrderList },
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
        total: function () {
            const totalInfo = {
                TotalAmount: 0,
                TotalUnitPrice: 0,
                TotalCostPrice: 0,
                TotalProfitAmount: 0,
                TotalQuantity: 0,
                TotalActualReceiveAmount: 0,
                TotalAmountRefDetail: 0
            };
            if (this.reports) {
                for (const item of this.reports) {
                    totalInfo.TotalAmount += item.TotalAmount;
                    totalInfo.TotalUnitPrice += item.TotalUnitPrice;
                    totalInfo.TotalCostPrice += item.TotalCostPrice;
                    totalInfo.TotalProfitAmount += item.TotalProfitAmount;
                    totalInfo.TotalQuantity += item.TotalQuantity;
                    totalInfo.TotalActualReceiveAmount += item.TotalActualReceiveAmount;
                    totalInfo.TotalAmountRefDetail += item.TotalAmountRefDetail;
                }
            }
            return totalInfo;
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
            const startDate = this.commonJs.formatDateTimeYYMMDDHHMMSS(
                reportParam.TimeRange?.StartDateTime
            );
            const endDate = this.commonJs.formatDateTimeYYMMDDHHMMSS(
                reportParam.TimeRange?.EndDateTime
            );
            this.maxios
                .get(
                    `reports/customers/revenue?startDate=${startDate ?? ""
                    }&endDate=${endDate ?? ""}`
                )
                .then((res) => {
                    this.reports = res;
                    this.$emitter.emit("updateFirstLoad");
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
                        sums[index] = this.commonJs.formatMoney(this.total.TotalProfitAmount);
                        break;
                    case "TotalAmount":
                        sums[index] = this.commonJs.formatMoney(this.total.TotalAmount);
                        break;
                    case "TotalActualReceiveAmount":
                        sums[index] = this.commonJs.formatMoney(this.total.TotalActualReceiveAmount);
                        break;
                    case "TotalQuantity":
                        sums[index] = this.total.TotalQuantity;
                        break;
                    case "TotalAmountRefDetail":
                        sums[index] = this.commonJs.formatMoney(this.total.TotalAmountRefDetail);
                        break;
                    default:
                        sums[index] = "";
                        break;
                }
            });
            return sums;
        },
        onSelectedItem(item) {
            this.itemSelected = item;
            this.showCustomerOrderList = true;
        }
    },
    data() {
        return {
            reports: [],
            showCustomerOrderList: false,
            itemSelected: null
        };
    },
};
</script>
<style scoped>
@import url(./report.css);
</style>
  