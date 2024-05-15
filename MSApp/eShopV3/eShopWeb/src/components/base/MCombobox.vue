<template>
  <div class="combobox-wrapper" :class="cls">
    <label v-if="label" for=""
      >{{ label }}
      <span v-if="required">(<span class="required">*</span>)</span>:</label
    >
    <div
      class="combobox"
      ref="mcombobox"
      :class="{ 'combobox--error': errorText }"
    >
      <span
        v-if="isShowClear && !isDisabled"
        class="clear-button"
        @click="clearButtonOnClick"
        :class="{ right72: showAddButton == true }"
        ><i class="icofont-close"></i
      ></span>
      <input
        type="text"
        class="input combobox__input"
        ref="minput"
        v-model="textInput"
        :placeholder="placeholder"
        @input="inputOnChange"
        :disabled="isDisabled"
        @keydown="selecItemUpDown"
        @blur="onBlur"
      />
      <button
        v-if="!isDisabled"
        class="button combobox__button"
        @click="btnSelectDataOnClick"
        type="button"
        @keydown="selecItemUpDown"
        tabindex="-1"
        :disabled="isDisabled"
      >
        <!-- <i class="fa-solid fa-chevron-down"></i> -->
        <img
          :src="require('./icon/down.png')"
          alt=""
          srcset=""
          width="24"
          height="24"
        />
      </button>
      <button
        v-if="showAddButton"
        @click="btnExtOnClick"
        class="btnComboboxExt"
        :class="iconExtCls"
      ></button>
      <div
        v-if="isShowListData"
        class="combobox__data"
        ref="combobox__data"
        v-clickoutside="hideListData"
      >
        <a
          class="combobox__item"
          v-for="(item, index) in dataFilter"
          :class="{
            'combobox__item--focus': index == indexItemFocus,
            'combobox__item--selected':
              index == indexItemSelected || modelValue == item[this.propValue],
            'combobox__item--disable-select': item['DisableSelect'],
          }"
          :key="item[this.propValue]"
          :ref="'toFocus_' + index ?? 0"
          :value="item[this.propValue]"
          @click="itemOnSelect(item, index)"
          @focus="saveItemFocus(index)"
          @keydown="selecItemUpDown(index)"
          @keyup="selecItemUpDown(index)"
          tabindex="1"
        >
          <div class="combobox__item-icon">
            <img
              v-show="index == indexItemSelected"
              :src="require('./icon/check.png')"
              alt=""
              srcset=""
              width="14"
              height="11"
            />
          </div>
          <span>{{ item[this.propText] }}</span>
        </a>
      </div>
      <div v-if="showLoadingData" class="combobox-loading"></div>
    </div>
    <div v-if="errorText" class="combobox--error-text" v-html="errorText"></div>
  </div>
</template>
<script>
/* eslint-disable */
/**
 * Gán sự kiện nhấn click chuột ra ngoài combobox data (ẩn data list đi)
 * NVMANH (31/07/2022)
 */
const clickoutside = {
  mounted(el, binding, vnode, prevVnode) {
    el.clickOutsideEvent = (event) => {
      // Nếu element hiện tại không phải là element đang click vào
      // Hoặc element đang click vào không phải là button trong combobox hiện tại thì ẩn đi.
      if (
        !(
          (
            el === event.target || // click phạm vi của combobox__data
            el.contains(event.target) || //click vào element con của combobox__data
            el.previousElementSibling.contains(event.target) ||
            el.previousElementSibling.previousElementSibling.contains(
              event.target
            )
          ) //click vào element button trước combobox data (nhấn vào button thì hiển thị)
        )
      ) {
        // Thực hiện sự kiện tùy chỉnh:
        binding.value(event, el);
        // vnode.context[binding.expression](event); // vue 2
      }
    };
    document.body.addEventListener("click", el.clickOutsideEvent);
  },
  beforeUnmount: (el) => {
    document.body.removeEventListener("click", el.clickOutsideEvent);
  },
};

