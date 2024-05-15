<template>
  <m-dialog title="Gia hạn VIP" @onClose="onClose">
    <template v-slot:content> 
        <m-combobox
        label="Thời hạn VIP"
        v-model="license.VIPOption"
        url="/dictionarys/vip-options"
        propText="Text"
        propValue="Value"
        @onSelected="onSelectVIP"
        >
            
        </m-combobox>

    </template>
    <template v-slot:footer>
      <div class="form__button">
        <button class="btn btn--default" @click="onRenewVIP" :disabled="!vipSelected">
          <i class="icofont-ui-rate-add"></i> Gia hạn
        </button>
        <button class="btn btn--close">
            <i class="icofont-ui-close"></i> Hủy
        </button>
      </div>
    </template>
  </m-dialog>
</template>
<script>
export default {
  name: "RenewLicense",
  props: ["organization"],
  emits: ["onClose"],
  created() {},
  methods: {
    onRenewVIP(){
        this.license.OrganizationId = this.organization.OrganizationId;
        this.license.VIPOption = this.vipSelected.Value;
        this.commonJs.showConfirm(`Bạn có chắc chắn muốn gia hạn <b>${this.vipSelected?.Text} VIP</b> cho <b>${this.organization.OrganizationName}</b>?`, ()=>{
            this.maxios.post(`organizations/${this.organization.OrganizationId}/licenses`, this.license)
            .then(()=>{
                this.$emit("onClose");
            })
        })
    },
    onSelectVIP(value, text, option){
        this.vipSelected = option;
    },
    onClose(){
        this.$emit("onClose");
    }
  },
  data() {
    return {
        license:{},
        vipSelected:null
    };
  },
};
</script>
<style scoped>
.form__button{
    display: flex;
    flex-direction: row-reverse;
    column-gap: 10px;
    align-items: center;
    justify-content: flex-start;
}
</style>
