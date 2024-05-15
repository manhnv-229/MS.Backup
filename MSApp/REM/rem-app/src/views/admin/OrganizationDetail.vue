<template>
  <m-page :title="organization.OrganizationName" :rightSlot="true">
    <template v-slot:header-right> </template>
    <template v-slot:container>
      <form>
        <div class="org--name">
          {{ organization.OrganizationName }}
          <div v-if="organization.IsConfirm" class="confirm-info">
            <i class="icofont-check-circled"></i>
          </div>
        </div>

        <div class="org-container">
          <div v-if="id" class="org-update">
            <div class="license-info" :class="{ 'license--limitted': !licenseDate }">
              <div class="license__icon"><i class="icofont-license"></i></div>
              <div v-if="licenseDate" class="license__content" v-html="licenseInfoHTML"></div>
              <div v-else class="license__content" v-html="licenseInfoHTML"></div>
            </div>
            <div class="function-button">
              <button v-if="!organization.IsConfirm" class="btn btn--default"
                @click.prevent="onConfirmOrganization(true)">
                <i class="icofont-check-circled"></i> Xác nhận doanh nghiệp
              </button>
              <button v-else class="btn btn--remove" @click.prevent="onConfirmOrganization(false)">
                <i class="icofont-close-circled"></i> Bỏ xác nhận
              </button>
              <button class="btn btn--success" @click.prevent="onRenewVIP">
                <i class="icofont-certificate"></i> Gia hạn VIP
              </button>
              <button v-if="licenseDate" class="btn btn--remove" @click.prevent="onCancelVIP">
                <i class="icofont-close-circled"></i> Hủy VIP
              </button>
            </div>
          </div>
          <!-- THÔNG TIN DOANH NGHIỆP -->
          <div class="sub-title">Thông tin doanh nghiệp</div>
          <div class="m-row">
            <div class="m-col" style="width: 100px">
              <m-input label="Mã" ref="txtOrganizationCode" v-model="organization.OrganizationCode" :required="true"
                :disabled="true" :isFocus="false">
              </m-input>
            </div>
            <div class="m-col"></div>
          </div>
          <div class="m-row">
            <div class="m-col flex-1">
              <m-input label="Tên doanh nghiệp" ref="txtOrganizationName" v-model="organization.OrganizationName"
                :required="true" :isDisabled="false" :isFocus="true">
              </m-input>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input label="Lĩnh vực kinh doanh (VD: Tạp hóa/ Dịch vụ giải trí)" ref="txtOrganizationCode"
                v-model="organization.Description" :isDisabled="false">
              </m-input>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input label="Slogan (khẩu hiệu)" ref="txtSlogan" v-model="organization.Slogan" :isDisabled="false">
              </m-input>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input label="Chủ doanh nghiệp/ đại lý" ref="txtFullName" v-model="organization.OwnerName"
                :isDisabled="false" :onlyNumberChar="false" :required="true">
              </m-input>
            </div>
            <div class="m-col"></div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input label="Số điện thoại" v-model="organization.PhoneNumber" ref="txtPhoneNumber" :isDisabled="false"
                :required="true" :onlyNumberChar="true">
              </m-input>
            </div>
            <div class="m-col">
              <m-input label="Email" ref="txtEmail" v-model="organization.Email" :isDisabled="false" :required="true">
              </m-input>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input label="Giấy phép hoạt động kinh doanh (nếu có)" v-model="organization.BusinessLicense"
                :isDisabled="false" :onlyNumberChar="true">
              </m-input>
            </div>
            <div class="m-col">
              <m-input label="Website" v-model="organization.Website" :isDisabled="false">
              </m-input>
            </div>
          </div>
          <div class="m-row">
            <m-text-area label="Địa chỉ" v-model="organization.Address"></m-text-area>
          </div>
        </div>
        <!-- THÔNG TIN TÀI KHOẢN -->

        <div v-if="formMode == Enum.FormMode.ADD" class="account-container">
          <div class="sub-title">Thông tin tài khoản</div>
          <div class="m-row">
            <div class="m-col" style="width: 100px">
              <m-input label="Tài khoản đăng nhập" ref="txtUserName" v-model="user.UserName" :required="true"
                :isFocus="false">
              </m-input>
            </div>
            <div class="m-col flex-1"></div>
          </div>
          <div class="m-row">
            <div class="m-col" style="width: 100px">
              <m-input label="Mật khẩu" ref="txtPassword" type="password" v-model="user.Password" :required="true">
              </m-input>
            </div>
            <div class="m-col flex-1">
              <m-input label="Xác nhận mật khẩu" type="password" ref="txtRePassword" v-model="user.RePassword"
                :required="true">
              </m-input>
            </div>
          </div>
        </div>
      </form>
    </template>
    <template v-slot:footer>
      <div class="footer__button">
        <button class="btn btn--success" @click="onClose">
          <i class="icofont-swoosh-left"></i> <span>Quay lại</span>
        </button>
        <button class="btn btn--default mg-left-10" :disabled="!isChange" @click="onSave">
          <i class="icofont-ui-check"></i> <span>Lưu</span>
        </button>
      </div>
    </template>
  </m-page>
  <RenewLicense v-if="showVIPOption" @onClose="onDoneRenew" :organization="organization"></RenewLicense>
