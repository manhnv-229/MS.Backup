import axios from "axios";
import router from "../router";
import store from "../store";
import { AUTH_LOGOUT } from "../store/actions/auth";
import commonJs from "../scripts/common";
import MISAEnum from "../scripts/enum";
import.meta.env

const apiCall = ({ url, data, method, showToast, showMsg, callback,hideLoading }) =>
    new Promise((resolve, reject) => {
        try {
            axios.defaults.baseURL = `${VUE_APP_BASE_URL}`;
            var token = localStorage.getItem("user-token");
            var connectionId = localStorage.getItem("connectionId");
            var clientHost =  location.host;
            if (token)
                axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
                axios.defaults.headers.common["connectionId"] = `${connectionId}`;
                axios.defaults.headers.common["clientHost"] = `${clientHost}`;
            if (showMsg == undefined) {
                showMsg = true;
            }
            if (!hideLoading) {
                commonJs.showLoading();
            }
            axios({
                    method: method || "GET",
                    url: url,
                    data: data,
                })
                .then((res) => {
                    resolve(res.data);
                    if (method && method != "GET") {
                        if (showToast == undefined) {
                            showToast = true;
                        }
                        if (window.location.pathname != "/login" && showToast)
                            commonJs.showToast(`Thành công!`, MISAEnum.MsgType.Success);
                    }
                    if (callback) {
                        callback();
                    }
                    commonJs.hideLoading();
                })
                .catch((res) => {
                    try {
                        if (!res.response) {
                            console.log(res);
                            commonJs.showMessenger({
                                title: "Lỗi",
                                msg: "Không thể xử lý yêu cầu này, vui lòng liên hệ QTV để được trợ giúp.",
                                type: MISAEnum.MsgType.Error,
                                confirm: callback,
                            });
                        } else {
                            var statusCode = res.response.status;
                            if (showMsg)
                                if (statusCode == 0) {
                                    commonJs.showMessenger({
                                        title: "Lỗi kết nối",
                                        msg: "Không thể kết nối đến máy chủ, vui lòng thử lại sau",
                                        type: MISAEnum.MsgType.Error,
                                        confirm: callback,
                                    });
                                }
                            if (statusCode == 400) {
                                var response = {
                                    userMsg: res.response.data.UserMsg,
                                    errors: res.response.data.errors,
                                    status: 400,
                                };
                                if (response.errors && response.errors.ValidErrors) {
                                    commonJs.showErrorMessenger(
                                        "Dữ liệu không hợp lệ.",
                                        response.errors.ValidErrors,
                                        callback
                                    );
                                } else {
                                    var error = response.errors;
                                    var errorsMsg = [];
                                    for (const key in error) {
                                        if (Object.hasOwnProperty.call(error, key)) {
                                            const errorsArray = error[key];
                                            if (errorsArray) {
                                                for (const msg of errorsArray) {
                                                    errorsMsg.push(msg);
                                                }
                                            }
                                        }
                                    }
                                    commonJs.showErrorMessenger(
                                        "Dữ liệu không hợp lệ.",
                                        errorsMsg,
                                        callback
                                    );
                                }
                                reject(response);
                            } else {
                                if (statusCode == 500) {
                                    res.devMsg = res.message;
                                    res.message = res.response.data.UserMsg;
                                    res.message =
                                        res.message ||
                                        "Có lỗi phía máy chủ, vui lòng liên hệ Quản trị viên để được trợ giúp.";
                                    console.log(res);
                                    commonJs.showErrorMessenger("Lỗi máy chủ", res.message);
                                }

                                if (statusCode == 524) {
                                    res.message =
                                        res.message ||
                                        "Quá trình xử lý cần nhiều thời gian và sẽ sớm hoàn thành. Bạn có thể làm việc khác trong khi quá trình xử lý hoàn tất.";
                                    commonJs.showMessenger({
                                        title: "",
                                        msg: res.message,
                                        type: MISAEnum.MsgType.Info,
                                    });
                                }
                                if (statusCode == 401) {
                                    if (res.config.url.includes("/login")) {
                                        res.message =
                                            "Tên tài khoản hoặc mật khẩu không đúng, vui lòng kiểm tra lại.";
                                        commonJs.showErrorMessenger(
                                            "Sai thông tin đăng nhập",
                                            res.message
                                        );
                                    } else if(location.pathname != '/login') {
                                        res.message =
                                            "Phiên làm việc đã hết hạn, bạn cần đăng nhập để sử dụng tính năng này.";
                                        commonJs.showMessenger({
                                            title: "",
                                            msg: res.message,
                                            type: MISAEnum.MsgType.Info,
                                            confirm: function() {
                                                store.dispatch(AUTH_LOGOUT).then(() => {
                                                    router.push("/login");
                                                });
                                            },
                                        });
                                    }
                                }
                                if (statusCode == 403) {
                                    res.message = res.response?.data?.UserMsg;
                                    if (!res.message)
                                        res.message =
                                        "Bạn bị giới hạn quyền truy cập tài nguyên, vui lòng liên hệ quản trị viên để được trợ giúp.";
                                    commonJs.showErrorMessenger(
                                        "Yêu cầu bị từ chối.",
                                        res.message
                                    );
                                }
                                if (statusCode == 404) {
                                    res.message =
                                        "Hệ thống không tìm thấy dịch vụ cung cấp cho bạn, vui lòng quay lại sau hoặc liên hệ quản trị để được trợ giúp!";
                                    commonJs.showErrorMessenger("Lỗi dịch vụ", res.message);
                                }
                            }
                        }
                        reject(res.response);
                        commonJs.hideLoading();
                    } catch (error) {
                        console.log(`error`, error);
                        reject(res.response);
                        commonJs.hideLoading();
                    }
                });
        } catch (err) {
            reject(new Error(err));
            commonJs.hideLoading();
        }
    });

export default apiCall;