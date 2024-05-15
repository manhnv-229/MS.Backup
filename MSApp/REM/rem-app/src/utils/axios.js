import apiCall from "./api";

const maxios = {
    get: (url, callback, hideLoading) => {
        return apiCall({ url: url, method: "GET", callback: callback, hideLoading });
    },
    post: (url, data, callback, hideLoading) => {
        return apiCall({ url: url, data: data, method: "POST", callback: callback, hideLoading });
    },
    put: (url, data, callback, hideLoading) => {
        return apiCall({ url: url, data: data, method: "PUT", callback: callback, hideLoading });
    },
    patch: (url, data, callback, hideLoading) => {
        return apiCall({ url: url, data: data, method: "PATCH", callback: callback, hideLoading });
    },
    delete: (url, callback, hideLoading) => {
        return apiCall({ url: url, method: "DELETE", callback: callback, hideLoading });
    },
}
export default maxios;