</template>
<script>
import Enum from "../../scripts/enum";
import router from "../../router";
import RenewLicense from "./RenewLicense.vue";
export default {
  name: "OrganizationSettings",
  components: { RenewLicense },
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
    licenseDate: function () {
      var license = this.organization.MSLicense;
      if (license && license.length > 0) {
        var date = this.commonJs.formatDateTime(license[0].ExpiredDate);
        return date;
      }
      return null;
    },
    licenseInfo: function () {
      let htmlString = "";
      if (this.licenseDate) {
        htmlString = `<b>${this.organization.OrganizationName}</b> là đối tác VIP của Mạnh Software.
              Hạn đến: ${this.licenseDate}.`;
      } else {
        htmlString = `<b>${this.organization.OrganizationName}</b> chưa đăng ký thành viên VIP, một số
                          chức năng có thể bị giới hạn.<br>
                          Vui lòng liên hệ:
                          <li>Công ty Phần mềm Mạnh Software.</li>
                          <li>SĐT: <a href="tel:0961179969">0961.179.969 (Mr Mạnh)</a></li>
                          <li>Email: <a href="mail:nmanh.com@gmail.com">nmanh.com@gmail.com</a></li>
                          <li>Địa chỉ: Tòa nhà Technosoft, ngõ 15 Duy Tân, Cầu Giấy, Hà nội</li>`;
      }
      return htmlString;
    },
    isChange: function () {
      var newOrgJSON = JSON.stringify(this.organization);
      if (newOrgJSON != this.organizationOrigilJSON) {
        return true;
      } else {
        return false;
      }
    },
  },
  created() {
    if (this.id) {
      this.loadData();
    } else {
      this.maxios.get(`organizations/new-code`).then((newCode) => {
        this.organization.OrganizationCode = newCode;
      });
    }
  },
  methods: {
    loadData() {
      var orgId = this.id;
      if (orgId) {
        this.maxios.get(`organizations/${orgId}`).then((res) => {
          this.organization = res;
          this.licenseInfoHTML = this.licenseInfo;
          this.organizationOrigilJSON = JSON.stringify(res);
        });
      } else {
        router.push("/");
      }
    },
    onClose() {
      router.push(`/admin`);
    },
    onDoneRenew() {
      this.showVIPOption = false;
      this.loadData();
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.user.Organization = this.organization;
        this.user.FullName = this.organization.OwnerName;
        this.user.Email = this.organization.Email;
        this.user.PhoneNumber = this.organization.PhoneNumber;
      }
      if (this.validate()) {
        if (this.formMode == Enum.FormMode.ADD) {
          this.maxios
            .post(`accounts/register`, this.user, this.setFocusFirstInputErrors)
            .then(() => {
              router.push("/admin");
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
                      this.$refs.txtUserPhoneNumber.setInValidState(
                        true,
                        erroText
                      );
                      propErrors.push(this.$refs.txtUserPhoneNumber);
                      break;
                    case "Email":
                      this.stepRegister = 2;
                      this.$refs.txtEmail.setInValidState(true, erroText);
                      propErrors.push(this.$refs.txtEmail);
                      break;
                    case "OrganizationName":
                      this.stepRegister = 2;
                      this.$refs.txtOrganizationName.setInValidState(
                        true,
                        erroText
                      );
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
        } else {
          this.maxios
            .put(
              `organizations/${this.organization.OrganizationId}`,
              this.organization
            )
            .then(() => {
              router.push("/admin");
            });
        }
      }
    },
    validate() {
      var errorMsgs = [];
      var fieldsInValid = [];
      if (!this.commonJs.checkRequired(this.organization.OrganizationCode)) {
        let errorText = "<b>Mã doanh nghiệp</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtOrganizationCode.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtOrganizationCode);
      }
      if (!this.commonJs.checkRequired(this.organization.OrganizationName)) {
        let errorText = "<b>Tên doanh nghiệp</b> không được để trống.";
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
      if (this.formMode == Enum.FormMode.ADD) {
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
        if (!this.commonJs.checkRequired(this.user.PhoneNumber)) {
          let errorText = "<b>Số điện thoại</b> không được để trống.";
          errorMsgs.push(errorText);
          this.$refs.txtPhoneNumber.setInValidState(true, errorText);
          fieldsInValid.push(this.$refs.txtPhoneNumber);
        }
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
    onConfirmOrganization(isConfirm) {
      this.commonJs.showConfirm(
        "Khi doanh nghiệp được xác nhận, doanh nghiệp này sẽ không thể bị xóa khỏi hệ thống.",
        () => {
          this.maxios.patch(
            `organizations/${this.organization.OrganizationId}`,
            { IsConfirm: isConfirm },
            () => {
              this.loadData();
            }
          );
        }
      );
    },
    onRenewVIP() {
      this.showVIPOption = true;
    },
    onCancelVIP() {
      this.commonJs.showConfirm(
        "Khi hủy thành viên VIP, doanh nghiệp sẽ không thể sử dụng các tính năng trong hạng mục VIP.",
        () => {
          this.maxios
            .delete(
              `organizations/${this.organization.OrganizationId}/licenses`,
              this.license
            )
            .then(() => {
              this.loadData();
            });
        }
      );
    },
    setFocusFirstInputErrors() {
      if (this.propFirstNotValid) {
        this.$nextTick(function () {
          this.propFirstNotValid.setFocus();
        });
      }
    },
  },
  data() {
    return {
      user: {},
      organization: {},
      licenseInfoHTML: "",
      organizationOrigilJSON: null,
      propFirstNotValid: null,
      showVIPOption: false,
    };
  },
};
</script>
<style scoped>
.org--name{
  display: flex;
  align-items: center;
  column-gap: 10px;
  font-size: 32px;
  font-weight: 800;
}
.org-container {
  padding-bottom: 10px;
}

.sub-title {
  font-size: 16px;
  font-weight: 700;
  padding: 10px 0px;
  border-top: 1px dashed #c1c1c1;
  border-bottom: 1px dashed #c1c1c1;
  margin-bottom: 10px;
}

.footer-org {
  border-top: 1px solid #dedede;
  padding: 20px 0;
  margin-top: 20px;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  width: 100%;
  column-gap: 10px;
}

.license-info {
  border-top: 1px dashed #dedede;
  border-bottom: 1px dashed #dedede;
  padding: 20px 0;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  column-gap: 10px;
  color: green;
  box-shadow: 0 1px 5px 2px #dedede;
}

.license__icon {
  font-size: 30px;
}

.license__content {
  font-size: 11px;
}

.license--limitted {
  color: red;
}

.footer__button {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px 0;
  border-top: 1px dashed #dedede;
}

.footer__button button {
  display: flex;
  align-items: center;
  column-gap: 5px;
}

.footer__button button i {
  font-size: 24px;
}

.function-button {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  column-gap: 5px;
  row-gap: 5px;
  margin-bottom: 10px;
  flex-wrap: wrap;
}

.function-button button {
  display: flex;
  align-items: center;
  column-gap: 5px;
}

.function-button button i {
  font-size: 24px;
}

.confirm-info {
  font-size: 24px;
  color: green;
}
</style>
