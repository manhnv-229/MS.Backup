<template>
  <div class="m-checkbox">
    <input :id="id" class="input-checkbox" :disabled="disabled" v-model="value" type="checkbox" name="mcheckbox"
      @change="onChange" @click="onClick" /><label :for="id" v-if="label">{{ label }}</label>
  </div>
</template>
<script>
export default {
  name: "MCheckbox",
  emits: ["update:modelValue", "onChange", "onClick"],
  props: ["label", "id", "modelValue", "disabled"],
  created() {
    var isTrueSet = (String(this.modelValue)?.toLowerCase() === 'true' || (String(this.modelValue)?.toLowerCase() !== "0" && String(this.modelValue)?.toLowerCase() !== 'false'));
    this.value = isTrueSet;
  },
  watch: {
    modelValue(newValue) {
      // console.log(`modelValue - ${this.label}`, newValue);
      var isTrueSet = (String(newValue)?.toLowerCase() === 'true' || String(newValue)?.toLowerCase() == "1");
      this.value = isTrueSet;
    },
  },
  methods: {
    onChange() {
      this.$emit("update:modelValue", this.value);
      this.$emit("onChange", this.value);
    },
    setCheck(isCheck) {
      this.value = isCheck;
    },
    onClick() {
      this.$emit("onClick", this.value);
    }
  },
  data() {
    return {
      value: null,
    };
  },
};
</script>
<style scoped>
.m-checkbox {
  display: flex;
  align-items: center;
  column-gap: 5px;
}

.input-checkbox {
  height: 20px;
  width: 20px;
  border: 1px solid #ccc;
}

.input-checkbox:focus,
.input-checkbox:active {
  outline: 1px dashed #0033ff;
}
</style>
