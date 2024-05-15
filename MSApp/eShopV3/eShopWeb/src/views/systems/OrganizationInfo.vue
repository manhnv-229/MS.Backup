<template>
  <m-dialog title="Thông tin cửa hàng" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Mã cửa hàng"
            ref="txtorganizationCode"
            v-model="organization.OrganizationCode"
            :validated="isValidated"
            :required="true"
            :isDisabled="false"
            :isFocus="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <m-input
            label="Tên cửa hàng"
            ref="txtorganizationName"
            v-model="organization.OrganizationName"
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
            label="Nhóm cửa hàng"
            v-model="organization.OrganizationId"
            url="/organizationgroups"
            propText="organizationGroupName"
            propValue="organizationGroupId"
          ></m-combobox>
        </div>
        <div class="m-col"></div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Giới tính"
            v-model="organization.Gender"
            url="/dictionarys/gender"
            propText="Text"
            propValue="Value"
          ></m-combobox>
        </div>
        <div class="m-col">
          <label for="">Ngày thành lập:</label>
          <el-date-picker
            v-model="organization.DateOfBirth"
            type="date"
            format="DD-MM-YYYY"
          />
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Số điện thoại"
            v-model="organization.PhoneNumber"
            :isDisabled="false"
            :onlyNumberChar="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <m-input
            label="Email"
            v-model="organization.Email"
            :isDisabled="false"
            :onlyNumberChar="true"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <m-text-area label="Địa chỉ" v-model="organization.Address"></m-text-area>
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
  name: "OrganizationInfo",
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
      this.maxios.get(`organizations/${this.id}`).then((res) => {
        this.organization = res;
      });
    } else {
      this.maxios.get(`organizations/new-code`).then((res) => {
        this.organization.organizationCode = res;
      });
    }
  },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`organizations`, this.organization).then(() => {
          this.$emit("onClose");
        });
      } else {
        this.maxios.put(`organizations/${this.id}`, this.organization).then(() => {
          this.$emit("onClose");
        });
      }
    },
  },
  data() {
    return {
      organization: {},
    };
  },
};
</script>
<style scoped></style>
