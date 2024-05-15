import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import debounce from "./scripts/debounce";
import api from "./utils/api";
import maxios from "./utils/axios";
import Enum from "./scripts/enum";
import MPage from "./components/layout/ThePage.vue";
import MTableLayout from "./components/base/MTableLayout.vue";
import MInput from "./components/base/MInput.vue";
import MInputSearch from "./components/base/MInputSearch.vue";
import MCheckbox from "./components/base/MCheckbox.vue";
import MButton from "./components/base/MButton.vue";
import MButtonIcon from "./components/base/MButtonIcon.vue";
import MButtonFontIcon from "./components/base/MButtonFontIcon.vue";
import MTextArea from "./components/base/MTextArea.vue";
import MCombobox from "./components/base/MCombobox.vue";
import MDialog from "./components/base/MDialog.vue";
import MDialogDragable from "./components/base/dialog/MDialogDragable.vue";
import MLoading from "./components/base/MLoading.vue";
import MPaging from "./components/base/MPaging.vue";
import MToast from "./components/base/MToast.vue";
import "element-plus/dist/index.css";
import "element-plus/es/components/loading/style/css";
import "element-plus/es/components/date-picker/style/css";
import "element-plus/es/components/tag/style/css";
import vi from "element-plus/dist/locale/vi.mjs";
import ElementPlus from "element-plus";
import CKEditor from "@ckeditor/ckeditor5-vue";
import MEditor from './components/base/meditor/MEditorFull.vue'
import { ElTable, ElTableColumn } from "element-plus";

import print from "./utils/print";
import commonJs from "./scripts/common";
import timerange from "./scripts/timerange";
import tinyEmitter from "tiny-emitter/instance";

import "./scripts/dialogdragable.js"
window.$ = window.jQuery = require('jquery');

// store.commit("CREATE_CONNECTION");

const app = createApp(App);
app.component("MPage", MPage);
app.component("MTableLayout", MTableLayout);
app.component("MInput", MInput);
app.component("MCheckbox", MCheckbox);
app.component("MInputSearch", MInputSearch);
app.component("MButton", MButton);
app.component("MTextArea", MTextArea);
app.component("MCombobox", MCombobox);
app.component("MButtonIcon", MButtonIcon);
app.component("MButtonFontIcon", MButtonFontIcon);
app.component("MDialog", MDialog);
app.component("MToast", MToast);
app.component("MLoading", MLoading);
app.component("MPaging", MPaging);
app.component("MTable", ElTable);
app.component("MColumn", ElTableColumn);
app.component("MDialogDragable", MDialogDragable);
app.component("MEditor", MEditor);


app.config.globalProperties.commonJs = commonJs;
app.config.globalProperties.timeRange = timerange;
app.config.globalProperties.debounce = debounce;
app.config.globalProperties.api = api;
app.config.globalProperties.maxios = maxios;
app.config.globalProperties.$emitter = tinyEmitter;
app.config.globalProperties.Enum = Enum;
app.config.globalProperties.hubConnection = commonJs.createHubConnection();
app.config.unwrapInjectedRef = true;
// app.config.globalProperties.hubConnection = store.getters.hubConnection;

document.addEventListener("DOMContentLoaded", function () {
  var elements = document.getElementsByTagName("INPUT");
  for (var i = 0; i < elements.length; i++) {
    elements[i].oninvalid = function (e) {
      e.target.setCustomValidity("");
      if (!e.target.validity.valid) {
        e.target.setCustomValidity("Thông tin này không được phép để trống");
      }
    };
    elements[i].oninput = function (e) {
      e.target.setCustomValidity("");
    };
  }
});

// Đoạn này xử lý cảnh báo ------------------------------------------------
var passiveEvent = false;
try {
  var opts = Object.defineProperty({}, "passive", {
    get: function () {
      passiveEvent = true;
      return passiveEvent;
    },
  });
  window.addEventListener("test", null, opts);
} catch (e) {
  console.log(e);
}
// in my case I need both passive and capture set to true, change as you need it.
passiveEvent = passiveEvent ? { capture: true, passive: true } : true;

document.addEventListener("touchstart", null, { passive: true });
//if you need to handle mouse wheel scroll
// var supportedWheelEvent: string = "onwheel" in HTMLDivElement.prototype ? "wheel" :
//     document.onmousewheel !== undefined ? "mousewheel" : "DOMMouseScroll";
//-----------------------------------------------------------------------------

app.use(ElementPlus, {
  locale: vi,
});
app.use(store).use(router).use(CKEditor).use(print);
app.mount("#app");
