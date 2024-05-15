<template>
  <m-page title="Danh mục sản phẩm" :rightSlot="true">
    <template v-slot:header-right>
      <button class="btn btn--default btn--refresh" @click="onAddNew">
        <i class="icofont-ui-add"></i> Thêm sản phẩm
      </button>
    </template>
    <template v-slot:container>
      <m-table-layout>
        <!-- TOOLBAR -->
        <template v-slot:toolbar>
          <div class="toolbar__left">
            <m-input-search
              width="300px"
              placeholder="Tìm kiếm"
              v-model="keySearch"
            ></m-input-search>
          </div>
          <div class="toolbar__right">
            <button class="btn btn-refresh" @click="loadData()">
              <i class="icofont-refresh"></i>
            </button>
          </div>
        </template>
        <!-- BẢNG DỮ LIỆU -->
        <template v-slot:container>
          <m-table
            ref="tbProducts"
            :data="products"
            empty-text="Không có dữ liệu"
            width="100%"
            height="100%"
          >
            <m-column prop="ProductCode" label="#" width="100px"></m-column>
            <m-column prop="ProductName" label="Tên dịch vụ/sản phẩm"></m-column>
            <m-column label="Đơn giá" align="right">
              <template #default="scope">
                <div>{{ commonJs.formatMoney(scope.row.UnitPrice) }}</div>
              </template>
            </m-column>
            <m-column width="50" fixed="right">
              <template #default="scope">
                <div class="button-column">
                  <button
                    class="btn--table-mini --color-edit"
                    :title="scope.row.ProductName"
                    @click="onUpdate(scope.row)"
                  >
                    <i class="icofont-ui-edit"></i>
                  </button>
                  <button
                    class="btn--table-mini --color-red"
                    :title="scope.row.ProductName"
                    @click="onDelete(scope.row)"
                  >
                    <i class="icofont-ui-delete"></i>
                  </button>
                </div>
              </template>
            </m-column>
          </m-table>
        </template>
        <!-- PHÂN TRANG -->
        <template v-slot:footer>
          <m-paging
            entityName="sản phẩm/dịch vụ"
            :totalRecords="totalRecords"
            :pageSizeData="[10, 20, 50, 100, 200]"
            v-model:limit="limit"
            v-model:offset="offset"
            @onChangePagesize="loadData"
            @onPrevPage="loadData"
            @onNextPage="loadData"
            @onInitPaging="onInitPaging"
          ></m-paging>
        </template>
      </m-table-layout>
    </template>
    <template v-slot:footer> </template>
  </m-page>
  <router-view name="ProductDetailView"></router-view>
</template>
<script>
import router from "@/router";
import debounce from "@/scripts/debounce";
export default {
  name: "ProductList",
  emits: [],
  props: [],
  created() {},
  watch: {
    keySearch: debounce(function () {
      this.loadData();
    }, 500),
  },
  methods: {
    onInitPaging(startIndex, endIndex, pageSize) {
      this.limit = pageSize;
      this.offset = startIndex - 1;
      this.loadData();
    },
    loadData() {
      var key = this.keySearch;
      this.maxios
        .get(`products/paging?limit=${this.limit}&offset=${this.offset}&key=${key || ""}`)
        .then((res) => {
          this.products = res.Data;
          this.totalRecords = res.TotalRecords;
        });
    },
    onAddNew() {
      router.push(`products/create`);
    },
    onUpdate(product) {
      router.push(`products/${product.ProductId}`);
      console.log(product);
    },
    onDelete(product) {
      this.commonJs.showConfirm(
        `Bạn có chắc chắn muốn xóa [${product.ProductName}] không?`,
        () => {
          this.maxios.delete(`products/${product.ProductId}`);
          this.loadData();
        }
      );
    },
  },
  data() {
    return {
      products: [],
      totalRecords: 0,
      isReload: false,
      keySearch: "",
      limit: 0,
      offset: 0,
    };
  },
};
</script>
<style scoped></style>
