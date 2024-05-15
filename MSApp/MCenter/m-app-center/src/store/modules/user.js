import { USER_REQUEST, USER_ERROR, USER_SUCCESS } from "../actions/user";
import apiCall from "../../utils/api";
// import { createApp } from 'vue'
import { AUTH_LOGOUT } from "../actions/auth";
// const app = createApp({});
import router from "../../router";
import store from "..";
// import { SET_NEW_HUB_CONNECTION } from "../actions/signalR";
import { UPDATE_ORGANIZATION_INFO } from "../actions/signalR";
import { GET_ALL_INVENTORY, GET_ALL_CUSTOMER, GET_ALL_EMPLOYEE,GET_ALL_PAYMENT_TYPES, GET_ALL_SLOT, GET_ALL_SLOT_GROUP, GET_ALL_BRANCH,GET_ALL_SERVICE } from "../actions/entity";
const state = {
    status: "",
    role: null,
    isReload: true,
    profile: {}
};

const getters = {
    getProfile: state => state['profile'],
    isProfileLoaded: state => !!state.profile.UserName,
    role: state => {
        if (state.isReload)
            return localStorage.getItem("userRoleValue");
        else
            return state.role;
    }
};

const actions = {
    /**
     * Lấy thông tin của user
     * @param {*} param0 
     */
    [USER_REQUEST]: ({ commit, dispatch }) => {
        return new Promise((resolve, reject) => {
            commit(USER_REQUEST);
            var userId = localStorage.getItem('user-id');
            apiCall({ url: `/accounts/${userId}`, method: "GET" })
                .then(resp => {
                    var roles = resp.Roles;
                    if (roles && roles.length > 0) {
                        localStorage.setItem("userRoleValue", roles[0].RoleValue);
                        resp.RoleValue = roles[0].RoleValue;
                    }
                    localStorage.setItem("userName", resp.UserName);
                    localStorage.setItem("avatar", resp.AvatarFullPath);
                    localStorage.setItem("firstName", resp.firstName);
                    localStorage.setItem("lastName", resp.LastName);
                    localStorage.setItem("fullName", resp.FullName);
                    localStorage.setItem("employeeId", resp.EmployeeId);
                    store.dispatch(UPDATE_ORGANIZATION_INFO);
                    // store.dispatch(GET_ALL_INVENTORY);
                    // store.dispatch(GET_ALL_CUSTOMER);
                    // store.dispatch(GET_ALL_EMPLOYEE);
                    // store.dispatch(GET_ALL_PAYMENT_TYPES);
                    // store.dispatch(GET_ALL_SLOT);
                    // store.dispatch(GET_ALL_SLOT_GROUP);
                    // store.dispatch(GET_ALL_BRANCH);
                    // store.dispatch(GET_ALL_SERVICE);
                    commit(USER_SUCCESS, resp);
                    resolve(resp);
                })
                .catch((res) => {
                    console.log(`Lỗi USER REQUEST`, res);
                    commit(USER_ERROR);
                    // if resp is unauthorized, logout, to
                    dispatch(AUTH_LOGOUT);
                    reject(res);
                });
        })

    }
};

const mutations = {
    [USER_REQUEST]: state => {
        state.status = "loading";
    },
    [USER_SUCCESS]: (state, resp) => {
        state.status = "success";
        state['profile'] = resp;
        state.role = resp.RoleValue;
        state['isReload'] = false;
        // localStorage.setItem("userRole", resp.RoleValue);
        // app.$set(state, "profile", resp);

    },
    [USER_ERROR]: state => {
        state.status = "error";
    },
    [AUTH_LOGOUT]: state => {
        state.profile = {};
        state.role = null;
        state.isReload = true;
        router.push("/login");
    }
};

export default {
    state,
    getters,
    actions,
    mutations
};