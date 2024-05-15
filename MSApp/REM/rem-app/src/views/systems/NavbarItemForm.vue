<template>
  <MDialogDragable
    class=""
    :overlay="true"
    :showHeader="true"
    :showFooter="true"
    :dragable="false"
    @onClose="onClose"
  >
    <template #header>
      <div>Thông tin Navbar Item</div>
    </template>
    <template #content>
      <div>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Tiêu đề"
              :required="true"
              v-model="navbarItem.Label"
              :isFocus="true"
            ></m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Path"
              :required="true"
              v-model="navbarItem.Path"
              :isFocus="false"
            ></m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-combobox
              label="Là Sub Menu của"
              url="/navbaritems"
              propText="Label"
              propValue="NavbarItemId"
              v-model="navbarItem.ParentId"
            ></m-combobox>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Thứ tự"
              :required="true"
              :onlyNumberChar="true"
              align="right"
              v-model="navbarItem.Sort"
              :isFocus="false"
            ></m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-checkbox
              label="Sử dụng Icon Font"
              v-model="navbarItem.UseIconFont"
            ></m-checkbox>
          </div>
        </div>
        <div class="m-row" v-if="navbarItem.UseIconFont">
          <div class="m-col">
            <m-input
              label="Icon Font"
              v-model="navbarItem.IconFont"
              :isFocus="false"
              v-if="navbarItem.UseIconFont"
            ></m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Icon Class"
              v-model="navbarItem.IconCls"
              :isFocus="false"
              v-if="!navbarItem.UseIconFont"
            ></m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-checkbox
              label="Là dữ liệu hệ thống"
              v-model="navbarItem.IsSystem"
            ></m-checkbox>
          </div>
        </div>
      </div>
    </template>
    <template #footer>
      <div style="display: flex; column-gap: 4px">
        <m-button-icon
          cls="btn--close"
          iconCls="icon--close-white"
          @click="onClose"
          >Đóng</m-button-icon
        >
        <m-button-icon
          cls="btn--success"
          iconCls="icon--confirm"
          @click="onSaveNavbarItem"
          >Lưu</m-button-icon
        >
      </div>
    </template>
  </MDialogDragable>
</template>
<script>
export default {
  name: "NavbarItemForm",
  props: ["inputObject"],
  emits: ["onClose", "onSaveNavbarItemSuccess"],
  created() {
    this.navbarItem = this.inputObject;
  },
  computed: {
    formMode: function () {
      if (this.inputObject.NavbarItemId) {
        return this.Enum.FormMode.UPDATE;
      } else {
        return this.Enum.FormMode.ADD;
      }
    },
  },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onSaveNavbarItem() {
      if (this.formMode === this.Enum.FormMode.UPDATE) {
        this.navbarItem.EntityState = this.Enum.EntityState.UPDATE;
        this.$emit("onSaveNavbarItemSuccess",this.navbarItem);
        // this.maxios
        //   .put(`navbaritems/${this.navbarItem.NavbarItemId}`, this.navbarItem)
        //   .then((res) => {
        //     this.$emit("onSaveNavbarItemSuccess");
        //   });
      } else {
        this.navbarItem.EntityState = this.Enum.EntityState.ADD;
        this.$emit("onSaveNavbarItemSuccess",this.navbarItem);
        // this.maxios.post(`navbaritems`, this.navbarItem).then((res) => {
        //   this.$emit("onSaveNavbarItemSuccess");
        // });
      }
      this.$emit("onClose");
    },
  },
  data() {
    return { navbarItem: { UseIconFont: false, IsSystem: false } };
  },
};
</script>
