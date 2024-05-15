<template>
  <router-view name="TenantRouterView"></router-view>
</template>
<script>
import { computed } from "vue";
export default {
  name: "TenantIndex",
  created() {
    this.maxios.get(`/system-configs`).then((res) => {
      this.appDomain.BAZA = res.domain;
      this.systemConfigs = res.configs;
      var bazaServer = this.systemConfigs.find(item=>item.ConfigKey === "BAZA_DB_DEFAULT");
      this.bazaDbServerDefault = JSON.parse(bazaServer.JSON_Value);
    });
  },
  provide() {
    return {
      appDomain: computed(() => this.appDomain),
      bazaDbServerDefault: computed(() => this.bazaDbServerDefault),
    };
  },
  methods: {
    loadData(){
    }
  },
  data() {
    return {
      appDomain:{},
      systemConfigs:[],
      bazaDbServerDefault:{}
    };
  },
};
</script>
<style scoped></style>
