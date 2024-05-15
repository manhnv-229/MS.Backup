<template>
  <m-dialog title="Thông tin nhóm khách hàng" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Tên nhóm"
            ref="txtCustomerGroupName"
            v-model="customergroup.CustomerGroupName"
            :validated="isValidated"
            :required="true"
            :isDisabled="false"
            :isFocus="true"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <m-text-area
          label="Mô tả"
          v-model="customergroup.Description"
        ></m-text-area>
      </div>
    </template>
    <template v-slot:footer>
      <button class="btn btn--close" @click="onClose">Hủy</button>
      <button class="btn btn--default mg-left-10" @click="onSave">Lưu</button>
    </template>
  </m-dialog>
</template>
<script>
import Enum from "../../scripts/enum";
export default {
  name: "customergroupDetail",
  emits: ["onClose", "onSaveSuccess"],
  props: ["id"],
  computed: {
    formMode: function () {
      if (this.id) {
        return Enum.FormMode.UPDATE;
      } else {
        return Enum.FormMode.ADD;
      }
    },
  },
  created() {
    if (this.id) {
      this.maxios.get(`customergroups/${this.id}`).then((res) => {
        this.customergroup = res;
      });
    }
  },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`customergroups`, this.customergroup).then(() => {
          this.$emit("onClose");
          this.$emit("onSaveSuccess", this.customergroup);
        });
      } else {
        this.maxios
          .put(`customergroups/${this.id}`, this.customergroup)
          .then(() => {
            this.$emit("onClose");
            this.$emit("onSaveSuccess", this.customergroup);
          });
      }
    },
  },
  data() {
    return {
      customergroup: {},
    };
  },
};
</script>
<style scoped></style>
