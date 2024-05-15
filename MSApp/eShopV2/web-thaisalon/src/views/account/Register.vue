<template>
  <m-page title="Đăng ký">
    <template v-slot:header-right></template>
    <template v-slot:container>
      <!-- BƯỚC 1 - NHẬP THÔNG TIN CÁ NHÂN -->
      <div v-show="stepRegister == 1" id="step-1" class="user-info">
        <div class="m-row">
          <div class="m-col" style="width: 100px">
            <m-input
              label="Tài khoản đăng nhập"
              ref="txtUserName"
              v-model="user.UserName"
              :validated="isValidated"
              :required="true"
              :isFocus="false"
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
              v-model="user.Password"
              :validated="isValidated"
              :required="true"
              :isFocus="true"
            >
            </m-input>
          </div>
          <div class="m-col flex-1">
            <m-input
              label="Xác nhận mật khẩu"
              type="password"
              ref="txtRePassword"
              v-model="user.RePassword"
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
              label="Họ và tên"
              ref="txtFullName"
              v-model="user.FullName"
              :required="true"
              :isDisabled="false"
            >
            </m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <label for="">Ngày sinh:</label>
            <el-date-picker v-model="user.DateOfBirth" type="date" format="DD-MM-YYYY" />
          </div>
          <div class="m-col">
            <m-combobox
              label="Giới tính"
              v-model="user.Gender"
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
              v-model="user.PhoneNumber"
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
              v-model="user.Email"
              placeholder="VD: nvmanh@nmanh.com"
              :isDisabled="false"
              :required="true"
            >
            </m-input>
          </div>
        </div>
      </div>
      <!-- BƯỚC 2 - NHẬP THÔNG TIN ĐƠN VỊ -->
      <div v-show="stepRegister == 2" id="step-2" class="organization-info">
        <div class="m-row">
          <div class="m-col" style="width: 100px">
            <m-input
              label="Mã cửa hàng"
              ref="txtOrganizationCode"
              v-model="organization.OrganizationCode"
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
              v-model="organization.OrganizationName"
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
              v-model="organization.Description"
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
              v-model="organization.Slogan"
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
              v-model="organization.OwnerName"
              :isDisabled="false"
              :onlyNumberChar="true"
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
              v-model="organization.PhoneNumber"
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
              v-model="organization.Email"
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
              v-model="organization.BusinessLicense"
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
            v-model="organization.Address"
          ></m-text-area>
        </div>
      </div>

      <!-- BƯỚC 3 - XÁC NHẬN THÔNG TIN ĐĂNG KÝ -->
      <div v-if="stepRegister == 3" id="step-3" class="step-3">
        <div class="license-info">
          <div class="license__icon"><i class="icofont-license"></i></div>
          <div class="license__content">
            <div>
              Cám ơn <b>{{ user.FullName }}</b
              >!
            </div>
            <div>
              Bạn đã đăng ký thành công <b>{{ organization.OrganizationName }}</b> và được
              tặng 15 ngày dùng thử tính năng VIP của ứng dụng. <br />
              <i class="icofont-hand-right"></i> Vui lòng nhập mã xác nhận được gửi đến
              Email <b>{{ user.Email }}</b> để kích hoạt tài khoản. <br />
              <i class="icofont-hand-right"></i> Tính năng VIP sẽ hoạt động ngay sau khi
              bạn xác nhận địa chỉ Email.
            </div>
            <div class="confirm-box">
              <div class="confirm__form">
                <m-input
                  label="Mã xác nhận"
                  type="text"
                  v-model="tokenConfirm"
                  ref="txtConfirmToken"
                ></m-input>
                <m-button
                  id="btn-confirm"
                  text="Xác nhận"
                  @click="confirmAccount"
                ></m-button>
              </div>
            </div>
            <div>
              Bạn có thể đăng ký thành viên VIP hoặc gia hạn VIP sau thời gian dùng thử
              chỉ với 2000đ/ngày.
            </div>
          </div>
        </div>
        <div class="contant-info">
          Mọi vấn đề cần hỗ trợ, xin vui lòng liên hệ:
          <div class="author-name">Mạnh Software</div>
          <li>
            <i class="icofont-mobile-phone"></i>:
            <a href="tel:0961179969">0961179969</a>
          </li>
          <li>
            <i class="icofont-ui-email"></i>:
            <a href="email:https://nmanh.com">manhnv229@gmail.com</a>
          </li>
          <li>
            <i class="icofont-web"></i>:
            <a href="web:https://nmanh.com">https://www.nmanh.com</a>
          </li>
        </div>
      </div>
    </template>
    <template v-slot:footer>
      <!-- CÁC BƯỚC THỰC HIỆN -->
      <div class="step-register">
        <el-steps :active="stepRegister" align-center>
          <el-step title="Bước 1" description="Tài khoản" />
          <el-step title="Bước 2" description="Cửa hàng" />
          <el-step title="Bước 3" description="Hoàn thành" />
        </el-steps>
      </div>
      <div class="footer-org">
        <div class="footer__button">
          <button
            v-if="stepRegister == 2"
            class="btn btn--default mg-left-10 btn-step"
            @click="onPrevStep"
          >
            <span class="step-icon"><i class="icofont-swoosh-left"></i></span>
            <span>Quay lại</span>
          </button>
        </div>
        <div class="footer__button">
          <!-- <button class="btn btn--close" @click="onClose">Hủy</button> -->
          <button
            v-if="stepRegister == 1"
            class="btn btn--default mg-left-10 btn-step"
            @click="onNextStep1"
          >
            <span>Bước tiếp theo</span>
            <span class="step-icon"><i class="icofont-swoosh-right"></i></span>
          </button>
          <button
            v-if="stepRegister == 2"
            class="btn btn--default mg-left-10 btn-step"
            @click="onNextStep2"
          >
            <span>Hoàn thành</span>
            <span class="step-icon"><i class="icofont-swoosh-right"></i></span>
          </button>
          <button
            v-if="stepRegister == 3"
            class="btn btn--default mg-left-10 btn-step"
            @click="onLogin"
          >
            <span>Đăng nhập chương trình</span>
            <span class="step-icon"><i class="icofont-login"></i></span>
          </button>
        </div>
      </div>
    </template>
  </m-page>
