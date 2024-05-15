<template>
  <m-dialog title="Danh mục vị trí/ Chức vụ" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Tên vị trí/ Chức vụ"
            ref="txtPositionName"
            v-model="position.PositionName"
            :validated="isValidated"
            :required="true"
            :isDisabled="false"
            :isFocus="true"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <m-text-area label="Mô tả" v-model="position.Description"></m-text-area>
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
  name: "PositionDetail",
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
  },
  created() {
    if (this.id) {
      this.maxios.get(`positions/${this.id}`).then((res) => {
        this.position = res;
      });
    }
  },
  methods: {
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`positions`, this.position).then(() => {
          this.$emit("onClose");
        });
      } else {
        this.maxios.put(`positions/${this.id}`, this.position).then(() => {
          this.$emit("onClose");
        });
      }
    },
  },
  data() {
    return {
      position: {},
    };
  },
};
</script>
<style scoped></style>
