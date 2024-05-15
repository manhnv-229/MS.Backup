<template>
    <div class="m-checkbox">
        <input :id="id" class="input-checkbox" :disabled="disabled" v-model="value" type="checkbox" name="mcheckbox"><label :for="id">{{ label }}</label>
    </div>
</template>
<script>
export default {
    name:"MCheckbox",
    emits:["update:modelValue"],
    props:["label", "id","modelValue","disabled"],
    created() {
        this.value = this.modelValue;
    },
   watch: {
    value(newValue){
        this.$emit("update:modelValue",newValue);
    }
   },
    methods: {
      
    },
    data() {
        return {
            value: null
        }
    },
}
</script>
<style scoped>
.m-checkbox{
    display: flex;
    align-items: center;
    column-gap: 5px;
}
.input-checkbox{
    height: 20px;
    width: 20px;
    border:1px solid #ccc;
}

.input-checkbox:focus,.input-checkbox:active{
    outline:1px dashed #0033ff;
}

</style>