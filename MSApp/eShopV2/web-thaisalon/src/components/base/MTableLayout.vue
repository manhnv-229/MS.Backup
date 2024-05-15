<template>
    <div class="table">
        <div class="table__toolbar">
            <slot name="toolbar"></slot>
        </div>
        <div class="table__container">
            <slot name="container"></slot>
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

    }
}
</script>
<style scoped>
@import url(../../styles/base/table.css);
</style>