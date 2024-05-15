<template>
  <div class="page">
    <div class="page__container">
      <m-table-layout>
        <template #toolbar>
            <slot name="toolbar"></slot>
        </template>
        <template #container>
            <m-table :data="datax" empty-text="Không có dữ liệu" width="100%" height="100%">
                <m-column v-for="(col, index) in columns" :key="index" :prop="col.PropName" :label="col.Label" :width="col.Width">
                    <template #default="scope">
                            <span>{{ scope.row[col.PropName]}}</span>
                    </template>
                </m-column>
            </m-table>
        </template>
        <template #footer>
          <slot name="footer"></slot>
        </template>
      </m-table-layout>
    </div>
    <div class="page__footer">
      
    </div>
  </div>
</template>
<script>
export default {
  name: "BasePageList",
  props:{
    columns:{
        type: Array,
        default: ()=>[]
    },
    data:{
        type: Array,
        default: ()=>[]
    }
  },
  created() {
    this.columnx = this.columns;
  },
  watch: {
    data: {
        immediate: true,
        handler: function(newData){
            this.datax = newData;
        }
    }
  },
  methods: {},

  data() {
    return {
        columnx:[],
        datax: []
    };
  },
};
</script>
<style scoped>
</style>