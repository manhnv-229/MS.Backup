/* eslint-disable */
import store from "../store";
import router from "../router";
import MISAEnum from "./enum";
import { globalProps } from '@/main.js';
import { AUTH_REQUEST } from "../store/actions/auth";
import { REFRESH_INVENTORY_DATA } from "../store/actions/entity";
import {
  CLEAR_ERROR_MSG,
  SET_ERROR_MSG,
} from "../store/actions/notification.js";
import { SHOW_LOADING, HIDE_LOADING } from "../store/actions/loading.js";
import { CLEAR_TOAST, SET_TOAST } from "../store/actions/toast.js";
import {
  SET_CONNECTING_HUB,
  SET_NEW_HUB_CONNECTION,
} from "../store/actions/signalR";
import { HIDE_PROGRESS, SHOW_PROGRESS } from "../store/actions/progressbar";
import { createHubConnection } from "./singleR";
const commonJs = {
  login: function (username, password) {
    commonJs.showLoading();
    store
      .dispatch(AUTH_REQUEST, { username, password })
      .then(() => {
        console.log(`login success`);
        console.log("create signalR!");
        globalProps.hubConnection = createHubConnection();
        router.push("/refs");
        commonJs.hideLoading();
      })
      .catch((res) => {
        var msg = null;
        if (res.status == 403) {
          msg =
            "Tài khoản của bạn chưa được cấp quyền truy cập tài nguyên hiện tại. Vui lòng liên hệ Mr Mạnh để được cấp quyền.";
          commonJs.showMessenger({
            title: "Truy cập bị từ chối",
            msg: msg,
            type: MISAEnum.MsgType.Error,
            confirm: () => {
              router.push("/reports");
            },
          });
        }
        commonJs.hideLoading();
      });
  },

  /** -----------------------------------------
   * Kiểm tra dữ liệu bắt buộc
   * @param {any} value
   * @returns true - hợp lệ; false - không hợp lệ.
   * Author: NVMANH (12/12/2022)
   */
  checkRequired(value) {
    try {
      if (value == "" || value == null || value == undefined) {
        return false;
      }
      return true;
    } catch (error) {
      console.log(error);
    }
  },

  /** -----------------------------------------
   * Kiểm tra email đã đúng định dạng hay chưa?
   * @param {String} email Địa chỉ email
   * @returns true - hợp lệ; false - không hợp lệ.
   * Author: NVMANH (12/12/2022)
   */
  checkEmailFormat(email) {
    try {
      if (!email) {
        return true;
      }
      let regex = new RegExp("[a-z0-9]+@[a-z]+.[a-z]{2,3}");
      return regex.test(email);
    } catch (error) {
      console.log(error);
    }
  },
  checkDateIsToday(date) {
    try {
      date = new Date(date);
      let today = new Date();
      if (date.toDateString() === today.toDateString()) {
        return true;
      } else {
        return false;
      }
    } catch (error) {
      return false;
    }
  },

  /**-----------------------------------------
   * Loại bỏ dấu tiếng việt -> không dấu
   * @param {*} alias
   * @returns
   */
  change_alias: function (alias) {
    var str = alias;
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(
      /!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g,
      " "
    );
    str = str.replace(/ + /g, " ");
    str = str.trim();
    return str;
  },
  /**
   * Thực hiện định dạng dữ liệu ngày tháng -> ngày/tháng/năm
   * @param {*} value
   * @returns Chuỗi dạng ngày/tháng/năm
   * Author: NVMANH (2020)
   */
  formatDate: function (value) {
    try {
      if (!value) return "";
      value = new Date(value);
      var date = value.getDate();
      date = date < 10 ? `0${date}` : date;
      var month = value.getMonth() + 1;
      month = month < 10 ? `0${month}` : month;
      var year = value.getFullYear();
      return `${date}/${month}/${year}`;
    } catch (error) {
      return "";
    }
  },

  /**
   * Định dạng ngày tháng
   * @param {*} value
   * @returns Chuỗi dạng: Ngày 22 tháng 09 năm 1989
   * Author: NVMANH (2022)
   */
  formatDateWithStringVN: function (value) {
    try {
      if (!value) return "";
      value = new Date(value);
      var date = value.getDate();
      date = date < 10 ? `0${date}` : date;
      var month = value.getMonth() + 1;
      month = month < 10 ? `0${month}` : month;
      var year = value.getFullYear();
      return `Ngày ${date} tháng ${month} năm ${year}`;
    } catch (error) {
      return "";
    }
  },

  /**
   * Định dạng ngày tháng
   * @param {*} value
   * @returns Chuỗi dạng:  22/09/1989 [xuống dòng] 12:24:2024
   * Author: NVMANH (2022)
   */
  formatDateTime: function (value) {
    try {
      if (!value) return "";
      value = new Date(value);
      var date = value.getDate();
      date = date < 10 ? `0${date}` : date;
      var month = value.getMonth() + 1;
      month = month < 10 ? `0${month}` : month;
      var year = value.getFullYear();

      var hour = commonJs.addZeroPrefix(value.getHours());
      var min = commonJs.addZeroPrefix(value.getMinutes());
      var second = commonJs.addZeroPrefix(value.getSeconds());
      return `${date}/${month}/${year} \n
                        ${hour}:${min}:${second}`;
    } catch (error) {
      return "";
    }
  },
  /**
   * Định dạng ngày tháng
   * @param {*} value
   * @returns Chuỗi HTML dạng ->  22/09/1989 [xuống dòng] 12:24:2024
   * Author: NVMANH (2022)
   */
  formatDateTimeHTML: function (value) {
    try {
      if (!value) return "";
      value = new Date(value);
      var date = value.getDate();
      date = date < 10 ? `0${date}` : date;
      var month = value.getMonth() + 1;
      month = month < 10 ? `0${month}` : month;
      var year = value.getFullYear();
      var hour = commonJs.addZeroPrefix(value.getHours());
      var min = commonJs.addZeroPrefix(value.getMinutes());
      var second = commonJs.addZeroPrefix(value.getSeconds());
      let html = `<div class="flex--column align--center"><div>${date}/${month}/${year}</div><div>(${hour}:${min}:${second})</div><div>`;
      return html;
    } catch (error) {
      return "";
    }
  },
  /**
   * Định dạng ngày tháng
   * @param {*} value
   * @returns Chuỗi HTML dạng ->  22/09/1989 12:24:2024
   * Author: NVMANH (2022)
   */
  formatDateTimeHTMLInline: function (value) {
    try {
      if (!value) return "";
      value = new Date(value);
      var date = value.getDate();
      date = date < 10 ? `0${date}` : date;
      var month = value.getMonth() + 1;
      month = month < 10 ? `0${month}` : month;
      var year = value.getFullYear();
      var hour = commonJs.addZeroPrefix(value.getHours());
      var min = commonJs.addZeroPrefix(value.getMinutes());
      var second = commonJs.addZeroPrefix(value.getSeconds());
      let html = `<div class="flex--row justify-content--end gap-6"><div>${date}/${month}/${year}</div><div> (${hour}:${min}:${second})</div><div>`;
      return html;
    } catch (error) {
      return "";
    }
  },
  /**
   * Định dạng ngày tháng
   * @param {*} value
   * @returns Chuỗi dạng ->  22/09/1989 12:24:2024
   * Author: NVMANH (2022)
   */
  formatDateTimeYYMMDDHHMMSS: function (value) {
    try {
      if (!value) return "";
      value = new Date(value);
      var date = value.getDate();
      date = date < 10 ? `0${date}` : date;
      var month = value.getMonth() + 1;
      month = month < 10 ? `0${month}` : month;
      var year = value.getFullYear();

      var hour = commonJs.addZeroPrefix(value.getHours());
      var min = commonJs.addZeroPrefix(value.getMinutes());
      var second = commonJs.addZeroPrefix(value.getSeconds());
      return `${year}-${month}-${date} ${hour}:${min}:${second}`;
    } catch (error) {
      return "";
    }
  },
  /**
   * Định dạng ngày tháng
   * @param {*} value
   * @returns Chuỗi dạng ->  22/09/1989 12:24:2024
   * Author: NVMANH (2022)
   */
  formatDateTimeDDMMYYHHMMSS: function (value) {
    try {
      if (!value) return "";
      value = new Date(value);
      var date = value.getDate();
      date = date < 10 ? `0${date}` : date;
      var month = value.getMonth() + 1;
      month = month < 10 ? `0${month}` : month;
      var year = value.getFullYear();

      var hour = commonJs.addZeroPrefix(value.getHours());
      var min = commonJs.addZeroPrefix(value.getMinutes());
      var second = commonJs.addZeroPrefix(value.getSeconds());
      return `${date}/${month}/${year} ${hour}:${min}:${second}`;
    } catch (error) {
      return "";
    }
  },
  addZeroPrefix(number) {
    try {
      if (number == null || number == undefined) return "";
      return (number = number < 10 ? `0${number}` : number);
    } catch (error) {
      return "";
    }
  },
  getTime: function (value) {
    try {
      if (!value) return "";
      value = new Date(value);
      var date = value.getDate();
      date = date < 10 ? `0${date}` : date;
      var month = value.getMonth() + 1;
      month = month < 10 ? `0${month}` : month;
      var year = value.getFullYear();

      var hour = value.getHours();
      var datePart = "sáng";
      if (hour >= 18) {
        datePart = "tối";
      } else if (hour <= 18 && hour > 12) {
        datePart = "chiều";
      }
      hour = commonJs.addZeroPrefix(hour);
      var min = commonJs.addZeroPrefix(value.getMinutes());
      var second = commonJs.addZeroPrefix(value.getSeconds());

      return `${hour}:${min} (${datePart})`;
    } catch (error) {
      return "";
    }
  },
  /**
   * Thực hiện định dạng hiển thị tiền tệ dạng VNĐ
   * @param {} record
   * @param {*} row
   * @param {*} value
   * @returns
   */
  formatMoney: function (value) {
    if (!value) return "0 đ";
    try {
      value = new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
      }).format(value);
      return value;
    } catch (error) {
      return "";
    }
  },
  /**
   * Hiển thị Toast Messenger
   * @param {String} msg
   * @param {*} type Loại Toast hiển thị
   * Author: NVMANH (02/09/2022)
   */
  showToast(msg, type) {
    if (!type) type = Enum.MsgType.Info;

    store.dispatch(SET_TOAST, { msg: msg, type: type });
    setTimeout(function () {
      store.dispatch(CLEAR_TOAST, {});
    }, 5000);
  },
  showProgress(progressInfo) {
    store.dispatch(SHOW_PROGRESS, progressInfo);
  },
  hideProgress() {
    store.dispatch(HIDE_PROGRESS);
  },
  /**
   * Hiển thị cảnh báo
   * Authror: NVMANH (16/08/2022)
   */
  showErrorMessenger() {
    var payload = arguments;
    var title = null;
    var msg = [];
    var callback = null;
    if (payload.length == 0) {
      title = "Thông báo";
      msg.push("Có lỗi xảy ra, vui lòng liên hệ MISA để được trợ giúp.");
    } else if (payload.length == 1) {
      msg.push(payload[0]);
    } else {
      title = payload[0].toString();
      for (let index = 1; index < payload.length; index++) {
        const item = payload[index];
        if (
          item &&
          (typeof item === "string" ||
            item instanceof String ||
            typeof item == "number")
        ) {
          msg.push(item);
        } else if (Array.isArray(item)) {
          for (const itemChild of item) {
            if (
              typeof itemChild === "string" ||
              itemChild instanceof String ||
              typeof itemChild == "number"
            ) {
              msg.push(itemChild);
            }
          }
        } else {
          if (typeof item == "function") {
            callback = item;
          }
        }
      }
    }
    store.dispatch(SET_ERROR_MSG, {
      title: title,
      msg: msg,
      type: MISAEnum.MsgType.Error,
      confirm: callback,
    });
  },
  /**
   * Kiểm tra xem đối tượng có tồn tại trong mảng hay không theo giá trị của property
   * @param {Array} array
   * @param {Object} object
   * @param {String} propName
   * Authro: NVMANH (30/11/2022)
   */
  checkObjectExistInArray(array, object, propName) {
    return array.some((obj) => {
      if (object[propName] == obj[propName]) {
        return true;
      }
      return false;
    });
  },

  getObjectMaxValueInArray(array, propName) {
    var maxObject = array.reduce(function (object1, object2) {
      return (object1[propName] || 0) > (object2[propName] || 0)
        ? object1
        : object2;
    }, array[0]);
    return maxObject;
  },

  /**
   * Kiểm tra xem đối tượng có tồn tại trong mảng hay không theo giá trị của property
   * @param {Array} array
   * @param {Object} object
   * @param {String} propName
   * Authro: NVMANH (30/11/2022)
   */
  getObjectExistInArray(array, object, propName) {
    var index = array.findIndex((obj) => {
      if (object[propName] == obj[propName]) {
        return true;
      }
      return false;
    });
    if (index >= 0) {
      return array[index];
    } else {
      return null;
    }
  },
  /**
   * Ẩn cảnh báo
   * Authror: NVMANH (16/08/2022)
   */
  hideErrorMessenger() {
    store.dispatch(CLEAR_ERROR_MSG);
  },

  showConfirm(msg, callback) {
    commonJs.showMessenger({
      title: "Xác nhận",
      msg: msg || "Bạn có chắc chắn muốn thực hiện hành động này?",
      type: MISAEnum.MsgType.Question,
      confirm: callback,
      showCancelButton: true,
    });
  },
  hideConfirm() {
    commonJs.hideMessenger();
  },

  showMessenger({ title, msg, type, confirm, showCancelButton }) {
    store.dispatch(SET_ERROR_MSG, {
      title: title || "Thông báo",
      msg: msg || "Có lỗi xảy ra",
      type: type || MISAEnum.MsgType.Info,
      confirm: confirm,
      showCancelButton: showCancelButton,
    });
  },

  hideMessenger() {
    store.dispatch(CLEAR_ERROR_MSG);
  },

  showLoading() {
    store.dispatch(SHOW_LOADING);
  },
  showLoadingEl(el) {
    el.classlist.add("loading");
  },
  hideLoading() {
    store.dispatch(HIDE_LOADING);
  },
  showConnectingHub() {
    store.dispatch(SET_CONNECTING_HUB, true);
  },
  hideConnectingHub() {
    store.dispatch(SET_CONNECTING_HUB, false);
  },
  getPreviousDay(date) {
    const previous = new Date(date.getTime());
    previous.setDate(date.getDate() - 1);

    return previous;
  },
  /**
   * Lấy khoảng thời gian theo ngày (từ đầu tới cuối ngày)
   * @param {date} date (Ngày)
   * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
   * Author: NVMANH (02/08/2019)
   */
  getRangeDateTimeInDay(date) {
    var startDateTime = date.setHours(0, 0, 0, 0);
    var endDateTime = date.setHours(23, 59, 59, 999);
    return {
      StartDateTime: new Date(startDateTime),
      EndDateTime: new Date(endDateTime),
    };
  },

  /**
   * Lấy khoảng thời gian theo ngày (từ đầu tới cuối ngày)
   * @param {date} date (Ngày)
   * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
   * Author: NVMANH (02/08/2019)
   */
  getRangeDateTimeInYesterday(date) {
    date = this.getPreviousDay(date);
    var startDateTime = date.setHours(0, 0, 0, 0);
    var endDateTime = date.setHours(23, 59, 59, 999);
    return {
      StartDateTime: new Date(startDateTime),
      EndDateTime: new Date(endDateTime),
    };
  },

  /**
   * Lấy khoảng thời gian theo tuần của một ngày bất kỳ (từ đầu tới cuối tuần)
   * @param {any} date (Ngày)
   * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
   * Author: NVMANH (02/08/2019)
   */
  getRangeDateTimeWeekOfDate(date) {
    // Lấy ngày đầu tuần (thứ 2):
    var first = date.getDate() - (date.getDay() || 5 + 1); // First day is the day of the month - the day of the week
    var monday = new Date(date.setDate(first));
    var mondayForChange = new Date(date.setDate(first));
    var lastWeek = monday.getDate() + 6;
    var sunday = new Date(mondayForChange.setDate(lastWeek));
    // var sunday = monday.addDays(6);
    return {
      StartDateTime: new Date(monday.setHours(0, 0, 0, 0)),
      EndDateTime: new Date(sunday.setHours(23, 59, 59, 999)),
    };
  },

  /**
   * Lấy khoảng thời gian theo tuần của một ngày bất kỳ (từ đầu tới cuối tuần)
   * @param {any} date (Ngày)
   * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
   * Author: NVMANH (02/08/2019)
   */
  getRangeDateTimeMonthOfDate(date) {
    var startDateTime = new Date(date.setDate(1));
    var endDateTime = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    return {
      StartDateTime: new Date(startDateTime.setHours(0, 0, 0, 0)),
      EndDateTime: new Date(endDateTime.setHours(23, 59, 59, 999)),
    };
  },

  /**
   * Lấy khoảng thời gian theo tuần của một ngày bất kỳ (từ đầu tới cuối tuần)
   * @param {any} date (Ngày)
   * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
   * Author: NVMANH (02/08/2019)
   */
  getRangeDateTimeQuarterOfDate(date) {
    var thisQuater = Math.ceil((date.getMonth() + 1) / 3);
    var startMonth = thisQuater * 3 - 3;
    var endMonth = thisQuater * 3 - 1;
    var startDateTime = new Date(date.getFullYear(), startMonth, 1);
    var endDateTime = new Date(date.getFullYear(), endMonth + 1, 0);
    return {
      StartDateTime: new Date(startDateTime.setHours(0, 0, 0, 0)),
      EndDateTime: new Date(endDateTime.setHours(23, 59, 59, 999)),
    };
  },
  /**
   * Lấy khoảng thời gian theo tuần của một ngày bất kỳ (từ đầu tới cuối tuần)
   * @param {any} date (Ngày)
   * @returns {object} Đối tượng bao gồm: Ngày bắt đầu và ngày kết thúc.
   * Author: NVMANH (02/08/2019)
   */
  getRangeDateTimeYearOfDate(date) {
    var currentYear = date.getFullYear();
    var startDateTime = new Date(currentYear, 0, 1);
    var endDateTime = new Date(currentYear, 12, 0);
    return {
      StartDateTime: new Date(startDateTime.setHours(0, 0, 0, 0)),
      EndDateTime: new Date(endDateTime.setHours(23, 59, 59, 999)),
    };
  },
  mobileCheck() {
    let check = false;
    (function (a) {
      if (
        /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(
          a
        ) ||
        /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(
          a.substr(0, 4)
        )
      )
        check = true;
    })(navigator.userAgent || navigator.vendor || window.opera);
    return check;
  },
  tabletCheck() {
    let check = false;
    (function (a) {
      if (
        /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(
          a
        ) ||
        /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(
          a.substr(0, 4)
        )
      )
        check = true;
    })(navigator.userAgent || navigator.vendor || window.opera);
    return check;
  },
  mapObjectPropertiesValue(source, target) {
    if (
      typeof source === "object" &&
      source !== null &&
      typeof target === "object" &&
      target !== null
    ) {
      // Biến là một đối tượng
      for (const key in source) {
        if (Object.hasOwnProperty.call(source, key)) {
          target[key] = source[key];
        }
      }
    } 
  },
};
export default commonJs;
