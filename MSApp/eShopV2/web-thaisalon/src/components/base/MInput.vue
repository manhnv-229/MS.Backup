<template>
  <div class="input-wrapper" @click="onClick">
    <label v-if="label"
      >{{ label }}
      <span v-if="required">(<span class="required">*</span>)</span>:</label
    >
    <div class="input-block">
      <span
        v-if="isShowClear"
        class="clear-button"
        :class="{ 'clear-button--left': align == 'right' }"
        @click="clearButtonOnClick"
        ><i class="icofont-close"></i
      ></span>
      <input
        :type="type"
        ref="minput"
        :required="required"
        :placeholder="placeholder"
        v-model="value"
        @input="onInput"
        @keydown="onKeyDown"
        @blur="onBlur"
        :maxlength="maxLength"
        :autocomplete="autocomplete"
        :disabled="disabled"
        :style="{ 'text-align': align }"
        class="input"
       
        :class="{
          'input--invalid': inValid,
          'input-editable': editOnClick,
          'input-money': format == 'money',
        }"
      />
      <span v-if="format == 'money'" class="money-code">{{ moneyCode }}</span>
    </div>

    <div v-if="inValid" class="validate-error">
      <div class="error__content">
        <span v-if="errorText" v-html="errorText"></span>
        <div v-else>
          <span v-if="label"
            ><b>{{ label }}</b></span
          ><span v-else>này</span> không được phép để trống.
        </div>
      </div>
      <!-- <div class="error__arrow">
        <div class="arrow"></div>
      </div> -->
    </div>
  </div>
