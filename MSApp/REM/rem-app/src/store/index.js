import { createStore } from 'vuex'
import user from './modules/user'
import auth from './modules/auth'
import error from './modules/error'
import notification from './modules/notification'
import loading from './modules/loading'
import toast from './modules/toast'
import signalR from './modules/signalR'
import progressbar from './modules/progressbar'
import entity from './modules/entity'
import router from './modules/router'
const store = createStore({
    state: {},
    mutations: {},
    actions: {},
    modules: {
        user,
        auth,
        error,
        notification,
        loading,
        toast,
        signalR,
        progressbar,
        entity,
        router
    }
})
export default store