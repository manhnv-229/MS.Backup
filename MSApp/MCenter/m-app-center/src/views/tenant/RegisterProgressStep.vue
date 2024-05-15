<template>
  <div class="register-info">
    <div class="register__description">
      Quá trình đăng ký đang được xử lý tự động, vui lòng theo dõi tiến trình.<br/>
      <b>Lưu ý tiến trình đăng ký đang tiến hành sẽ không thể dừng lại.</b> <br/>
      Sau khi quá trình đăng ký hoàn tất, một email thông báo hoàn tất sẽ được gửi đến Email
      đăng ký.
    </div>
  </div>
  <div class="progress">
    <RegisterProgressStepItem
      v-for="(item, index) in steps"
      :key="index"
      :stepItem="item"
      :stepError="stepError"
      :stepIndexComplete="stepComplete"
    ></RegisterProgressStepItem>
  </div>
</template>
<script>
import RegisterProgressStepItem from "./RegisterProgressStepItem.vue";
export default {
  name: "RegisterProgressStep",
  components: { RegisterProgressStepItem },
  emits:["onFireErrorRegister"],
  inject: ["catalogRegister", "userRegister"],

  watch: {
    stepError: function (newValue) {
      if (newValue > 0) {
        this.$emit("onFireErrorRegister", newValue);
      }
    },
  },
  created() {
    this.hubConnection.on(
      "UpdateProgress",
      this.updateRegisterProgress
    );
    this.hubConnection.on(
      "UpdateProgressWhenHasError",
      this.updateRegisterProgressWhenFail
    );
  },
  beforeUnmount() {
    this.hubConnection.off(
      "UpdateProgress",
      this.updateRegisterProgress
    );
    this.hubConnection.off(
      "UpdateProgressWhenHasError",
      this.updateRegisterProgressWhenFail
    );
  },
  methods: {
    updateRegisterProgress(stepCode, stepComplete) {
      console.log(`${stepComplete}-${stepCode}`);
      this.stepComplete = stepComplete;
    },
    updateRegisterProgressWhenFail(stepCode, stepError) {
      console.log(`${stepComplete}-${stepCode}`);
      this.stepError = stepError;
    },
  },
  data() {
    return {
      stepError: 0,
      stepComplete: 0,
      steps: [
        { StepIndex: 1, StepName: "Xác thực thông tin thuê bao" },
        { StepIndex: 2, StepName: "Đăng ký thuê bao mới." },
        { StepIndex: 3, StepName: "Khai báo thông tin cửa hàng." },
        { StepIndex: 4, StepName: "Tạo cơ sở dữ liệu cho ứng dụng quản lý." },
        {
          StepIndex: 5,
          StepName: "Tạo tài khoản và cấp quyền truy cập ứng dụng.",
        },
        { StepIndex: 6, StepName: "Khởi tạo tên miền cho ứng dụng (nginx)." },
        {
          StepIndex: 7,
          StepName: "Cấu hình tên miền truy cập ứng dụng (cloudflare).",
        },
        { StepIndex: 8, StepName: "Cấp License dùng cho ứng dụng." },
        { StepIndex: 9, StepName: "Gửi thư thông báo thành công." },
      ],
    };
  },
};
</script>
<style scoped>
.progress {
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: 4px;
}

.register-info {
  box-shadow: 0 0 3px #dedede;
  color: var(--color-success);
  margin-bottom: 10px;
  display: grid;
  gap: 10px;
  padding-bottom: 10px;
}

.register-detail {
  border: 1px solid #ccc;
  border-radius: 4px;
  padding: 12px;
  width: fit-content;
}
</style>