</template>
<script>
import router from "@/router";
import Enum from "@/scripts/enum";
export default {
  name: "OrganizationRegister",
  emits: ["onClose"],
  props: ["id"],
  computed: {
    titleByStep: function () {
      if (this.stepRegister == 1) {
        return "Đăng ký tài khoản";
      } else if (this.stepRegister == 2) {
        return "Đăng ký cửa hàng";
      } else {
        return "Hoàn thành đăng ký";
      }
    },
  },
  created() {
    // Lấy mã cửa hàng mới:
    this.maxios.get("organizations/new-code").then((res) => {
      this.organization.OrganizationCode = res;
    });
  },
  /* eslint-disable */
  methods: {
    confirmAccount() {
      var userConfirmToken = {
        UserName: this.user.UserName,
        TokenCode: this.tokenConfirm,
        Email: this.user.Email,
      };
      this.maxios
        .post("accounts/confirm", userConfirmToken, this.setFocusInputToken)
        .then(() => {
          this.isConfirmed = true;
          var msg = `Chúc mừng! <br> Bạn đã xác nhận thành công và nhận 15 ngày sử dụng tính năng VIP.`;
          this.commonJs.showMessenger({
            title: "Xác nhận thành công",
            msg: msg,
            type: Enum.MsgType.Info,
            confirm: () => {
              this.commonJs.login(this.user.UserName, this.user.Password);
            },
          });
        });
    },
    onValidateRequiredInput(isValid, input) {
      console.log(isValid, input);
    },
    onClose() {
      router.push("/");
    },
    onSave() {
      var isValid = this.validateStepTwo();
      if (isValid) {
        this.user.Organization = this.organization;
        this.maxios
          .post(`accounts/register`, this.user, this.setFocusFirstInputErrors)
          .then(() => {
            this.stepRegister = 3;
          })
          .catch((res) => {
            console.log(res);
            const statusCode = res.status;
            if (statusCode == 400) {
              let errors = res.errors;
              let props = Object.keys(errors);
              var propErrors = [];
              for (const prop of props) {
                var erroText = errors[prop];
                switch (prop) {
                  case "UserName":
                    this.stepRegister = 1;
                    this.$refs.txtUserName.setInValidState(true, erroText);
                    propErrors.push(this.$refs.txtUserName);
                    break;
                  case "Password":
                    this.stepRegister = 1;
                    this.$refs.txtPassword.setInValidState(true, erroText);
                    propErrors.push(this.$refs.txtPassword);
                    break;
                  case "RePassword":
                    this.stepRegister = 1;
                    this.$refs.txtRePassword.setInValidState(true, erroText);
                    propErrors.push(this.$refs.txtRePassword);
                    break;
                  case "FullName":
                    this.stepRegister = 1;
                    this.$refs.txtFullName.setInValidState(true, erroText);
                    propErrors.push(this.$refs.txtFullName);
                    break;
                  case "Mobile":
                    this.stepRegister = 1;
                    this.$refs.txtUserPhoneNumber.setInValidState(true, erroText);
                    propErrors.push(this.$refs.txtUserPhoneNumber);
                    break;
                  case "Email":
                    this.stepRegister = 2;
                    this.$refs.txtEmail.setInValidState(true, erroText);
                    propErrors.push(this.$refs.txtEmail);
                    break;
                  case "OrganizationName":
                    this.stepRegister = 2;
                    this.$refs.txtOrganizationName.setInValidState(true, erroText);
                    propErrors.push(this.$refs.txtOrganizationName);
                    break;
                  default:
                    break;
                }
              }
              if (propErrors.length > 0) {
                this.propFirstNotValid = propErrors[0];
              }
            }
          });
      }
    },
    validateStepTwo() {
      var errorMsgs = [];
      var fieldsInValid = [];
      if (!this.commonJs.checkRequired(this.organization.OrganizationCode)) {
        let errorText = "<b>Mã cửa hàng/ đại lý</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtOrganizationCode.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtOrganizationCode);
      }
      if (!this.commonJs.checkRequired(this.organization.OrganizationName)) {
        let errorText = "<b>Tên cửa hàng/ đại lý</b> không được để trống.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtOrganizationName);
        this.$refs.txtOrganizationName.setInValidState(true, errorText);
      }
      if (!this.commonJs.checkRequired(this.organization.PhoneNumber)) {
        let errorText = "<b>Số điện thoại</b> không được để trống.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtPhoneNumber);
        this.$refs.txtPhoneNumber.setInValidState(true, errorText);
      }
      if (!this.commonJs.checkRequired(this.organization.Email)) {
        let errorText = "<b>Email</b> không được để trống.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtEmail);
        this.$refs.txtEmail.setInValidState(true, errorText);
      }
      if (!this.commonJs.checkEmailFormat(this.organization.Email)) {
        let errorText = "<b>Email</b> không đúng định dạng.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtEmail);
        this.$refs.txtEmail.setInValidState(true, errorText);
      }
      if (errorMsgs.length > 0) {
        this.commonJs.showMessenger({
          title: "Dữ liệu không hợp lệ",
          msg: errorMsgs,
          type: Enum.MsgType.Error,
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
    validateStepOne() {
      var errorMsgs = [];
      var fieldsInValid = [];
      if (!this.commonJs.checkRequired(this.user.UserName)) {
        let errorText = "<b>Tài khoản</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtUserName.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtUserName);
      }
      if (!this.commonJs.checkRequired(this.user.Password)) {
        let errorText = "<b>Mật khẩu</b> không được để trống.";
        errorMsgs.push(errorText);
        fieldsInValid.push(this.$refs.txtPassword);
        this.$refs.txtPassword.setInValidState(true, errorText);
      } else {
        if (this.user.Password != this.user.RePassword) {
          let errorText = "<b>Mật khẩu xác nhận</b> không khớp.";
          errorMsgs.push(errorText);
          fieldsInValid.push(this.$refs.txtRePassword);
          this.$refs.txtRePassword.setInValidState(true, errorText);
        }
      }
      if (!this.commonJs.checkRequired(this.user.FullName)) {
        let errorText = "<b>Họ và tên</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtFullName.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtFullName);
      }
      if (!this.commonJs.checkRequired(this.user.Email)) {
        let errorText = "<b>Email</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtUserEmail.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtUserEmail);
      }
      if (!this.commonJs.checkRequired(this.user.PhoneNumber)) {
        let errorText = "<b>Số điện thoại</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtUserPhoneNumber.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtUserPhoneNumber);
      }
      if (errorMsgs.length > 0) {
        this.commonJs.showMessenger({
          title: "Dữ liệu không hợp lệ",
          msg: errorMsgs,
          type: Enum.MsgType.Error,
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
    onPrevStep() {
      this.stepRegister -= 1;
    },
    onNextStep1() {
      var stepOneValid = this.validateStepOne();
      if (stepOneValid) {
        this.organization.Email = this.user.Email;
        this.organization.PhoneNumber = this.user.PhoneNumber;
        this.organization.OwnerName = this.user.FullName;
        this.stepRegister = 2;
        this.$nextTick(function () {
          this.$refs.txtOrganizationName.setFocus();
        });
      }
    },
    onNextStep2() {
      this.onSave();
    },
    onLogin() {
      this.commonJs.login(this.user.UserName, this.user.Password);
    },
    setFocusFirstInputErrors() {
      if (this.propFirstNotValid) {
        this.$nextTick(function () {
          this.propFirstNotValid.setFocus();
        });
      }
    },
    setFocusInputToken() {
      this.$refs.txtConfirmToken.setFocus();
    },
  },
  data() {
    return {
      registerTitle: "Đăng ký tài khoản",
      organization: {},
      user: {},
      isValidated: false,
      stepRegister: 1,
      propFirstNotValid: null,
      tokenConfirm: "",
      isConfirmed: false,
    };
  },
};
</script>
<style scoped>
.confirm-box {
  margin: 10px;
}

.confirm__form {
  max-width: 350px;
  display: flex;
  flex-direction: row;
  align-items: flex-end;
  justify-content: flex-start;
  column-gap: 10px;
}

#btn-confirm {
  width: 100px;
  flex-shrink: 0;
  flex-grow: 0;
  flex-basis: 100px;
}

.footer-org {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  column-gap: 10px;
  margin-bottom: 10px;
}

.license-info {
  display: flex;
  align-items: center;
  column-gap: 10px;
  color: green;
  border: 1px dashed green;
  padding: 5px;
}
.license__icon {
  font-size: 30px;
}

#step-3 {
  display: flex;
  flex-direction: column;
  row-gap: 10px;
}

.license__content {
  max-width: 600px;
}
.license--limitted {
  color: red;
}
.step-icon {
  font-size: 24px;
}

.btn-step {
  display: flex;
  align-items: center;
}

.author-name {
  font-weight: 700;
}

.step-register {
  margin-bottom: 10px;
}
</style>
