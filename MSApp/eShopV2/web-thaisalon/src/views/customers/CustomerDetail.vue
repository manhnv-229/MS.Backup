<template>
  <m-dialog title="Thông tin khách hàng" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Mã khách hàng"
            ref="txtCustomerCode"
            v-model="customer.CustomerCode"
            :validated="isValidated"
            :required="true"
            :isDisabled="false"
            :isFocus="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <m-input
            label="Tên khách hàng"
            ref="txtcustomerName"
            v-model="customer.FullName"
            :validated="isValidated"
            :required="true"
            :isDisabled="false"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Nhóm khách hàng"
            v-model="customer.CustomerGroupId"
            url="/customergroups"
            propText="CustomerGroupName"
            propValue="CustomerGroupId"
          ></m-combobox>
        </div>
        <div class="m-col"></div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Giới tính"
            v-model="customer.Gender"
            url="/dictionarys/gender"
            propText="Text"
            propValue="Value"
          ></m-combobox>
        </div>
        <div class="m-col">
          <label for="">Ngày sinh:</label>
          <el-date-picker
            v-model="customer.DateOfBirth"
            type="date"
            format="DD-MM-YYYY"
          />
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Số điện thoại"
            v-model="customer.Mobile"
            :isDisabled="false"
            :onlyNumberChar="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <m-input
            label="Email"
            v-model="customer.Email"
            :isDisabled="false"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <m-text-area label="Địa chỉ" v-model="customer.Address"></m-text-area>
      </div>
    </template>
    <template v-slot:footer>
      <button class="btn btn--close" @click="onClose">Hủy</button>
      <button class="btn btn--default mg-left-10" @click="onSave">Lưu</button>
    </template>
  </m-dialog>
</template>
<script>
import Enum from "@/scripts/enum";
export default {
  name: "CustomerDetail",
  emits: ["onClose"],
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
      this.maxios.get(`customers/${this.id}`).then((res) => {
        this.customer = res;
      });
    } else {
      this.maxios.get(`customers/new-code`).then((res) => {
        this.customer.CustomerCode = res;
      });
    }
  },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`customers`, this.customer).then(() => {
          this.$emit("onClose");
        });
      } else {
        this.maxios.put(`customers/${this.id}`, this.customer).then(() => {
          this.$emit("onClose");
        });
      }
    },
  },
  data() {
    return {
      customer: {},
    };
  },
};
</script>
<style scoped></style>
