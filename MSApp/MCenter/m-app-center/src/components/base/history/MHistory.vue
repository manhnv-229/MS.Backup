<template>
    <div v-show="showHistory" class="activities-history">
        <button v-if="showHistory" class="history__button--close" @click="onShowHistory">
            <i class="icofont-ui-close"></i>
        </button>
        <div v-show="showHistory" class="history__header">
            <span>Lịch sử hoạt động</span>
        </div>
        <div v-show="showHistory" class="history__detail">
            <div class="activity-item" v-for="(item, index) in activitiesHistory" :key="index">
                <div v-html="item"></div>
            </div>
        </div>
    </div>
    <div v-show="!showHistory" class="history__minize">
            <div class="history__number" @click="onShowHistory">
                {{ activitiesHistory?.length || 0 }}
            </div>
            <button class="history__button icon icon--history" @click="onShowHistory"></button>
        </div>
</template>
<script>
export default {
    name: "MHistory",

    created() {
        this.$emitter.on("AddActivity", this.addActivityHistory);
    },
    methods: {
        addActivityHistory(act) {
            this.activitiesHistory.push(act);
        },
        onShowHistory() {
            this.showHistory = !this.showHistory;
        },
    },
    data() {
        return {
            activitiesHistory: [],
            showHistory: false,
        };
    },
};
</script>
<style>
.activities-history {
    position: fixed;
    bottom: 24px;
    left: 86px;
    z-index: 1000;
    width: 250px;
    height: 400px;
    box-sizing: border-box;
    display: flex;
    flex-direction: column;
}
.activity-item{
    background-color: #d4ecff;
    border-radius: 4px;
    padding: 4px;
}
.history__header{
    padding: 12px;
    border: 1px solid #dedede;
    border-radius: 4px 4px 0 0;
    background-color: #fff;
}
.history__detail {
    padding: 12px;
    width: 100%;
    flex:1;
    background-color: #fff;
    border: 1px solid #ccc;
    border-top: unset;
    border-radius: 0 0 4px 4px;
    display: flex;
    flex-direction: column-reverse;
    row-gap: 6px;
    overflow-y: auto;
    box-sizing: border-box;
}

.history__minize {
    position: fixed;
    bottom: 24px;
    left: 86px;
    z-index: 1000;
}

.history__button {
    width: 50px;
    height: 50px;
    background-repeat: no-repeat;
    background-position: center;
    background-size: contain;
    border: unset;
    background-color: transparent;
    cursor: pointer;
}

.history__button:hover {
    opacity: 1;
}

.history__number {
    border: 1px solid #dedede;
    position: absolute;
    top: -5px;
    right: -5px;
    width: 24px;
    height: 24px;
    border-radius: 50%;
    background-color: #fff;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 700;
    color: #ff0000;
    cursor: pointer;
    z-index: 1001;
}

.history__number:hover {
    width: 28px;
    height: 28px;
    top: -10px;
    right: -10px;
}

.history__button--close {
    border: 1px solid #ff0000;
    border-radius: 50%;
    position: absolute;
    width: 38px;
    height: 38px;
    top: -10px;
    right: -10px;
    font-size: 20px;
    padding: 0;
    color: #ff0000;
    background-color: #fff;
    cursor: pointer;
    box-shadow: 0px 0px 8px #6d6d6d;
    z-index: 1001;
}
</style>
