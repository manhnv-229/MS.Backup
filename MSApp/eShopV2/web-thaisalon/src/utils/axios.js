import apiCall from "./api";

const maxios = {
    get: (url, callback) => {
        return apiCall({ url: url, method: "GET", callback: callback });
    },
    post: (url, data, callback) => {
        return apiCall({ url: url, data: data, method: "POST", callback: callback });
    },
    put: (url, data, callback) => {
        return apiCall({ url: url, data: data, method: "PUT", callback: callback });
    },
    patch: (url, data, callback) => {
        return apiCall({ url: url, data: data, method: "PATCH", callback: callback });
    },
    delete: (url, callback) => {
        return apiCall({ url: url, method: "DELETE", callback: callback });
    },
}
export default maxios;