<template>
  <m-dialog title="Thông tin nhà phân phối" @onClose="onClose">
    <template v-slot:content>
      <div class="m-row">
        <m-input
          label="Tên nhà phân phối"
          ref="txtdistributorName"
          v-model="distributor.DistributorName"
          :required="true"
          :isDisabled="false"
        >
        </m-input>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Số điện thoại"
            v-model="distributor.PhoneNumber"
            :isDisabled="false"
            :onlyNumberChar="true"
          >
          </m-input>
        </div>
        <div class="m-col">
          <m-input
            label="Email"
            v-model="distributor.Email"
            :isDisabled="false"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Mã số thuế công ty"
            v-model="distributor.TaxCode"
            :isDisabled="false"
          >
          </m-input>
        </div>
        <div class="m-col">
        </div>
      </div>
      <div class="m-row">
        <div class="m-col">
          <m-input
            label="Nhân viên liên hệ"
            v-model="distributor.ContactName"
            :isDisabled="false"
          >
          </m-input>
        </div>
        <div class="m-col">
          <m-input
            label="Số điện thoại liên hệ"
            v-model="distributor.ContactPhoneNumber"
            :isDisabled="false"
            :onlyNumberChar="true"
          >
          </m-input>
        </div>
      </div>
      <div class="m-row">
        <m-text-area
          label="Địa chỉ công ty"
          v-model="distributor.Address"
        ></m-text-area>
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
  name: "DistributorDetail",
  emits: ["onClose", "onSaveSuccess"],
  components: {},
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
      this.maxios.get(`distributors/${this.id}`).then((res) => {
        this.distributor = res;
      });
    }
  },
  methods: {
    onDistributorGroupAdd() {
      this.showDistributorGroupFormAdd = true;
    },
    async onAddGroupSuccess(group) {
      await this.$refs.cbxDistributorGroup.reloadDataAndSetSelect(
        group.DistributorGroupName
      );
    },
    onClose() {
      this.$emit("onClose");
    },
    onSave() {
      if (this.formMode == Enum.FormMode.ADD) {
        this.maxios.post(`distributors`, this.distributor).then(() => {
          this.$emit("onClose");
          this.$emit("onSaveSuccess", this.distributor);
        });
      } else {
        this.maxios
          .put(`distributors/${this.id}`, this.distributor)
          .then(() => {
            this.$emit("onClose");
            this.$emit("onSaveSuccess", this.distributor);
          });
      }
    },
  },
  data() {
    return {
      distributor: {},
    };
  },
};
</script>
<style scoped></style>
