<template>
  <the-navbar v-if="isAuthenticated"></the-navbar>
  <div class="layout-container" v-if="isAuthenticated">
    <the-header
      :isAuthenticated="isAuthenticated"
      :isProfileLoaded="isProfileLoaded"
      :organizationInfo="organizationInfo"
    ></the-header>
    <the-main></the-main>
  </div>
  <router-view name="LoginPage"></router-view>
  <router-view name="HomePage"></router-view>
  <router-view name="Other"></router-view>
  <router-view name="Organization"></router-view>

  <MLoading v-if="showLoading" />
  <MLoading v-if="connectingHub" />
  <!-- <home-page v-if="!isAuthenticated && showHomePage" v-model:showHomePage="showHomePage"></home-page> -->
  <!-- <MLoading /> -->
  <MDialogNotification
    v-if="isShowError"
    :showCancelButton="showCancelButton"
    :title="titleNotification"
    :arrayMsgs="errorsNotification"
    :type="msgType"
    @btnCancelClick="hideNotice"
    @confirmFunction="confirmFunction"
  />
  <m-toast
    v-if="isShowToast"
    :msg="msgToast"
    :type="msgToastType"
    @onCloseToast="onCloseToast"
  ></m-toast>
  <progress-bar
    v-if="isShowProgressBar"
    :processList="processList"
  ></progress-bar>
  <MNote v-if="isAuthenticated"></MNote>
  <MHistory v-if="isAuthenticated"></MHistory>
</template>
<script>
import TheHeader from "./layout/TheHeader.vue";
import TheNavbar from "./layout/TheNavbar.vue";
import TheMain from "./layout/TheMain.vue";
import { SET_ROUTER_HISTORY } from "./store/actions/router";
import { USER_REQUEST } from "./store/actions/user";
import { CLEAR_TOAST } from "./store/actions/toast";
import { mapGetters, mapState } from "vuex";
import MDialogNotification from "./components/base/MDialogNotification.vue";
import MToast from "./components/base/MToast.vue";
import ProgressBar from "./components/base/ProgressBar.vue";
import MNote from "./components/base/note/MNote.vue";
import MHistory from "@/components/base/history/MHistory.vue";
import { computed } from "vue";
export default {
  name: "App",
  components: {
    TheHeader,
    TheMain,
    TheNavbar,
    MToast,
    MDialogNotification,
    ProgressBar,
    MNote,
    MHistory,
  },
  provide() {
    return {
      organization: computed(() => this.organizationInfo),
    };
  },
  computed: {
    ...mapGetters([
      "getProfile",
      "isAuthenticated",
      "isProfileLoaded",
      "isShowError",
      "errorsNotification",
      "titleNotification",
      "showCancelButton",
      "isShowLoading",
      "connectingHub",
      "msgType",
      "confirmFunction",
      "role",
      "isShowToast",
      "msgToast",
      "msgToastType",
      "isShowProgressBar",
      "processList",
      "organizationInfo",
    ]),
    ...mapState({
      authLoading: (state) => state.auth.status === "loading",
      name: (state) => `${state.user.profile.title} ${state.user.profile.name}`,
    }),
  },
  watch: {
    isShowLoading: function (newValue) {
      if (newValue == false) {
        this.$nextTick(function () {
          this.showLoading = false;
        });
      } else {
        this.showLoading = true;
      }
    },
    $route(to, from) {
      const payLoad = {
        to: to,
        from: from,
      };
      this.$store.dispatch(SET_ROUTER_HISTORY, payLoad);
    },
  },
  async created() {
    if (this.$store.getters.isAuthenticated) {
      await this.$store.dispatch(USER_REQUEST);
      // this.hubConnection = notification.createHub();
      // // this.hubConnection = notification.CreateHubProxy();
      // this.hubConnection
      //   .start()
      //   .then(() => {
      //     console.log("Đã kết nối tới Hub...");
      //   })
      //   .catch((err) => console.error(err));
    }
  },
  methods: {
    hideNotice() {
      this.commonJs.hideConfirm();
    },
    closeMessageBox() {
      this.showMessageBox = false;
    },
    onCloseToast() {
      this.$store.dispatch(CLEAR_TOAST);
    },
  },
  data() {
    return {
      showHomePage: false,
      showNew: false,
      showLoading: false,
      progressing: false,
    };
  },
}
</script>
<style>
@import url(./styles/main.css);
</style>
