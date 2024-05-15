<template>
  <m-page title="THÔNG TIN DOANH NGHIỆP">
    <!-- <template v-slot:header-right></template> -->
    <template v-slot:container>
      <div class="org-container">
        <div
          class="license-info"
          :class="{ 'license--limitted': !licenseDate }"
        >
          <div class="license__icon"><i class="icofont-license"></i></div>
          <div
            v-if="licenseDate"
            class="license__content"
            v-html="licenseInfoHTML"
          ></div>
          <div v-else class="license__content" v-html="licenseInfoHTML"></div>
        </div>
        <div class="m-tab">
          <div class="m-tab__list">
            <button
              class="m-tab__button"
              :class="{ 'm-tab__button--active': tabActive === 'info' }"
              @click="onChangeTab('info')"
            >
              Thông tin cửa hàng
            </button>
            <button
              class="m-tab__button"
              :class="{ 'm-tab__button--active': tabActive === 'setting' }"
              @click="onChangeTab('setting')"
            >
              Cài đặt cửa hàng
            </button>
            <button
              class="m-tab__button"
              :class="{ 'm-tab__button--active': tabActive === 'navbar' }"
              @click="onChangeTab('navbar')"
            >
              Thanh điều hướng
            </button>
          </div>
          <div class="m-tab__content">
            <!-- THÔNG TIN CHUNG -->
            <div v-if="tabActive === 'info'" class="org--info">
              <div class="m-row">
                <div class="m-col" style="width: 100px">
                  <m-input
                    label="Mã"
                    ref="txtorganizationCode"
                    v-model="organization.OrganizationCode"
                    :required="true"
                    :disabled="true"
                    :isFocus="false"
                  >
                  </m-input>
                </div>
                <div class="m-col"></div>
              </div>
              <div class="m-row">
                <div class="m-col flex-1">
                  <m-input
                    label="Tên doanh nghiệp"
                    ref="txtorganizationName"
                    v-model="organization.OrganizationName"
                    :required="true"
                    :isDisabled="false"
                    :isFocus="true"
                  >
                  </m-input>
                </div>
              </div>
              <div class="m-row">
                <div class="m-col">
                  <m-input
                    label="Lĩnh vực kinh doanh (VD: Tạp hóa/ Dịch vụ giải trí)"
                    ref="txtorganizationCode"
                    v-model="organization.Description"
                    :isDisabled="false"
                  >
                  </m-input>
                </div>
              </div>
              <div class="m-row">
                <div class="m-col">
                  <m-input
                    label="Slogan (khẩu hiệu):"
                    ref="txtorganizationCode"
                    v-model="organization.Slogan"
                    :isDisabled="false"
                  >
                  </m-input>
                </div>
              </div>
              <div class="m-row">
                <div class="m-col">
                  <m-input
                    label="Chủ doanh nghiệp/ đại lý"
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
                    v-model="organization.Email"
                    :isDisabled="false"
                    :required="true"
                    :onlyNumberChar="true"
                  >
                  </m-input>
                </div>
              </div>
              <div class="m-row">
                <div class="m-col">
                  <m-input
                    label="Giấy phép hoạt động kinh doanh (nếu có)"
                    v-model="organization.BusinessLicense"
                    :isDisabled="false"
                    :onlyNumberChar="true"
                  >
                  </m-input>
                </div>
                <div class="m-col">
                  <m-input
                    label="Website"
                    v-model="organization.Website"
                    :isDisabled="false"
                  >
                  </m-input>
                </div>
              </div>
              <div class="m-row">
                <m-text-area
                  label="Địa chỉ"
                  v-model="organization.Address"
                ></m-text-area>
              </div>
            </div>
            <!-- CÀI ĐẶT -->
            <div v-else-if="tabActive === 'setting'" class="org--setting">
              <!-- <button id="btnAddSetting" class="btn">Thêm cài đặt</button> -->
              <div
                class="m-row"
                v-for="setting in organization.Setting"
                :key="setting"
              >
                <div class="m-col">
                  <m-checkbox
                    v-if="
                      setting.SettingValueType == Enum.SettingValueType.BOOLEAN
                    "
                    label="Gửi Email thông báo khi lập hoặc sửa hóa đơn. (Việc này có thể ảnh hưởng đến hiệu năng của chương trình.)"
                    v-model="setting.SettingValue"
                    @onChange="onChangeSetting(setting)"
                  ></m-checkbox>
                  <m-input
                    cls="setting-input"
                    :label="setting.SettingName"
                    v-else-if="
                      setting.SettingValueType == Enum.SettingValueType.INTEGER
                    "
                    v-model="setting.SettingValue"
                    :required="true"
                    :disabled="false"
                    :isFocus="false"
                    :onlyNumberChar="true"
                    @onBlur="onChangeSetting(setting)"
                    align="right"
                  >
                  </m-input>
                  <m-input
                    :label="setting.SettingName"
                    v-else-if="
                      setting.SettingValueType == Enum.SettingValueType.STRING
                    "
                    v-model="setting.SettingValue"
                    @onBlur="onChangeSetting(setting)"
                    :required="true"
                    :disabled="false"
                    :isFocus="false"
                  >
                  </m-input>
                </div>
                <div class="m-col"></div>
              </div>
            </div>
            <div v-else-if="tabActive === 'navbar'" class="org--setting">
              <NavbarSetting></NavbarSetting>
            </div>
          </div>
        </div>
      </div>
    </template>
    <template v-slot:footer>
      <div class="footer__button">
        <!-- <button class="btn btn--close" @click="onClose">Hủy</button> -->
        <button
          class="btn btn--default mg-left-10"
          :disabled="!isChange"
          @click="onSave"
        >
          <i class="icofont-ui-check"></i> Lưu
        </button>
      </div>
    </template>
  </m-page>
