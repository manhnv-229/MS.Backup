<template>
    <div class="table">
        <div class="table__toolbar" v-if="showToolbar">
            <slot name="toolbar"></slot>
        </div>
        <div class="table__container">
            <div class="table__container-border">
                <slot name="container"></slot>
            </div>
        </div>
        <div class="table__footer">
            <slot name="footer"></slot>
        </div>
    </div>
</template>
<script>
export default {
    name:"MTable",
    emits:[],
    props:{
        showToolbar:{
            type: Boolean,
            default: true,
            required: false
        }
    }
}
</script>
<style scoped>
@import url(../../styles/base/table.css);
</style>