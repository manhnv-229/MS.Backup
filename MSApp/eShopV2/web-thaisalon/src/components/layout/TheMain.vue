<template>
  <div class="main">
    <div class="main-container">
      <!-- <router-view :key="$route.path"></router-view> -->
      <router-view></router-view>
      <router-view class="register" name="Register"></router-view>
    </div>
  </div>
</template>
<script>
export default {};
</script>

<style scoped>
.main {
  width: 100%;
  position: relative;
  display: flex;
  flex: 1;
  justify-content: center;
  box-sizing: border-box;
  overflow-y: auto;
}

.main-container {
  height: 100%;
  max-width: 700px;
  position: relative;
  background-color: #ffffff5c;
  box-sizing: border-box;
  overflow-y: auto;
  padding: 10px;
}
</style>
