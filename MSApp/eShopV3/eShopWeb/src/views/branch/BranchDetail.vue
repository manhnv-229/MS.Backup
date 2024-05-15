<template>
    <m-dialog title="Thông tin chi nhánh" @onClose="onClose">
      <template v-slot:content>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Mã chi nhánh"
              ref="txtBranchName"
              v-model="branch.BranchCode"
              :required="true"
              :isDisabled="false"
              :isFocus="true"
            >
            </m-input>
          </div>
        </div>
        <div class="m-row">
          <div class="m-col">
            <m-input
              label="Tên chi nhánh"
              ref="txtBranchName"
              v-model="branch.BranchName"
              :required="true"
              :isDisabled="false"
            >
            </m-input>
          </div>
        </div>
        <div class="m-row">
          <m-text-area label="Mô tả" v-model="branch.Description"></m-text-area>
        </div>
      </template>
      <template v-slot:footer>
        <button class="btn btn--close" @click="onClose">Hủy</button>
        <button class="btn btn--default mg-left-10" @click="onSave">Lưu</button>
      </template>
    </m-dialog>
  </template>
  <script>
  import Enum from "@/scripts/enum";
  export default {
    name: "BranchDetail",
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
      isAdministrator: function(){
        var role = localStorage.getItem("userRoleValue");
        if (role == Enum.Role.Administrator) {
          return true;
        }else{
          return false;
        }
      }
    },
    created() {
      if (this.id) {
        this.maxios.get(`branchs/${this.id}`).then((res) => {
          this.branch = res;
        });
      }else{
        this.maxios.get(`branchs/new-code`).then((res) => {
          this.branch.BranchCode = res;
        });
      }
    },
    methods: {
      onClose() {
        this.$emit("onClose");
      },
      onSave() {
        if (this.formMode == Enum.FormMode.ADD) {
          this.maxios.post(`branchs`, this.branch).then(() => {
            this.$emit("onClose");
          });
        } else {
          this.maxios.patch(`branchs/${this.id}`, this.branch).then(() => {
            this.$emit("onClose");
          });
        }
      },
    },
    data() {
      return {
        branch: {},
      };
    },
  };
  </script>
  <style scoped>
  </style>