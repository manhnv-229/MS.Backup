import {
  AUTH_REQUEST,
  AUTH_ERROR,
  AUTH_SUCCESS,
  AUTH_LOGOUT,
} from "../actions/auth";
import { USER_REQUEST } from "../actions/user";
import apiCall from "../../utils/api";
import axios from "axios";
// import { DISCONECT_HUB } from "../actions/signalR";
const state = {
  token: localStorage.getItem("user-token") || "",
  status: "",
  hasLoadedOnce: false,
};

const getters = {
  isAuthenticated: (state) => !!state.token,
  authStatus: (state) => state.status,
};

const actions = {
  [AUTH_REQUEST]: ({ commit, dispatch }, user) => {
    return new Promise((resolve, reject) => {
      commit(AUTH_REQUEST);
      apiCall({ url: "/Accounts/login", data: user, method: "POST" })
        .then((resp) => {
          // Lữu trữ Token và user id trong local storage:
          localStorage.setItem("user-token", resp.Token);
          localStorage.setItem("user-id", resp.UserId);
          // Here set the header of your ajax library to the token value.
          // example with axios
          axios.defaults.headers.common[
            "Authorization"
          ] = `Bearer ${resp.Token}`;
          commit(AUTH_SUCCESS, resp);
          dispatch(USER_REQUEST)
            .then(() => {
              resolve(resp);
            })
            .catch((res) => {
              reject(res);
            });
        })
        .catch((err) => {
          commit(AUTH_ERROR, err);
          console.log("xóa local storage do EXCEPTION!");
          localStorage.removeItem("user-token");
          reject(err);
        });
    });
  },
  [AUTH_LOGOUT]: ({ commit }) => {
    return new Promise((resolve) => {
      var userName = localStorage.getItem("userName");
      if (userName) {
        apiCall({
          url: `/accounts/revoke/${userName}`,
          data: null,
          method: "POST",
        })
          .then(() => {
            console.log("logout success!");
            localStorage.clear();
            // dispatch(DISCONECT_HUB);
          })
          .catch(() => {});
      }
      commit(AUTH_LOGOUT);
      localStorage.removeItem("user-token");
      localStorage.removeItem("user-id");
      localStorage.removeItem("userName");
      resolve();
    });
  },
};

const mutations = {
  [AUTH_REQUEST]: (state) => {
    state.status = "loading";
  },
  [AUTH_SUCCESS]: (state, resp) => {
    state.status = "success";
    state.token = resp.Token;
    state.hasLoadedOnce = true;
  },
  [AUTH_ERROR]: (state) => {
    state.status = "error";
    state.hasLoadedOnce = true;
  },
  [AUTH_LOGOUT]: (state) => {
    state.token = "";
  },
};

export default {
  state,
  getters,
  actions,
  mutations,
};
