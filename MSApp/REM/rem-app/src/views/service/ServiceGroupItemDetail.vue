<template>
  <m-dialog title="Nhóm dịch vụ" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Tên nhóm dịch vụ"
            ref="txtServiceGroupName"
            v-model="serviceGroup.ServiceGroupName"
            :required="true"
            :isDisabled="false"
            :isFocus="true"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-combobox
            label="Thuộc nhóm"
            v-model="serviceGroup.ParentId"
            url="/service-groups"
            propText="ServiceGroupName"
            propValue="ServiceGroupId"
          ></m-combobox>
        </div>
      </div>
      <div class="m-row">
        <m-text-area
          label="Mô tả"
          v-model="serviceGroup.Description"
        ></m-text-area>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-checkbox
            id="chkIsSystem"
            :disabled="!isAdministrator"
            v-model="serviceGroup.IsSystem"
            label="Đơn vị hệ thống"
          ></m-checkbox>
        </div>
      </div>
    </template>
    <template v-slot:footer>
      <div class="footer-button">
        <button class="btn btn--default mg-left-10" @click="onSave">Lưu</button>
        <button class="btn btn--close" @click="onClose">Hủy</button>
      </div>
    </template>
  </m-dialog>
</template>
<script>
import router from "@/router";
import Enum from "../../scripts/enum";
export default {
  name: "ServiceGroupItemDetail",
  emits: ["onClose"],
  props: ["id"],
  computed: {
    formMode: function () {
      if (this.id) {
        return Enum.FormMode.UPDATE;
      } else {
        return Enum.FormMode.ADD;
      }
    },
    isAdministrator: function () {
      var role = localStorage.getItem("userRoleValue");
      if (role == Enum.Role.Administrator) {
        return true;
      } else {
        return false;
      }
    },
  },
  created() {
    if (this.id) {
      this.maxios.get(`service-groups/${this.id}`).then((res) => {
        this.serviceGroup = res;
      });
    }
  },
  methods: {
    redictRouter(){
      const currentPath = this.$router.currentRoute.value.path;
      if (currentPath.includes("/services/groups")) {
        router.push(this.routerStore?.History?.from?.path || `/services/groups`);
      } else {
        this.$emit("onClose");
      }
    },
    onClose() {
      this.redictRouter();
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`service-groups`, this.serviceGroup).then(() => {
          this.redictRouter();
        });
      } else {
        this.maxios
          .put(`service-groups/${this.id}`, this.serviceGroup)
          .then(() => {
            this.redictRouter();
          });
      }
    },
  },
  data() {
    return {
      serviceGroup: {},
    };
  },
};
</script>
<style scoped>
.footer-button {
  display: flex;
  align-items: center;
  flex-direction: row-reverse;
  justify-content: flex-start;
}
</style>
