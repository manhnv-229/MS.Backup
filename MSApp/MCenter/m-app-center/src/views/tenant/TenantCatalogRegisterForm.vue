<template>
  <div class="app-info">
    <div class="line">Thông tin thuê bao & tên miền</div>
    <div class="group-info">
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Sub domain"
            ref="txtSubDomain"
            v-model="catalogRegister.SubDomain"
            :isDisabled="false"
            :onlyNumberChar="false"
            :required="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <!-- <m-input
            label="Root Domain"
            ref="txtServer"
            v-model="catalogRegister.RootDomain"
            :isDisabled="false"
            :onlyNumberChar="false"
            :required="true"
          >
          </m-input> -->
          <m-combobox
            label="Root domain"
            ref="cbxRootDomain"
            propText="name"
            propValue="name"
            :dataInput="tenantDomains"
            v-model="catalogRegister.RootDomain"
          >

          </m-combobox>
        </div>
      </div>
    </div>

    <div class="line">Thông tin máy chủ dữ liệu</div>
    <div class="group-info">
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Máy chủ dữ liệu"
            ref="txtServer"
            v-model="catalogRegister.Server"
            :isDisabled="false"
            :onlyNumberChar="false"
            :required="true"
          >
          </m-input>
        </div>
        <div class="m-col"></div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Tên Database"
            ref="txtDatabaseName"
            v-model="catalogRegister.DatabaseName"
            :isDisabled="false"
            :onlyNumberChar="false"
            :required="true"
          >
          </m-input>
        </div>
        <div class="m-col"></div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="User Id (Database)"
            ref="txtUserId"
            v-model="catalogRegister.UserId"
            :isDisabled="false"
            :onlyNumberChar="false"
            :required="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <m-input
            label="Mật khẩu"
            ref="txtPassword"
            v-model="catalogRegister.Password"
            :isDisabled="false"
            :onlyNumberChar="false"
            :required="true"
          >
          </m-input>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: "TenantCatalogRegisterForm",
  props: [],
  emits: [],
  inject: ["tenantRegister", "catalogRegister","tenantDomains","appDomain"],
  created() {
    
  },
  methods: {
    setFirstFocus() {
      this.$refs.txtSubDomain.setFocus();
    },
    onValidate() {
      var errorMsgs = [];
      var fieldsInValid = [];
      if (!this.commonJs.checkRequired(this.catalogRegister.SubDomain)) {
        let errorText = "<b>SubDomain</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtSubDomain.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtSubDomain);
      }
      if (!this.commonJs.checkRequired(this.catalogRegister.RootDomain)) {
        let errorText = "<b>Root Domain</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.cbxRootDomain.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.cbxRootDomain);
      }
      if (!this.commonJs.checkRequired(this.catalogRegister.Server)) {
        let errorText = "<b>Server</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtServer.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtServer);
      }
      if (!this.commonJs.checkRequired(this.catalogRegister.DatabaseName)) {
        let errorText = "<b>Tên CSDL</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtDatabaseName.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtDatabaseName);
      }
      if (!this.commonJs.checkRequired(this.catalogRegister.UserId)) {
        let errorText = "<b>User Id</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtUserId.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtUserId);
      }
      if (!this.commonJs.checkRequired(this.catalogRegister.Password)) {
        let errorText = "<b>Mật khẩu</b> không được để trống.";
        errorMsgs.push(errorText);
        this.$refs.txtPassword.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.txtPassword);
      }
      var rootDomain = this.catalogRegister.RootDomain;
      var isValidRootDomain = this.tenantDomains.some(i=>i.name === rootDomain);
      if (!isValidRootDomain) {
        let errorText = "<b>Root Domain</b> không hợp lệ.";
        errorMsgs.push(errorText);
        this.$refs.cbxRootDomain.setInValidState(true, errorText);
        fieldsInValid.push(this.$refs.cbxRootDomain);
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
    return {isValidated: false,};
  },
};
</script>
<style scoped>
.app-info{
  display: grid;
  gap: 16px;
}
.line {
  padding: 10px 0;
  box-shadow: 0 5px 5px #dedede;
  margin-bottom: 10px;
  font-weight: 700;
  border-top: 1px dashed #dedede;
}

.group-info{
  padding: 0 10px;
  padding-bottom: 10px;
}
</style>
