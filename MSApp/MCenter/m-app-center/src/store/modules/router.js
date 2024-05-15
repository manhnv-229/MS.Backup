import { SET_ROUTER_HISTORY } from "../actions/router";
import { globalProps } from '@/main.js';
const state = {
  // isError: true,
  routerStore: null,
};

const getters = {
  routerStore: (state) => state.routerStore,
};

const actions = {
  [SET_ROUTER_HISTORY]: ({ commit }, data) => {
    const router = {
      History: {
        to: data.to,
        from: data.from,
      },
    };
    globalProps.routerStore = router;
    commit(SET_ROUTER_HISTORY, router);
  },
};
const mutations = {
  [SET_ROUTER_HISTORY]: (state, payLoad) => {
    state.routerStore = payLoad;
  },
};
export default {
  state,
  getters,
  actions,
  mutations,
};
