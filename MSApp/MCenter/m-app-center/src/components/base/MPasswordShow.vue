<template>
    <div class="m-pw-show">
        <div v-if="showData" class="m-pw-data">{{ input }}</div>
        <div v-else class="m-pw-data">⁎⁎⁎⁎⁎⁎⁎⁎⁎⁎</div>
        <div class="show-data icon icon-20 icon--eye" @click="showData =! showData"></div>
    </div>
</template>
<script>
export default {
    name:"MPasswordShow",
    props:["input"],
    data() {
        return {
            showData: false
        }
    },
}
</script>
<style scoped>
.m-pw-data{
    flex: 1
}
.m-pw-show{
    min-width: 80px;
    display: flex;
    align-items: center;
    gap: 6px;
}
.show-data {
    width: 20px;
    height: 20px;
    flex: 0 0 20px;
    cursor: pointer;
    border: unset;
    background-color: unset;
    opacity: .7;
}
</style>