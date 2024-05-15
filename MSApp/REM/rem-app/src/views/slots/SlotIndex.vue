<template>
    <m-page title="Danh sách Phòng/Bàn" :rightSlot="true">
      <!-- <template v-slot:header-right> </template> -->
      <template v-slot:container>
        <div class="mtab">
          <div class="mtab__list">
            <router-link to="/slots/dashboard" class="mtab__item">
              Phòng/Bàn
            </router-link>
            <router-link to="/slots/group" class="mtab__item"
              >Nhóm Phòng/Bàn</router-link
            >
          </div>
          <div class="mtab__container">
            <router-view name="SlotRouterView"></router-view>
          </div>
        </div>
      </template>
      <template v-slot:footer> </template>
    </m-page>

  </template>
  <script>
  import debounce from "../../scripts/debounce";
  export default {
    name: "SlotIndex",
    components: {  },
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
          .get(
            `slots/paging?limit=${this.limit}&offset=${this.offset}&key=${
              key || ""
            }`,
          )
          .then((res) => {
            this.slots = res.Data;
            this.totalRecords = res.TotalRecords;
          });
      },
      onAddNew() {
        this.showDetail = true;
        this.idSelected = null;
      },
      onUpdate(slot) {
        this.idSelected = slot.SlotId;
        this.showDetail = true;
      },
      onDelete(slot) {
        this.commonJs.showConfirm(
          `Bạn có chắc chắn muốn xóa [${slot.SlotName}] không?`,
          () => {
            this.maxios.delete(`slots/${slot.SlotId}`);
            this.loadData();
          },
        );
      },
      onCloseDetail() {
        this.showDetail = false;
        this.loadData();
      },
    },
    data() {
      return {
        slots: [],
        totalRecords: 0,
        isReload: false,
        keySearch: "",
        limit: 0,
        offset: 0,
        showDetail: false,
        idSelected: null,
        reportTabActive: "SlotList",
      };
    },
  };
  </script>
  <style scoped></style>
  