function removeVietnameseTones(str) {
  if (str == null || str == undefined) {
    return "";
  }
  str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
  str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
  str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
  str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
  str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
  str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
  str = str.replace(/đ/g, "d");
  str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
  str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
  str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
  str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
  str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
  str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
  str = str.replace(/Đ/g, "D");
  // Some system encode vietnamese combining accent as individual utf-8 characters
  // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
  str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
  str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư
  // Remove extra spaces
  // Bỏ các khoảng trắng liền nhau
  str = str.replace(/ + /g, " ");
  str = str.trim();
  // Remove punctuations
  // Bỏ dấu câu, kí tự đặc biệt
  str = str.replace(
    /!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g,
    " "
  );
  return str;
}

const keyCode = {
  Enter: 13,
  ArrowUp: 38,
  ArrowDown: 40,
  ESC: 27,
};
export default {
  name: "MSCombobox",
  directives: {
    clickoutside,
  },
  props: [
    "value",
    "cls",
    "label",
    "modelValue",
    "url",
    "propValue",
    "propText",
    "isLoadData",
    "placeholder",
    "required",
    "isDisabled",
    "isFocus",
    "firstSelected",
    "showAddButton",
    "iconExtCls",
    "dataInput",
    "allowAllSelect",
  ],
  emits: [
    "onSelected",
    "getValue",
    "update:modelValue",
    "onChangeText",
    "onClickExtButton",
    "onChangeValueSelected",
    "onLoadDataCompleted",
  ],
  methods: {
    /**
     * Lưu lại index của item đã focus
     * NVMANH (31/07/2022)
     */
    saveItemFocus(index) {
      this.indexItemFocus = index;
    },

    /**
     * Ẩn danh sách item
     * NVMANH (31/07/2022)
     */
    hideListData() {
      this.isShowListData = false;
    },

    /**
     * Nhấn vào button thì hiển thị hoặc ẩn List Item
     * NVMANH (31/07/2022)
     */
    btnSelectDataOnClick() {
      this.dataFilter = this.data;
      if (!this.isShowListData) {
        this.showListData();
      } else {
        this.isShowListData = false;
      }
    },

    /**
     * Click chọn item trong danh sách
     * NVMANH (31/07/2022)
     */
    itemOnSelect(item, index) {
      const text = item[this.propText];
      const value = item[this.propValue];
      this.textInput = text; // Hiển thị text lên input.
      this.indexItemSelected = index;
      this.isShowListData = false;
      this.$emit("getValue", value, text, item);
      this.$emit("update:modelValue", value);
      this.$nextTick(function () {
        this.$refs.minput?.focus();
        this.errorText = null;
      });
    },

    /**
     * Nhập text thì thực hiện filter dữ liệu và hiển thị
     * NVMANH (31/07/2022)
     */
    inputOnChange() {
      var me = this;
      // Thực hiện lọc các phần tử phù hợp trong data:
      this.dataFilter = this.data.filter((e) => {
        let text = removeVietnameseTones(me.textInput)
          .toLowerCase()
          .replace(" ", "");
        let textOfItem = removeVietnameseTones(e[me.propText])
          .toLowerCase()
          .replace(" ", "");
        return textOfItem.includes(text);
      });
      this.showListData();
    },

    /**
     * Lựa chọn item bằng cách nhấn các phím lên, xuống trên bàn phím
     * NVMANH (31/07/2022)
     */
    selecItemUpDown() {
      var me = this;
      var keyCodePress = event.keyCode;
      var elToFocus = null;
      switch (keyCodePress) {
        case keyCode.ESC:
          this.isShowListData = false;
          break;
        case keyCode.ArrowDown:
          this.showListData();
          elToFocus = this.$refs[`toFocus_${me.indexItemFocus + 1}`];
          if (
            this.indexItemFocus == null ||
            !elToFocus ||
            elToFocus.length == 0
          ) {
            this.indexItemFocus = 0;
          } else {
            this.indexItemFocus += 1;
          }
          break;
        case keyCode.ArrowUp:
          this.showListData();
          elToFocus = this.$refs[`toFocus_${me.indexItemFocus - 1}`];
          if (
            this.indexItemFocus == null ||
            !elToFocus ||
            elToFocus.length == 0
          ) {
            this.indexItemFocus = this.dataFilter.length - 1;
          } else {
            this.indexItemFocus -= 1;
          }
          break;
        case keyCode.Enter:
          elToFocus = this.$refs[`toFocus_${me.indexItemFocus}`];
          if (elToFocus && elToFocus.length > 0) {
            elToFocus[0].click();
            this.isShowListData = false;
          }
          break;
        default:
          break;
      }
    },
    showListData() {
      this.isShowListData = true;
      this.$nextTick(() => {
        // Xử lý hiển thị data:
        const combobox = this.$refs["mcombobox"].getBoundingClientRect();
        const top = combobox.top;
        const left = combobox.left;
        const height = combobox.height;
        const width = combobox.width;
        const comboboxData = this.$refs["combobox__data"];
        comboboxData.style.top = `${top + 40}px`;
        comboboxData.style.left = `${left}px`;
        comboboxData.style.width = `fit-content`;
        comboboxData.style.minWidth = `${width}px`;
      });
    },
    /**
     * Thực hiện load lại dữ liệu và set item chọn theo tên của item
     * @param {String} text
     */
    async reloadDataAndSetSelect(text) {
      await this.loadData();
      const item = this.dataFilter.find((item) => item[this.propText] === text);
      this.$emit("update:modelValue", item[this.propValue]);
      this.setTextForInput();
      this.$nextTick(function () {
        this.$refs.minput.focus();
        this.errorText = null;
      });
    },
    async loadData() {
      // Thực hiện lấy dữ liệu từ api:
      var me = this;
      if (this.dataInput && Array.isArray(this.dataInput)) {
        this.processData(this.dataInput);
      } else {
        const token = localStorage.getItem("user-token");
        const headers = new Headers();
        var baseUrl = process.env.VUE_APP_BASE_URL;
        var url = this.url;
        if (baseUrl) url = baseUrl + url;
        headers.append("Authorization", token);
        this.showLoadingData = true;
        await fetch(url, { headers })
          .then((res) => res.json())
          .then((res) => {
            this.processData(res);
            this.showLoadingData = false;
          })
          .catch((res) => {
            this.showLoadingData = false;
          });
      }
    },

    /**
     * Xử lý dữ liệu đầu vào
     * @param {Array} data
     */
    processData(data) {
      if (this.allowAllSelect) {
        const allSelectItem = {};
        allSelectItem[this.propText] = "-- Tất cả --";
        allSelectItem[this.propValue] = null;
        this.data = [allSelectItem];
        this.data = this.data.concat(data);
        this.dataFilter = this.data;
      } else {
        this.data = data;
        this.dataFilter = data;
      }

      if (
        this.firstSelected &&
        this.dataFilter.length > 0 &&
        this.modelValue == null
      ) {
        var value = this.dataFilter[0][this.propValue];
        this.$emit("update:modelValue", value);
      }
      this.setTextForInput();
      this.$emit("onLoadDataCompleted");
    },

    /**
     * Thực hiện set text hiển thị tương ứng với item được chọn
     * Author: NVMANH(24/08/2022)
     */
    setTextForInput() {
      var me = this;
      var valueSetSelected = me.modelValue;
      // console.log(this.label, valueSetSelected);
      if (
        valueSetSelected == 0 ||
        valueSetSelected ||
        (this.allowAllSelect && valueSetSelected == null)
      ) {
        for (let index = 0; index < me.data.length; index++) {
          const element = me.data[index];
          const value = element[me.propValue];
          if (value !== undefined && value == valueSetSelected) {
            me.textInput = element[me.propText];
            me.indexItemSelected = index;
            this.$emit("onSelected", value, me.textInput, element);
            break;
          }
        }
      } else {
        this.textInput = null;
        this.isShowClear = false;
        this.indexItemSelected = null;
        this.indexItemFocus = null;
        this.$emit("onSelected", null, null, null);
      }
    },
    onBlur() {
      const text = this.textInput?.trim();
      if (text == undefined) {
        if (this.required == true) {
          this.errorText = `<b>${this.label}</b> không được phép để trống.`;
        } else {
          this.errorText = null;
        }
        return;
      } else {
        const item = this.data.find((item) => item[this.propText] === text);
        if (item == null || item === undefined) {
          // Hiển thị lỗi:
          this.errorText = `<b>${this.label}</b> không có trong hệ thống.`;
        } else {
          this.errorText = null;
          this.$emit("update:modelValue", item[this.propValue]);
          this.setTextForInput();
        }
      }
    },
    clearButtonOnClick() {
      this.textInput = null;
      this.isShowClear = false;
      this.indexItemSelected = null;
      this.indexItemFocus = null;
      this.$emit("update:modelValue", null);
      this.$nextTick(function () {
        this.$refs.minput?.focus();
        this.errorText = null;
      });
    },
    setFocusDefault() {
      if (this.isFocus == true) {
        this.$nextTick(function () {
          this.$refs.minput.focus();
        });
      }
    },
    setFocus() {
      this.$nextTick(function () {
        this.$refs.minput.focus();
      });
    },
    btnExtOnClick() {
      this.$emit("onClickExtButton");
    },
  },
  watch: {
    dataInput: function (newData) {
      this.processData(newData);
    },
    modelValue: function (newValue, oldValue) {
      this.setTextForInput();
      this.$emit("onChangeValueSelected", newValue, oldValue);
    },
    textInput: function (newValue) {
      if (newValue) {
        this.isShowClear = true;
      } else {
        this.isShowClear = false;
      }
      this.$emit("onChangeText", newValue);
    },
    showLoadingData: function (newValue) {
      if (!this.isDisabled && !newValue) this.setFocusDefault();
    },
  },
  created() {
    this.loadData();
  },
  mounted() {},
  data() {
    return {
      data: [], // data gốc
      textInput: null, // Text hiển thị trong input của item được chọn
      dataFilter: [], // data đã được filter
      isShowListData: false, // Hiển thị list data hay không
      indexItemFocus: null, // Index của item focus hiện tại
      indexItemSelected: null, // Index của item được selected
      showLoadingData: false,
      isShowClear: false,
      errorText: null,
    };
  },
};
</script>
<style scoped>
.required {
  color: #ff0000;
}

