import {
  GET_ALL_INVENTORY,
  GET_ALL_CUSTOMER,
  GET_ALL_EMPLOYEE,
  GET_ALL_PAYMENT_TYPES,
  REFRESH_INVENTORY_DATA,
  GET_ALL_SUCCESS,
  GET_ALL_FAIL,
} from "../actions/entity";

import apiCall from "@/utils/api";

const state = {
  // isError: true,
  inventories: [],
  customers: [],
  employees: [],
  status: false,
  paymentTypes: [],
};

const getters = {
  inventories: (state) => state.inventories,
  customers: (state) => state.customers,
  employees: (state) => state.employees,
  status: (state) => state.status,
  paymentTypes: (state) => state.paymentTypes,
};

const actions = {
  [GET_ALL_PAYMENT_TYPES]: ({ commit }) => {
    return new Promise((resolve, reject) => {
      apiCall({ url: "/dictionarys/payment-types", method: "GET" })
        .then((resp) => {
          const payload = {
            Data: resp,
            EntityName: "paymentTypes",
          };
          commit(GET_ALL_SUCCESS, payload);
          resolve(resp);
        })
        .catch((err) => {
          const payload = {
            Data: err,
            EntityName: "paymentTypes",
          };
          commit(GET_ALL_FAIL, payload);
          reject(err);
        });
    });
  },
  [GET_ALL_INVENTORY]: ({ commit }) => {
    return new Promise((resolve, reject) => {
      apiCall({ url: "/inventories/filter", method: "GET" })
        .then((resp) => {
          const payload = {
            Data: resp,
            EntityName: "inventories",
          };
          commit(GET_ALL_SUCCESS, payload);
          resolve(resp);
        })
        .catch((err) => {
          const payload = {
            Data: err,
            EntityName: "inventories",
          };
          commit(GET_ALL_FAIL, payload);
          reject(err);
        });
    });
  },
  [REFRESH_INVENTORY_DATA]: ({ commit }) => {
    return new Promise((resolve, reject) => {
      apiCall({ url: "/inventories/filter", method: "GET" })
        .then((resp) => {
          const payload = {
            Data: resp,
            EntityName: "inventories",
          };
          commit(GET_ALL_SUCCESS, payload);
          resolve(resp);
        })
        .catch((err) => {
          const payload = {
            Data: err,
            EntityName: "inventories",
          };
          commit(GET_ALL_FAIL, payload);
          reject(err);
        });
    });
  },
  [GET_ALL_CUSTOMER]: ({ commit }) => {
    return new Promise((resolve, reject) => {
      apiCall({ url: "/customers", method: "GET" })
        .then((resp) => {
          const payload = {
            Data: resp,
            EntityName: "customers",
          };
          commit(GET_ALL_SUCCESS, payload);
          resolve(resp);
        })
        .catch((err) => {
          const payload = {
            Data: err,
            EntityName: "customers",
          };
          commit(GET_ALL_FAIL, payload);
          reject(err);
        });
    });
  },
  [GET_ALL_EMPLOYEE]: ({ commit }) => {
    return new Promise((resolve, reject) => {
      apiCall({ url: "/employees", method: "GET" })
        .then((resp) => {
          const payload = {
            Data: resp,
            EntityName: "employees",
          };
          commit(GET_ALL_SUCCESS, payload);
          resolve(resp);
        })
        .catch((err) => {
          const payload = {
            Data: err,
            EntityName: "employees",
          };
          commit(GET_ALL_FAIL, payload);
          reject(err);
        });
    });
  },
};

const mutations = {
  [GET_ALL_SUCCESS]: (state, payLoad) => {
    state[payLoad.EntityName] = payLoad.Data;
  },
  [GET_ALL_FAIL]: (state, payLoad) => {
    state.status = "error";
    state[payLoad.EntityName] = [];
  },
};

export default {
  state,
  getters,
  actions,
  mutations,
};
