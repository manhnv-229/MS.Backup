<template>
    <div class="m-row">
      <div class="m-col" style="width: 100px">
        <m-input
          label="Mã cửa hàng"
          ref="txtOrganizationCode"
          v-model="organizationRegister.OrganizationCode"
          :validated="isValidated"
          :required="true"
          :disabled="false"
          :isFocus="false"
        >
        </m-input>
      </div>
      <div class="m-col flex-1"></div>
    </div>
    <div class="m-row">
      <div class="m-col flex-1">
        <m-input
          label="Tên cửa hàng"
          ref="txtOrganizationName"
          v-model="organizationRegister.OrganizationName"
          :validated="isValidated"
          :required="true"
          :isFocus="true"
        >
        </m-input>
      </div>
    </div>
    <div class="m-row">
      <div class="m-col">
        <m-input
          label="Lĩnh vực kinh doanh (VD: Tạp hóa/ Dịch vụ giải trí)"
          ref="txtDescription"
          v-model="organizationRegister.Description"
          :validated="isValidated"
          :isDisabled="false"
        >
        </m-input>
      </div>
    </div>
    <div class="m-row">
      <div class="m-col">
        <m-input
          label="Slogan:"
          ref="txtSlogan"
          v-model="organizationRegister.Slogan"
          :validated="isValidated"
          :isDisabled="false"
        >
        </m-input>
      </div>
    </div>
    <div class="m-row">
      <div class="m-col">
        <m-input
          label="Chủ cửa hàng/ đại lý"
          ref="txtOwnerName"
          v-model="organizationRegister.OwnerName"
          :isDisabled="false"
          :onlyNumberChar="false"
        >
        </m-input>
      </div>
      <div class="m-col"></div>
    </div>
    <div class="m-row">
      <div class="m-col">
        <m-input
          label="Số điện thoại"
          ref="txtPhoneNumber"
          v-model="organizationRegister.PhoneNumber"
          :isDisabled="false"
          :required="true"
          :onlyNumberChar="true"
        >
        </m-input>
      </div>
      <div class="m-col">
        <m-input
          label="Email"
          ref="txtEmail"
          v-model="organizationRegister.Email"
          placeholder="VD: nvmanh@nmanh.com"
          :isDisabled="false"
          :required="true"
        >
        </m-input>
      </div>
    </div>
    <div class="m-row">
      <div class="m-col">
        <m-input
          label="Giấy phép hoạt động kinh doanh (nếu có)"
          ref="txtBusinessLicense"
          v-model="organizationRegister.BusinessLicense"
          :isDisabled="false"
          :onlyNumberChar="true"
        >
        </m-input>
      </div>
    </div>
    <div class="m-row">
      <m-text-area
        label="Địa chỉ"
        ref="txtAddress"
        v-model="organizationRegister.Address"
      ></m-text-area>
    </div>
</template>
<script>
export default {
  name: "TenantOrganizationRegisterForm",
  props: [],
  emits: [],
  inject: ["organizationRegister"],
  methods: {
    setFirstFocus() {
      this.$nextTick(function () {
        this.$refs.txtOrganizationName.setFocus();
      });
    },
    onValidate() {
      var errorMsgs = [];
      var fieldsInValid = [];
      if (
        !this.commonJs.checkRequired(this.organizationRegister.OrganizationCode)
      ) {
        let errorText = "<b>Mã cửa hàng/ đại lý</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtOrganizationCode.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtOrganizationCode);
      }
      if (
        !this.commonJs.checkRequired(this.organizationRegister.OrganizationName)
      ) {
        let errorText = "<b>Tên cửa hàng/ đại lý</b> không được để trống.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtOrganizationName);
        this.$refs.txtOrganizationName.setInValidState(true, errorText);
      }
      if (!this.commonJs.checkRequired(this.organizationRegister.PhoneNumber)) {
        let errorText = "<b>Số điện thoại</b> không được để trống.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtPhoneNumber);
        this.$refs.txtPhoneNumber.setInValidState(true, errorText);
      }
      if (!this.commonJs.checkRequired(this.organizationRegister.Email)) {
        let errorText = "<b>Email</b> không được để trống.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtEmail);
        this.$refs.txtEmail.setInValidState(true, errorText);
      }
      if (!this.commonJs.checkEmailFormat(this.organizationRegister.Email)) {
        let errorText = "<b>Email</b> không đúng định dạng.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtEmail);
        this.$refs.txtEmail.setInValidState(true, errorText);
      }
      if (errorMsgs.length > 0) {
        this.commonJs.showMessenger({
          title: "Dữ liệu không hợp lệ",
          msg: errorMsgs,
          type: this.Enum.MsgType.Error,
          confirm: () => {
            if (fieldsInValid.length > 0) {
              fieldsInValid[0].setFocus();
            }
          },
        });

        return false;
      }
      return true;
    },
  },
  data() {
    return { isValidated: false };
  },
};
</script>
