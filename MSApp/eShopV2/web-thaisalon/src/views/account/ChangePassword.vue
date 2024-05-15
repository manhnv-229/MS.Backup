<template>
  <m-dialog title="Đổi mật khẩu" @onClose="onClose">
    <template v-slot:content>
      <form action="">
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Tài khoản đăng nhập:"
              ref="txtUserName"
              :modelValue="user.UserName"
              :required="true"
              :isDisabled="true"
            >
            </m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Mật khẩu hiện tại"
              ref="txtPassword"
              type="password"
              v-model="user.Password"
              :required="true"
              :isDisabled="false"
            >
            </m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Mật khẩu mới"
              ref="txtPassword"
              type="password"
              v-model="user.NewPassword"
              :required="true"
              :isDisabled="false"
            >
            </m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Xác nhận mật khẩu mới"
              ref="txtPassword"
              type="password"
              v-model="user.ReNewPassword"
              :required="true"
              :isDisabled="false"
            >
            </m-input>
          </div>
        </div>
      </form>
    </template>
    <template v-slot:footer>
      <button class="btn btn--close" @click="onClose">
        <i class="icofont-ui-close"></i> Hủy
      </button>
      <button class="btn btn--default mg-left-10" @click="onSave">
        <i class="icofont-key"></i> Đổi mật khẩu
      </button>
    </template>
  </m-dialog>
</template>
<script>
import Enum from "@/scripts/enum";
export default {
  name: "ChangePassword",
  emits: ["onClose"],
  props: ["id"],
  computed: {
    formMode: function () {
      return Enum.FormMode.UPDATE;
    },
  },
  created() {
    this.user.UserName = localStorage.getItem("userName");
  },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      this.maxios.put(`accounts/change-password`, this.user).then(() => {
        this.commonJs.showMessenger({
          title: "Đổi mật khẩu thành công",
          msg:
            "Bạn đã thay đổi mật khẩu thành công. Bạn có thể sử dụng mật khẩu mới cho lần đăng nhập kế tiếp.",
          type: Enum.MsgType.Success,
          confirm: () => {
            this.$emit("onClose");
          },
        });
      });
    },
  },
  data() {
    return {
      user: {},
    };
  },
};
</script>
<style scoped></style>
