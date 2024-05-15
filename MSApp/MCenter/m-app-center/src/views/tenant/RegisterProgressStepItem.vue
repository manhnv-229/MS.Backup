<template>
  <div class="progress-step" :class="{ 'step--complete': isComplete }">
    <div
      class="step__icon"
      :class="{
        'icon--complete': isComplete,
        'icon--loading': isLoading,
        'icon--waiting': isWaiting,
        'icon--error': isError,
      }"
    ></div>
    <div class="step__text">{{ stepItem.StepIndex }} - {{ stepItem.StepName }}</div>
    <div class="step__info" :class="{'icon--setting-gif':isLoading}">
        <span v-if="isComplete">Hoàn thành</span>
        <span v-if="isError" class="step-error-info">Lỗi</span>
    </div>
  </div>
</template>
<script>
export default {
  name: "RegisterProgressStep",
  props: ["stepIndexComplete", "stepItem","stepError"],
  created() {
    this.hubConnection.on(
      "UpdateRegisterProgress",
      this.updateRegisterProgress
    );
  },
  computed: {
    stepStatus: function () {},
    isComplete: function () {
      return this.stepIndexComplete >= this.stepItem.StepIndex;
    },
    isLoading: function () {
        return this.stepIndexComplete == this.stepItem.StepIndex - 1;
    },
    isWaiting: function () {
        return (this.stepItem.StepIndex - this.stepIndexComplete) > 1;
    },
    isError: function(){
        return this.stepItem.StepIndex === this.stepError;
    }
  },
  beforeUnmount() {
    this.hubConnection.off(
      "UpdateRegisterProgress",
      this.updateRegisterProgress
    );
  },
  methods: {
   
  },
  data() {
    return {
    };
  },
};
</script>
<style scoped>

.progress-step {
  padding: 10px;
  display: flex;
  align-items: center;
  gap: 4px;
  box-shadow: 0 0 3px #dedede;
  align-content: space-between;
}

.progress-step:has(.icon--loading){
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

.step__info:has(.step-error-info){
    background-image: none;
}

.step--complete {
  color: var(--color-success);
  font-weight: 700;
}

.step-error-info{
    color: #ff0000;
}
</style>
