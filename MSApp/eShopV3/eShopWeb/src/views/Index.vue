<template>
  <m-page title="Báo cáo thường kỳ" :rightSlot="true">
    <template v-slot:header-right>
     
    </template>
    <template v-slot:container>
      <div class="homepage-container">
        <img src="../assets/imgs/manhhaomart.jpg" alt="">
      </div>
    </template>
    <template v-slot:footer> </template>
  </m-page>
</template>
<script>
export default {
  name: "HomePage",
  props: ["showHomePage"],
};
</script>
<style scoped>
.homepage-container{
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}
.homepage-container img{
  width: 100%;
}
</style>
