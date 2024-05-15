<template>
  <div class="m-row">
    <div class="m-col">
      <m-input
        label="Mã thuê bao"
        ref="txtTenantCode"
        v-model="tenantRegister.TenantCode"
        :isDisabled="false"
        :onlyNumberChar="false"
        :required="true"
      >
      </m-input>
    </div>
    <div class="m-col"></div>
  </div>
  <div class="m-row">
    <div class="m-col" style="width: 100px">
      <m-input
        label="Tài khoản đăng nhập"
        ref="txtUserName"
        v-model="userRegister.UserName"
        :validated="isValidated"
        :required="true"
        :isFocus="true"
      >
      </m-input>
    </div>
    <div class="m-col flex-1"></div>
  </div>
  <div class="m-row">
    <div class="m-col" style="width: 100px">
      <m-input
        label="Mật khẩu"
        ref="txtPassword"
        type="password"
        v-model="userRegister.Password"
        :validated="isValidated"
        :required="true"
      >
      </m-input>
    </div>
    <div class="m-col flex-1">
      <m-input
        label="Xác nhận mật khẩu"
        type="password"
        ref="txtRePassword"
        v-model="userRegister.RePassword"
        :validated="isValidated"
        :required="true"
      >
      </m-input>
    </div>
  </div>
  <div class="m-row">
    <div class="m-col">
      <m-input
        label="Họ và tên"
        ref="txtFullName"
        v-model="userRegister.FullName"
        :required="true"
        :isDisabled="false"
      >
      </m-input>
    </div>
    <div class="m-col"></div>
  </div>
  <div class="m-row">
    <div class="m-col">
      <label for="">Ngày sinh:</label>
      <el-date-picker
        v-model="userRegister.DateOfBirth"
        type="date"
        format="DD-MM-YYYY"
      />
    </div>
    <div class="m-col">
      <m-combobox
        label="Giới tính"
        v-model="userRegister.Gender"
        url="/dictionarys/gender"
        propText="Text"
        propValue="Value"
      ></m-combobox>
    </div>
  </div>
  <div class="m-row">
    <div class="m-col">
      <m-input
        label="Số điện thoại"
        ref="txtUserPhoneNumber"
        v-model="userRegister.PhoneNumber"
        :isDisabled="false"
        :required="true"
        :onlyNumberChar="true"
      >
      </m-input>
    </div>
    <div class="m-col">
      <m-input
        label="Email"
        ref="txtUserEmail"
        v-model="userRegister.Email"
        placeholder="VD: nvmanh@nmanh.com"
        :isDisabled="false"
        :required="true"
      >
      </m-input>
    </div>
  </div>
</template>
<script>
export default {
  name: "TenantUserRegisterForm",
  props: [],
  emits: [],
  inject: ["userRegister","tenantRegister"],
  methods: {
    onValidate() {
      var errorMsgs = [];
      var fieldsInValid = [];
      if (!this.commonJs.checkRequired(this.tenantRegister.TenantCode)) {
        let errorText = "<b>Mã thuê bao</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtTenantCode.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtTenantCode);
      }
      if (!this.commonJs.checkRequired(this.userRegister.UserName)) {
        let errorText = "<b>Tài khoản</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtUserName.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtUserName);
      }
      if (!this.commonJs.checkRequired(this.userRegister.Password)) {
        let errorText = "<b>Mật khẩu</b> không được để trống.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtPassword);
        this.$refs.txtPassword.setInValidState(true, errorText);
      } else {
        if (this.userRegister.Password != this.userRegister.RePassword) {
          let errorText = "<b>Mật khẩu xác nhận</b> không khớp.";
          errorMsgs.push(errorText);
          fieldsInValid.push(this.$refs.txtRePassword);
          this.$refs.txtRePassword.setInValidState(true, errorText);
        }
      }
      if (!this.commonJs.checkRequired(this.userRegister.FullName)) {
        let errorText = "<b>Họ và tên</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtFullName.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtFullName);
      }
      if (!this.commonJs.checkRequired(this.userRegister.Email)) {
        let errorText = "<b>Email</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtUserEmail.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtUserEmail);
      }
      if (!this.commonJs.checkEmailFormat(this.userRegister.Email)) {
        let errorText = "<b>Email</b> không đúng định dạng.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtUserEmail);
        this.$refs.txtUserEmail.setInValidState(true, errorText);
      }
      if (!this.commonJs.checkRequired(this.userRegister.PhoneNumber)) {
        let errorText = "<b>Số điện thoại</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtUserPhoneNumber.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtUserPhoneNumber);
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
