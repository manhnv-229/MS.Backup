<template>
  <div class="configs">
    <div class="config-item" v-for="(item, index) in configs" :key="index">
      {{ item.Name }}
    </div>
  </div>
</template>
<script>
export default {
    name:"SystemConfig",
    props:[],
    created() {
       this.loadConfigs();
    },
    methods: {
        loadConfigs(){
            this.maxios.get("/system-configs")
        .then(res=>{
            this.configs = res;
        })
        }
    },
    data() {
        return {
            configs:[]
        }
    },

}
</script>
<style scoped></style>