</template>
<script>
import Enum from "../../scripts/enum";
import router from "../../router";
import NavbarSetting from "./NavbarSetting.vue";
import { computed } from "vue";
import store from "@/store";
import { UPDATE_ORGANIZATION_INFO } from "@/store/actions/signalR";
export default {
  name: "OrganizationSettings",
  emits: ["onClose"],
  props: ["id"],
  components: {NavbarSetting },
  inject: ["organization"],
  computed: {
    dragOptions() {
      return {
        animation: 200,
        group: "description",
        disabled: false,
        ghostClass: "ghost",
      };
    },
    formMode: function () {
      return Enum.FormMode.UPDATE;
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
                        <li>SĐT: <a href="tel:0933365999">0933.365.999 (Mr Mạnh)</a></li>
                        <li>Email: <a href="mail:manhhao.com@gmail.com">manhhao.com@gmail.com</a></li>
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
    // var orgId = localStorage.getItem("organizationId");
    // if (orgId) {
    //   this.maxios.get(`organizations/${orgId}`).then((res) => {
    //     this.organization = res;
    //     this.licenseInfoHTML = this.licenseInfo;
    //     this.organizationOrigilJSON = JSON.stringify(res);
    //   });
    // } else {
    //   router.push("/");
    // }
  },
  methods: {
    onClose() {
      router.push("/");
    },
    onSave() {
      this.maxios
        .put(
          `organizations/${this.organization.OrganizationId}`,
          this.organization
        )
        .then(() => {
          this.$emit("onClose");
          store.dispatch(UPDATE_ORGANIZATION_INFO);
        });
    },
    onChangeTab(tabName) {
      this.tabActive = tabName;
    },
    onChangeSetting(setting) {
      setting.EntityState = this.Enum.EntityState.UPDATE;
    },
  },
  data() {
    return {
      // organization: {},
      licenseInfoHTML: "",
      organizationOrigilJSON: null,
      tabActive: "info",
      allowSentEmail: false,
      showNavbarForm: false,
      navbarItem: { UseIconFont: false, IsSytem: false },
      drag: false,
      navItemUpdate: {},
    };
  },
};
</script>
<style scoped>
.ghost {
  opacity: 0.5;
  background: #c8ebfb;
}

.org-container {
  padding-bottom: 10px;
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
  justify-content: flex-end;
  padding: 10px 0;
  border-top: 1px dashed #dedede;
}

.m-tab {
  display: grid;
  row-gap: 16px;
}

.m-tab__list {
  height: 36px;
  width: 100%;
  background-color: #dedede;
}

.m-tab__button {
  height: 100%;
  border: unset;
  padding: 0 16px;
  cursor: pointer;
  background-color: unset;
}

.m-tab__button--active {
  background-color: #00b900;
  color: #fff;
}

.setting-input {
  display: flex;
  align-items: center;
  column-gap: 10px;
}

.setting-input input {
  margin-top: unset;
}

#btnAddSetting {
}
</style>