.combobox {
  position: relative;
  /* flex-direction: row; */
  border-radius: 4px;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  border: 1px solid #ccc;
}

.combobox__input,
select {
  width: 100%;
  height: 36px;
  flex: 1;
  padding-right: 56px !important;
  padding-left: 16px;
  outline: none;
  border: unset;
  box-sizing: border-box;
}

.combobox__input:focus,
.combobox__input:focus ~ .combobox__button {
  /* border-color: #3395ff; */
}

.combobox:has(input:focus) {
  border-color: #3395ff;
}

.combobox-wrapper label + .combobox {
  margin-top: 4px;
}

.combobox-wrapper:has(input[disabled]),
.combobox-wrapper:has(input[disabled]) * {
  cursor: not-allowed;
}

.combobox__button {
  /* position: absolute; */
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgb(90, 90, 90);
  border-radius: 0 4px 4px 0;
  right: 0px;
  top: 0px;
  border: unset;
  border-left: unset;
  height: 36px;
  width: 36px;
  flex-grow: 0;
  flex-shrink: 0;
  flex-basis: 36px;
  background-color: #fff;
  min-width: unset !important;
  outline: none;
  box-sizing: border-box;
  opacity: 0.5;
  cursor: pointer;
}

.combobox__button:hover,
.combobox__button:focus {
  background-color: #e6f2ff;
  color: #000;
}

