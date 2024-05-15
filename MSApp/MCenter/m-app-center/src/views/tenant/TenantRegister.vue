<template>
  <m-page title="Đăng ký">
    <template v-slot:header-right></template>
    <template v-slot:container>
      <!-- BƯỚC 1 - NHẬP THÔNG TIN CÁ NHÂN -->
      <div v-show="stepRegister == 0" id="step-1" class="user-info">
        <TenantUserRegisterForm ref="frmTenantUser"></TenantUserRegisterForm>
      </div>
      <!-- BƯỚC 2 - NHẬP THÔNG TIN ĐƠN VỊ -->
      <div v-show="stepRegister == 1" id="step-2" class="organization-info">
        <TenantOrganizationRegisterForm
          ref="frmTenantOrg"
        ></TenantOrganizationRegisterForm>
      </div>

      <!-- BƯỚC 3 - THÔNG TIN ỨNG DỤNG -->
      <div v-show="stepRegister == 2" id="step-3" class="step-3">
        <TenantCatalogRegisterForm
          ref="frmTenantCatalog"
        ></TenantCatalogRegisterForm>
      </div>

      <!-- BƯỚC 4 - XÁC NHẬN THÔNG TIN ĐĂNG KÝ -->
      <div v-if="stepRegister == 3" id="step-4" class="step-4">
        <RegisterProgressStep
          @onFireErrorRegister="onFireErrorRegisterProgress"
        ></RegisterProgressStep>
      </div>
      <div v-if="stepRegister == 4" class="step-4" id="step-5">
        <div class="license-info">
          <div class="license__icon"><i class="icofont-license"></i></div>
          <div class="license__content">
            <div>
              Đã hoàn thành đăng ký
              <b>{{ organization.OrganizationName }}</b
              ><br />Khách hàng đã có thể trải nghiệm miễn phí tất cả các tính
              năng của ứng dụng trong 30 ngày.
            </div>
            <div class="tenant-info">
              <div class="register-item">
                <div class="register__label">Tên cửa hàng:</div>
                <div class="register__text">
                  {{ organization.OrganizationName }}
                </div>
              </div>
              <div class="register-item">
                <div class="register__label">Số điện thoại:</div>
                <div class="register__text">
                  {{ organization.PhoneNumber }}
                </div>
              </div>
              <div class="register-item">
                <div class="register__label">Email:</div>
                <div class="register__text">
                  {{ organization.Email }}
                </div>
              </div>
              <div class="register-item">
                <div class="register__label">Tên miền:</div>
                <div class="register__text">
                  <a
                    :href="`https://${catalog.SubDomain}.${catalog.RootDomain}`"
                    target="_blank"
                    >{{ catalog.SubDomain }}.{{ catalog.RootDomain }}</a
                  >
                </div>
              </div>
              <div class="register-item">
                <div class="register__label">Tài khoản:</div>
                <div class="register__text">{{ user.UserName }}</div>
              </div>
              <div class="register-item">
                <div class="register__label">Mật khẩu:</div>
                <div class="register__text">
                  <MPasswordShow :input="user.Password"></MPasswordShow>
                </div>
              </div>
              <div class="register-item">
                <div class="register__label">Trạng thái ứng dụng:</div>
                <div v-if="appReady" class="register__text app--ready">
                  Sẵn sàng
                </div>
                <div v-else class="register__text app--restarting">
                  Đang khởi động...
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
    <template v-slot:footer>
      <!-- CÁC BƯỚC THỰC HIỆN -->
      <div class="step-register">
        <el-steps :active="stepRegister" align-center>
          <el-step title="Bước 1" description="Tài khoản" />
          <el-step title="Bước 2" description="Cửa hàng" />
          <el-step title="Bước 3" description="Thiết lập hệ thống" />
          <el-step title="Bước 4" description="Hoàn thành" />
        </el-steps>
      </div>
      <div class="footer-org">
        <router-link to="/tenants" class="link--back-login"
          >Quay lại trang chính</router-link
        >
        <div class="footer__button">
          <button
            v-if="stepRegister == 1 || stepRegister == 2"
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
            v-if="stepRegister == 0"
            class="btn btn--default mg-left-10 btn-step"
            @click="onNextStep1"
          >
            <span>Bước tiếp theo</span>
            <span class="step-icon"><i class="icofont-swoosh-right"></i></span>
          </button>
          <button
            v-if="stepRegister == 1"
            class="btn btn--default mg-left-10 btn-step"
            @click="onNextStep2"
          >
            <span>Bước tiếp theo</span>
            <span class="step-icon"><i class="icofont-swoosh-right"></i></span>
          </button>
          <button
            v-if="stepRegister == 2"
            class="btn btn--default mg-left-10 btn-step"
            @click="onNextStep3"
          >
            <span>Đăng ký</span>
            <span class="step-icon"><i class="icofont-swoosh-right"></i></span>
          </button>
          <button
            v-if="stepRegister >= 3"
            class="btn btn--default mg-left-10 btn-step"
            :disabled="stepRegister < 4 && !hasErrorProgresRegister"
            @click="onFinish"
          >
            <span class="step-icon"><i class="icofont-check"></i></span>
            <span>Hoàn thành</span>
          </button>
        </div>
      </div>
    </template>
  </m-page>
  <RegisterConfirmPopup
    v-if="showConfirmRegister"
    @onClose="showConfirmRegister = false"
    @onConfirm="onConfirmRegister"
  ></RegisterConfirmPopup>
