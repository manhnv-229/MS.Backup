<template>
  <MDialogDragable @onClose="onClose">
    <template #header>Xác nhận thông tin đăng ký</template>
    <template #content>
      <div class="confirm-info">
        <div class="m-row register-item">
          <div class="register__label">Tên cửa hàng:</div>
          <div class="register__text">
            {{ organizationRegister.OrganizationName }}
          </div>
        </div>
        <div class="m-row register-item">
          <div class="register__label">Số điện thoại:</div>
          <div class="register__text">
            {{ organizationRegister.PhoneNumber }}
          </div>
        </div>
        <div class="m-row register-item">
          <div class="register__label">Email:</div>
          <div class="register__text">{{ organizationRegister.Email }}</div>
        </div>
        <div class="m-row register-item">
          <div class="register__label">Tên miền:</div>
          <div class="register__text">
            <a :href="`${catalogRegister.SubDomain}.${catalogRegister.RootDomain}`"
              >{{ catalogRegister.SubDomain }}.{{ catalogRegister.RootDomain }}</a
            >
          </div>
        </div>
        <div class="m-row register-item">
          <div class="register__label">Tài khoản:</div>
          <div class="register__text">{{ userRegister.UserName }}</div>
        </div>
        <div class="m-row register-item">
          <div class="register__label">Mật khẩu:</div>
          <div class="register__text">
            <MPasswordShow :input="userRegister.Password"></MPasswordShow>
          </div>
        </div>
      </div>
      <div class="add-inventory">
        <MCheckbox label="Thêm dữ liệu hàng hoá" v-model="tenantRegister.AddInventoryData"></MCheckbox>
      </div>
      <div class="confirm__note">
        Lưu ý: Quá trình đăng ký sẽ không thể dừng lại sau khi tiến hành.
      </div>
    </template>
    <template #footer>
      <MButtonIcon
        cls="btn--close"
        iconCls="icon icon-20 icon--close-white"
        text="Huỷ bỏ"
        @click="onClose"
      ></MButtonIcon>
      <MButtonIcon
        cls="btn--default"
        iconCls="icon icon-20 icon--confirm"
        text="Xác nhận"
        @click="onConfirmRegister"
      ></MButtonIcon>
    </template>
  </MDialogDragable>
</template>
<script>
import MCheckbox from "@/components/base/MCheckbox.vue";
import MPasswordShow from "@/components/base/MPasswordShow.vue";
export default {
  name: "RegisterConfirmPopup",
  inject: [
    "catalogRegister",
    "userRegister",
    "tenantRegister",
    "organizationRegister",
    "appDomain",
  ],
  components: { MPasswordShow },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onConfirmRegister() {
      this.$emit("onConfirm");
    },
  },
};
</script>
<style scoped>
button + button {
  margin-left: 4px;
}
.confirm__note {
  margin-top: 10px;
  color: #ff0000;
  background-color: yellow;
  padding: 10px;
}
.confirm-info {
  border: 1px dashed var(--color-success);
  padding: 10px;
  border-radius: 4px;
  display: grid;
}

.register-item {
  display: flex;
  align-items: center;
}

.register__label {
  flex: 0 0 100px;
  font-weight: 700;
}

.add-inventory{
  padding: 10px 0;
}
</style>