.combobox__button:disabled {
  pointer-events: none;
}

.combobox__data {
  display: flex;
  flex-direction: column;
  padding: 4px 0px;
  position: fixed;
  width: 100%;
  top: 0;
  right: 0;
  border-radius: 4px;
  background-color: #fff;
  box-shadow: 0px 3px 6px #00000016;
  z-index: 1000;
  max-height: 200px;
  min-width: 200px;
  overflow-y: auto;
}

.combobox__item {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  padding-left: 10px;
  padding: 10px 10px 10px 10px;
  outline: none;
  cursor: pointer;
}

.combobox__item + .combobox__item {
}

.combobox__item-icon {
  width: 16px;
  height: 16px;
  flex-shrink: 0;
  flex-grow: 0;
  flex-basis: 16px;
  font-size: 16px;
  margin-right: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.combobox__item-icon--selected {
  width: 14px;
  height: 11px;
}

.combobox__item:hover,
.combobox__item:focus,
.combobox__item--focus,
.combobox__item--hover {
  background-color: #e6f2ff;
  color: #000;
}

.combobox__item--selected {
  pointer-events: none;
  background-color: #0062cc;
  color: #fff;
}

.combobox__item--disable-select {
  cursor: not-allowed !important;
  pointer-events: none;
  color: #dddddd;
}

.combobox-loading {
  position: absolute;
  top: 0;
  left: 0;
  bottom: 0;
  right: 0;
  background: linear-gradient(-45deg, #b7b7b7, #dddddd, #6d6d6d, #ffffff);
  animation: gradient 4s alternate infinite;
  border-radius: 4px;
  -webkit-animation: gradient 4s alternate infinite;
  -moz-animation: gradient 10s ease infinite;
  background-size: 200%;
  opacity: 0.7;
}

.clear-button {
  display: block;
  position: absolute;
  right: 36px;
  top: 50%;
  transform: translate(-50%, -50%);
  padding: 2px;
  line-height: 12px;
  font-size: 12px;
  text-align: center;
  color: #fff;
  border-radius: 50%;
  background-color: #b1b1b1;
  cursor: pointer;
}

.clear-button:hover,
.clear-button:focus {
  background-color: #999999;
}

.btnComboboxExt {
  width: 36px;
  height: 36px;
  flex-grow: 0;
  flex-shrink: 0;
  flex-basis: 36px;
  border-top: unset !important;
  border-right: unset !important;
  border-bottom: unset !important;
  cursor: pointer;
  background-image: url(../../assets/icon/add-50.png);
  background-position: center;
  background-size: 24px 24px;
  background-repeat: no-repeat;
  background-color: transparent;
  border-left: 1px solid #ccc;
}

.btnComboboxExt:focus,
.btnComboboxExt:hover {
  background-color: #e6f2ff;
}

.combobox--error {
  border: 1px solid #ff0000;
}

.combobox--error-text {
  color: #ff0000;
  font-size: 11px;
}

.right72 {
  right: 72px !important;
}

@keyframes moveGradient {
  50% {
    background-position: 100% 50%;
  }
}

@-webkit-keyframes gradient {
  0% {
    background-position: 0% 50%;
  }

  50% {
    background-position: 100% 50%;
  }

  100% {
    background-position: 0% 50%;
  }
}

@-moz-keyframes gradient {
  0% {
    background-position: 0% 50%;
  }

  50% {
    background-position: 100% 50%;
  }

  100% {
    background-position: 0% 50%;
  }
}

@keyframes gradient {
  0% {
    background-position: 0% 50%;
  }

  50% {
    background-position: 100% 50%;
  }

  100% {
    background-position: 0% 50%;
  }
}
</style>