</template>
<script>
import { computed } from "vue";
import router from "../../router";
import Enum from "../../scripts/enum";
import RegisterProgressStep from "./RegisterProgressStep.vue";
import RegisterConfirmPopup from "./RegisterInfoConfirmPopup.vue";
import MPasswordShow from "../../components/base/MPasswordShow.vue";
import TenantCatalogRegisterForm from "./TenantCatalogRegisterForm.vue";
import TenantOrganizationRegisterForm from "./TenantOrganizationRegisterForm.vue";
import TenantUserRegisterForm from "./TenantUserRegisterForm.vue";
export default {
  name: "TenantRegister",
  emits: ["onClose"],
  inject: ["appDomain", "bazaDbServerDefault"],
  props: ["id"],
  provide() {
    return {
      tenantRegister: computed(() => this.tenant),
      organizationRegister: computed(() => this.organization),
      userRegister: computed(() => this.user),
      catalogRegister: computed(() => this.catalog),
      tenantDomains: computed(() => this.tenantDomains),
    };
  },
  components: {
    RegisterProgressStep,
    RegisterConfirmPopup,
    MPasswordShow,
    TenantCatalogRegisterForm,
    TenantOrganizationRegisterForm,
    TenantUserRegisterForm,
  },
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
    this.maxios.get("tenants/cloudflare/zones").then((res) => {
      this.tenantDomains = res;
    });
    // Lấy mã thuê bao mới:
    this.maxios.get("/tenants/new-code").then((res) => {
      this.tenant.TenantCode = res;
    });
    this.catalog.RootDomain = this.appDomain.BAZA;
    this.catalog.DatabaseName = `${this.dbPrefix}${
      this.catalog.SubDomain || ""
    }`;
    this.catalog.Server = this.bazaDbServerDefault.Host;
    this.catalog.Port = this.bazaDbServerDefault.Port;
    this.catalog.UserId = this.bazaDbServerDefault.UserId;
    this.catalog.Password = this.bazaDbServerDefault.Password;
  },
  watch: {
    appDomain: {
      handler: function (newValue) {
        this.catalog.RootDomain = newValue.BAZA;
      },
      deep: true,
    },
    "catalog.SubDomain": function (newValue) {
      this.catalog.DatabaseName = `${this.dbPrefix}${newValue || ""}`;
    },
    bazaDbServerDefault: function (newValue) {
      this.catalog.Server = newValue.Host || this.bazaDbServerDefault.Server;
      this.catalog.Port = newValue.Port;
      this.catalog.UserId = newValue.UserId;
      this.catalog.Password = newValue.Password;
    },
    "user.FullName": function (newValue) {
      this.tenant.TenantName = newValue;
    },
    "user.PhoneNumber": function (newValue) {
      this.tenant.PhoneNumber = newValue;
    },
  },
  /* eslint-disable */
  methods: {
    onFireErrorRegisterProgress() {
      this.hasErrorProgresRegister = true;
    },
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
    onClose() {
      router.push("/");
    },
    onSave() {
      this.stepRegister = 3;
      this.tenant.Organization = this.organization;
      this.tenant.TenantUser = this.user;
      this.tenant.Catalog = this.catalog;
      this.maxios
        .post(
          `tenants/admin-register`,
          this.tenant,
          this.setFocusFirstInputErrors,
          true
        )
        .then((res) => {
          this.stepRegister = 4;
          this.maxios.post(
            "/webserver-configs/nginx/restart",
            null,
            () => {
              this.appReady = true;
            },
            true
          );
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
                case "Catalog.DatabaseName":
                  this.stepRegister = 2;
                  this.$refs.frmTenantCatalog.$refs.txtDatabaseName.setInValidState(true, erroText);
                  propErrors.push(this.$refs.frmTenantCatalog.$refs.txtDatabaseName);
                  break;
                case "Catalog.SubDomain":
                  this.stepRegister = 2;
                  this.$refs.frmTenantCatalog.$refs.txtSubDomain.setInValidState(true, erroText);
                  propErrors.push(this.$refs.frmTenantCatalog.$refs.txtSubDomain);
                  break;
                case "TenantUser.UserName":
                  this.stepRegister = 0;
                  this.$refs.frmTenantUser.$refs.txtUserName.setInValidState(true, erroText);
                  propErrors.push(this.$refs.frmTenantUser.$refs.txtUserName);
                  break;
                case "TenantUser.Password":
                  this.stepRegister = 0;
                  this.$refs.frmTenantUser.$refs.txtPassword.setInValidState(true, erroText);
                  propErrors.push(this.$refs.frmTenantUser.$refs.txtPassword);
                  break;
                case "RePassword":
                  this.stepRegister = 0;
                  this.$refs.frmTenantUser.$refs.txtPassword.setInValidState(true, erroText);
                  propErrors.push(this.$refs.frmTenantUser.$refs.txtPassword);
                  break;
                case "TenantUser.FullName":
                  this.stepRegister = 0;
                  this.$refs.frmTenantUser.$refs.txtFullName.setInValidState(true, erroText);
                  propErrors.push( this.$refs.frmTenantUser.$refs.txtFullName);
                  break;
                case "TenantUser.Mobile":
                  this.stepRegister = 0;
                  this.$refs.frmTenantUser.$refs.txtUserPhoneNumber.setInValidState(true, erroText);
                  propErrors.push( this.$refs.frmTenantUser.$refs.txtUserPhoneNumber);
                  break;
                case "Organization.Email":
                  this.stepRegister = 1;
                  this.$refs.frmTenantOrg.$refs.txtEmail.setInValidState(true, erroText);
                  propErrors.push( this.$refs.frmTenantOrg.$refs.txtEmail);
                  break;
                case "Organization.OrganizationName":
                  this.stepRegister = 1;
                  this.$refs.frmTenantOrg.txtOrganizationName.setInValidState(
                    true,
                    erroText
                  );
                  propErrors.push(this.$refs.frmTenantOrg.txtOrganizationName);
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
    },
    onPrevStep() {
      this.stepRegister -= 1;
    },
    onNextStep1() {
      var stepOneValid = this.$refs.frmTenantUser.onValidate();
      if (stepOneValid) {
        // Lấy mã cửa hàng mới:
        this.maxios.get("organizations/new-code").then((res) => {
          this.organization.OrganizationCode = res;
        });
        this.organization.Email = this.user.Email;
        this.organization.PhoneNumber = this.user.PhoneNumber;
        this.organization.OwnerName = this.user.FullName;
        this.stepRegister = 1;
        this.$refs.frmTenantOrg.setFirstFocus();
      }
    },
    onNextStep2() {
      var isValid = this.$refs.frmTenantOrg.onValidate();
      if (isValid) {
        this.stepRegister = 2;
        this.$nextTick(function () {
          this.$refs.frmTenantCatalog.setFirstFocus();
        });
      }
    },
    onNextStep3() {
      var isValid = this.$refs.frmTenantCatalog.onValidate();
      if (isValid) {
        this.showConfirmRegister = true;
      }
    },
    onConfirmRegister() {
      this.onSave();
      this.showConfirmRegister = false;
    },
    onFinish() {
      this.$router.push("/tenants");
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
      dbPrefix: "baza_",
      organization: {
        OrganizationName: "Cửa hàng Test",
      },
      user: {
        UserName: "nvmanh",
        FullName: "Nguyễn Văn Mạnh",
        Password: "1",
        RePassword: "1",
        PhoneNumber: "32323",
        Email: "abc@gmail.com",
      },
      catalog: {},
      isValidated: false,
      stepRegister: 0,
      propFirstNotValid: null,
      tokenConfirm: "",
      isConfirmed: false,
      hasErrorProgresRegister: false,
      tenant: { AddInventoryData: true },
      showConfirmRegister: false,
      tenantDomains: [],
      appReady: false,
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

.link--back-login {
  flex: 1;
}

.tenant-info {
  padding: 10px;
  background-color: #dedede;
  display: grid;
  gap: 4px;
  border-radius: 4px;
}
.register-item {
  display: flex;
  align-items: center;
}

.register__label {
  flex: 0 0 100px;
  font-weight: 700;
}

.app--restarting {
  color: #ff0000;
}

.app--ready {
  color: #008000;
}
</style>
