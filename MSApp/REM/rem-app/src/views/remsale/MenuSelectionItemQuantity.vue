<template>
    <div class="popup">
        <div class="popup-wrapper">
            <button class="mpopup__button-close" @click="onClose"></button>
            <div class="popup__title">Số lượng <b>{{ itemInput.InventoryItemName }}</b></div>
            <div class="popup__container">
                <m-input ref="txtQuantity" :selectAllText="true" v-model="quantity" :isFocus="true"
                    align="right" :onlyNumberChar="true">
                </m-input>
                <div class="error-info" v-if="error">{{ error }}</div>
            </div>
            <div class="popup__action">
                <button class="btn btn--default icon icon-16 icon--save" @click="onConfirmQuantity">Xác nhận</button>
            </div>
        </div>
    </div>
</template>
<script>
export default {
    name: "MenuSelectionItemQuantity",
    props: ["itemInput"],
    emits: ["onConfirmQuantity", "onClose"],
    inject: [],
    provide: [],
    computed: {

    },
    watch: {
        quantity: function(newValue){
            if (!newValue || newValue == 0 || newValue === null || newValue === undefined || newValue < 0) {
                this.error = "Số lượng phải lớn hơn 0.";
                this.$refs.txtQuantity.setFocus();
                this.$refs.txtQuantity.setSelectAllText();
            }else{
                this.error = null;
            }
        }
    },
    created() {

    },
    methods: {
        onConfirmQuantity() {
            if (!this.quantity || this.quantity == 0 || this.quantity === null || this.quantity === undefined || this.quantity < 0) {
                this.error = "Số lượng phải lớn hơn 0.";
                this.$refs.txtQuantity.setFocus();
            } else {
                this.$emit("onConfirmQuantity", this.quantity);
            }
        },
        onClose() {
            this.$emit("onClose");
        }
    },
    data() {
        return {
            quantity: 1,
            error: ""
        }
    },
}
</script>
<style scoped>
.popup {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: #72727269;
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 9999
}

.popup-wrapper {
    position: relative;
    background-color: #fff;
    border-radius: 4px;
}

.popup__title {
    padding: 12px 12px 0 12px;
    max-width: 150px;
}

.popup__container {
    padding: 12px;
}

.popup__action {
    display: flex;
    align-items: center;
    justify-content: flex-end;
    column-gap: 6px;
    padding: 12px
}
.error-info{
    color: #ff0000;
    margin-top: 4px;
}
</style>