<template>
  <MDialogDragable>
    <template #header
      >Tiến trình xoá {{ item.SubDomain }}.{{
        item.RootDomain
      }}</template
    >
    <template #content>
      <div class="delete-step">
        <div
          class="progress-step"
          v-for="(stepItem, index) in steps"
          :class="{ 'step--complete': stepItem === stepIndexComplete }"
          :key="index"
        >
          <div
            class="step__icon"
            :class="{
              'icon--complete': stepItem.StepIndex <= stepIndexComplete,
              'icon--loading': stepItem.StepIndex === stepIndexComplete + 1,
              'icon--waiting': stepItem.StepIndex > stepIndexComplete + 1,
              'icon--error': stepItem.StepIndex == stepError,
            }"
          ></div>
          <div class="step__text">
            {{ stepItem.StepIndex }} - {{ stepItem.StepName }}
          </div>
          <div
            class="step__info"
            :class="{
              'icon--setting-gif': stepItem.StepIndex === stepIndexComplete + 1,
            }" 
          >
            <span v-if="stepItem.StepIndex <= stepIndexComplete"
              >Hoàn thành</span
            >
            <span
              v-if="stepItem.StepIndex === stepError"
              class="step-error-info"
              >Lỗi</span
            >
          </div>
        </div>
      </div>
    </template>
    <template #footer>
      <MButtonIcon
        text="Hoàn thành"
        cls="btn--default"
        iconCls="icon icon-20 icon--confirm"
        :disabled="stepIndexComplete < 5 || stepError > 0"
        @click="onDeleteFinish"
      ></MButtonIcon>
    </template>
  </MDialogDragable>
</template>
<script>
export default {
  name: "DeleteTenantProgress",
  emits: ["onFinish"],
  props: ["item"],
  inject: [],
  created() {
    this.hubConnection.on("UpdateProgress", this.updateDeleteProgress);
    this.hubConnection.on(
      "UpdateeProgressWhenHasError",
      this.updateDeleteProgressWhenHasError
    );
  },
  beforeUnmount() {
    this.hubConnection.off("UpdateProgress", this.updateDeleteProgress);
    this.hubConnection.off(
      "UpdateProgressWhenHasError",
      this.updateDeleteProgressWhenHasError
    );
  },
  computed: {},
  methods: {
    updateDeleteProgress(stepCode, step) {
      console.log(`${step}-${stepCode}`);
      this.stepIndexComplete = step;
    },
    updateDeleteProgressWhenHasError(stepCode, step) {
      this.stepError = step;
    },
    onDeleteFinish() {
      this.$emit("onFinish");
      this.stepError = 0;
      this.stepIndexComplete = 0;
    },
  },
  data() {
    return {
      stepError: 0,
      stepIndexComplete: 0,
      steps: [
        { StepIndex: 1, StepName: "Kiểm tra phát sinh dữ liệu" },
        { StepIndex: 2, StepName: "Xoá cơ sở dữ liệu." },
        { StepIndex: 3, StepName: "Xoá địa chỉ Website." },
        { StepIndex: 4, StepName: "Xoá cấu hình tên miền." },
        {
          StepIndex: 5,
          StepName: "Xoá đăng ký và thông tin thuê bao.",
        },
      ],
    };
  },
};
</script>
<style scoped>
.delete-step {
  display: grid;
  gap: 10px;
}
.progress-step {
  padding: 10px;
  display: flex;
  align-items: center;
  gap: 4px;
  box-shadow: 0 0 3px #dedede;
  align-content: space-between;
}

.progress-step:has(.icon--loading) {
  font-weight: 700;
}

.step__icon {
  width: 32px;
  height: 32px;
  background-repeat: no-repeat;
  background-size: contain;
  background-position: center;
}

.step__info {
  flex: 1;
  text-align: right;
  padding-right: 12px;
  background-repeat: no-repeat;
  background-size: contain;
  background-position: 100% center;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.step__info:has(.step-error-info) {
  background-image: none;
}

.step--complete {
  color: var(--color-success);
  font-weight: 700;
}

.step-error-info {
  color: #ff0000;
}
</style>