</template>
<script>
// const focus = {
//   mounted: (el) => el.focus(),
// };
export default {
  name: "MInput",
  directives: {
    // enables v-focus in template
    // focus,
  },
  props: {
    modelValue: [String, Number, Boolean],
    editOnClick: {
      type: Boolean,
      default: false,
      required: false,
    },
    align: {
      type: String,
      default: "left",
      required: false,
    },
    placeholder: { type: String, default: "", required: false },
    autocomplete: { type: String, default: "", required: false },
    label: { type: String, default: null, required: false },
    type: { type: String, default: "text", required: false },
    format: { type: String, default: "text", required: false },
    moneyCode: { type: String, default: "đ", required: false },
    isFocus: { type: Boolean, default: false, required: false },
    required: { type: Boolean, default: false, required: false },
    disabled: { type: Boolean, default: false, required: false },
    validated: { type: Boolean, default: false, required: false },
    onlyNumberChar: { type: Boolean, default: false, required: false },
    maxLength: { type: Number, required: false },
  },
  emits: ["update:modelValue", "onValidate", "update:validated", "onBlur","onChange"],
  created() {
    if (this.format == "money") {
      this.value = this.getTextFromFormatValueMoney(this.modelValue||0);
    } else {
      if (this.type == "number") {
        this.value = eval(this.modelValue);
      } else {
        this.value = this.modelValue;
      }
    }
    this.selfValidated = this.validated;
  },
  mounted() {
    if (this.isFocus == true) {
      this.$nextTick(function () {
        this.$refs.minput.focus();
      });
    }
  },
  computed: {
    isDisabled() {
      if (
        this.disabled ||
        (!this.disabled && this.editOnClick && !this.allowEdit)
      ) {
        return true;
      } else {
        return false;
      }
    },
    inValid: function () {
      if (
        ((this.validated || this.selfValidated) &&
          this.required &&
          (this.modelValue == "" ||
            this.modelValue == undefined ||
            this.modelValue == null)) ||
        this.errorText
      ) {
        return true;
      } else {
        return false;
      }
    },
    isShowClear() {
      if (this.value && !this.isDisabled && this.format != "money") {
        return true;
      } else {
        return false;
      }
    },
  },
  methods: {
    onInput() {
      if (this.format === "money") {
        var value = this.getValueFromFormatMoneyText(this.value);
        this.value = this.getTextFromFormatValueMoney(value);
        this.$emit("update:modelValue", value);
        console.log("value: ", value);
        this.$emit("onChange", value);
      } else {
        this.$emit("update:modelValue", this.value);
        this.$emit("onChange", this.value);
      }
    },
    onKeyDown(evt) {
      evt = evt ? evt : window.event;
      if (this.onlyNumberChar == true || this.format == "money") {
        var charCode = evt.which ? evt.which : evt.keyCode;
        if (
          charCode > 31 &&
          (charCode < 48 || charCode > 57) &&
          (charCode < 96 || charCode > 105) &&
          charCode != 39 &&
          charCode != 37 &&
          !(charCode == 187 && this.type == "tel")
        ) {
          evt.preventDefault();
        }
      }
    },
    onClick(e) {
      if (this.editOnClick && !this.allowEdit) {
        this.allowEdit = true;
        //  const sel = el.selectionStart;
        this.$nextTick(function () {
          this.$refs.minput.focus();
          if (this.value && this.type != "number") {
            const valueLength = this.value.toString().length;
            const el = e.target;
            el.setSelectionRange(0, valueLength);
          }
        });
      }
    },
    onBlur() {
      this.selfValidated = true;
      this.$emit("onBlur");
      // Thiết lập lại mặc định giá trị allowEdit
      if (this.editOnClick) {
        this.allowEdit = false;
      }
    },
    clearButtonOnClick() {
      this.$emit("update:modelValue", null);
    },
    /**
     * Lấy chuỗi được định dạng tiền tệ
     * @param {any} newValue
     */
    getTextFromFormatValueMoney(newValue) {
      // return `${this.moneyCode} ${newValue}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
      return `${newValue}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    },

    /**
     * Lấy giá trị từ chuỗi định dạng tiền
     * @param {any} value
     */
    getValueFromFormatMoneyText(value) {
      //eslint-disable-next-line
      value = `${value}`.replace(/\đ\s?|(,*)/g, "");
      value = `${value}`.replace(`${this.moneyCode}`, "");
      return value;
    },

    setInValidState(isInValid, errorText) {
      this.selfValidated = isInValid;
      if (isInValid && errorText) {
        this.errorText = errorText;
      } else {
        this.errorText = "";
      }
    },

    setFocus(){
      this.$nextTick(function(){
        this.$refs.minput.focus();
      })
    }
  },
  watch: {
    validated: function (newValue) {
      if (newValue == true) {
        this.selfValidated = true;
        if (
          this.modelValue == "" ||
          this.modelValue == undefined ||
          this.modelValue == null
        ) {
          this.$emit("onValidate", false, this.$refs.minput);
        } else {
          this.$emit("onValidate", true, this.$refs.minput);
        }
      }
    },
    modelValue: function (newValue) {
      if (this.format === "money") {
        this.value = this.getTextFromFormatValueMoney(newValue);
      } else {
        this.value = newValue;
      }
      this.selfValidated = true;
      this.errorText = "";
      if (this.value == "" || this.value == undefined || this.value == null) {
        this.$emit("onValidate", false, this.$refs.minput);
      } else {
        this.$emit("onValidate", true, this.$refs.minput);
      }
    },
  },
  data() {
    return {
      value: null,
      selfValidated: false,
      allowEdit: false,
      errorText: "",
    };
  },
};
</script>
<style scoped>
.input {
  box-sizing: border-box;
}
input[type="number"] {
  text-align: right;
  padding-right: 16px;
}

input[type="tel"] {
  text-align: left;
}
.required {
  color: #ff0000;
}

.input-wrapper {
  position: relative;
  width: 100%;
  /* margin: 20px 0; */
}

.input-wrapper input {
  width: 100%;
  box-sizing: border-box;
}

.input-wrapper input:disabled {
}

/* .input-editable{
  border: unset;
} */

.input-editable:disabled {
  border: unset !important;
}

.input-wrapper label + div:has(input) {
  margin-top: 4px;
}

.input-wrapper + .input-wrapper {
}
.validate-error {
  position: relative;
  right: 0px;
  display: flex;
  font-size: 11px;
  flex-direction: column;
  align-items: flex-start;
  justify-content: center;
  color: #ff0000;
}
.error__arrow {
  position: absolute;
  top: calc(100% + 4px);
  width: 100%;
  height: 0px;
  display: flex;
  align-items: flex-end;
  justify-content: center;
}

.arrow {
  width: 10px;
  height: 10px;
  background-color: #ff0000;
  transform: rotate(45deg);
}
.input-block {
  position: relative;
}

.input-money {
  padding-right: 26px !important;
}

.money-code {
  display: block;
  position: absolute;
  right: 8px;
  top: 50%;
  transform: translate(-50%, -50%);
  padding: 0;
  border-bottom: 1px solid #212121;
  line-height: 12px;
  width: 6px;
  font-size: 12px;
}
.clear-button {
  display: block;
  position: absolute;
  right: 8px;
  top: 50%;
  transform: translate(-50%, -50%);
  padding: 2px;
  line-height: 12px;
  font-size: 12px;
  text-align: center;
  color: #fff;
  border-radius: 50%;
  background-color: #7e7e7e;
  cursor: pointer;
}

.clear-button--left {
  right: unset !important;
  left: 14px;
}
</style>
