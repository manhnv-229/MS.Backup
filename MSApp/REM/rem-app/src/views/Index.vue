<template>
  <m-page title="Báo cáo thường kỳ" :rightSlot="true">
    <template v-slot:header-right> </template>
    <template v-slot:container>
      <div class="homepage-container">
        <div class="app-info">
          <div class="app__name">
            Phần mềm quản lý cửa hàng, tạp hoá, quán cafe, nhà hàng,....
          </div>
          <div class="app_factory">
            Được phát triển bởi Công ty TNHH Thương Mại và Công Nghệ Mạnh Hào
          </div>
          <div class="company__address">Địa chỉ: 923 Lê Lợi, Dĩnh Kế, TP Bắc Giang</div>
          <div class="company__phone">SĐT: 0974.826.326</div>
          <div class="company__hotline">Hỗ trợ kỹ thuật vui lòng gọi: 0933.365.999 (Mr Mạnh)</div>
        </div>
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
.homepage-container {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}
.homepage-container img {
  width: 100%;
}

.app__name{
  font-size: 24px;
}

.app_factory{

}
</style>
