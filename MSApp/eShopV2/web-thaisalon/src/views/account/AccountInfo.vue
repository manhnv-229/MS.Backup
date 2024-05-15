<template>
  <m-page title="THÔNG TIN CÁ NHÂN">
    <template v-slot:header-right></template>
    <template v-slot:container>
      <div class="emp-container">
        <div class="form-group">
          <div class="m-row">
            <div class="m-col" style="width: 100px">
              <m-input
                label="Mã nhân viên"
                ref="txtuserCode"
                v-model="user.Employee.EmployeeCode"
                :required="true"
                :disabled="true"
                :isFocus="false"
              >
              </m-input>
            </div>
            <div class="m-col">
              <m-input
                label="Họ và tên"
                ref="txtEmployeeName"
                v-model="user.Employee.FullName"
                :required="true"
                :isDisabled="false"
              >
              </m-input>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <label for="">Ngày sinh:</label>
              <el-date-picker
                v-model="user.Employee.DateOfBirth"
                type="date"
                format="DD-MM-YYYY"
              />
            </div>
            <div class="m-col">
              <m-combobox
                label="Giới tính"
                v-model="user.Employee.Gender"
                url="/dictionarys/gender"
                propText="Text"
                propValue="Value"
              ></m-combobox>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Số điện thoại"
                v-model="user.Employee.Mobile"
                :isDisabled="false"
                :required="true"
                :onlyNumberChar="true"
              >
              </m-input>
            </div>
            <div class="m-col">
              <m-input
                label="Email"
                v-model="user.Employee.Email"
                :isDisabled="false"
                :required="true"
                :onlyNumberChar="true"
              >
              </m-input>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Số CMTND"
                v-model="user.Employee.IdentityNumber"
                :required="false"
              >
              </m-input>
            </div>
            <div class="m-col"></div>
          </div>
          <div class="m-row">
            <m-input label="Địa chỉ" v-model="user.Employee.Address"> </m-input>
          </div>
        </div>
        <!-- THÔNG TIN CÔNG VIỆC -->
        <div class="form-group">
          <div class="group__title">Thông tin công việc</div>
          <div class="m-row">
            <div class="m-col">
              <m-combobox
                label="Vị trí"
                ref="cbxPosition"
                v-model="user.Employee.PositionId"
                url="/positions"
                propText="PositionName"
                propValue="PositionId"
              ></m-combobox>
            </div>
            <div class="m-col">
              <label for="">Ngày bắt đầu làm việc:</label>
              <el-date-picker
                v-model="user.Employee.JoinDate"
                type="date"
                format="DD-MM-YYYY"
              />
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-combobox
                label="Tình trạng công việc"
                v-model="user.Employee.WorkStatus"
                url="/dictionarys/work-status"
                propText="Text"
                propValue="Value"
              ></m-combobox>
            </div>
            <div class="m-col">
              <m-input
                label="Mức lương"
                :onlyNumberChar="true"
                v-model="user.Employee.Salary"
              >
              </m-input>
            </div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Số tài khoản ngân hàng"
                v-model="user.Employee.BankAccountNumber"
              >
              </m-input>
            </div>
            <div class="m-col"></div>
          </div>
          <div class="m-row">
            <div class="m-col">
              <m-input label="Ngân hàng" v-model="user.Employee.BankName"> </m-input>
            </div>
            <div class="m-col">
              <m-input
                label="Chi nhánh ngân hàng"
                v-model="user.Employee.BankBranchName"
              >
              </m-input>
            </div>
          </div>
        </div>
        <!-- THÔNG TIN TÀI KHOẢN -->
        <div class="form-group">
          <div class="group__title">Thông tin tài khoản</div>
          <div class="m-row">
            <div class="m-col">
              <m-input
                label="Tài khoản đăng nhập"
                :modelValue="user.UserName"
                ref="txtUserName"
                :disabled="true"
              >
              </m-input>
            </div>
            <div class="m-col">
              <m-combobox
                label="Quyền trên Website"
                :modelValue="roleId"
                url="/roles"
                propText="OtherName"
                propValue="RoleId"
                :isDisabled="true"
              ></m-combobox>
            </div>
          </div>
          <!-- <div class="m-row">
            <div class="m-col">
              <m-input
                label="Mật khẩu"
                type="password"
                ref="txtPassword"
                :required="true"
                :validated="isValidated"
                v-model="user.Password"
              >
              </m-input>
            </div>
            <div class="m-col">
              <m-input
                label="Xác nhận mật khẩu"
                type="password"
                ref="txtRePassword"
                :required="true"
                :validated="isValidated"
                v-model="user.RePassword"
              >
              </m-input>
            </div>
          </div> -->
        </div>
      </div>
    </template>
    <template v-slot:footer>
      <div class="footer__button">
        <!-- <button class="btn btn--close" @click="onClose">Hủy</button> -->
        <button class="btn btn--default mg-left-10" :disabled="!isChange" @click="onSave"><i class="icofont-ui-check"></i> Lưu</button>
      </div>
    </template>
  </m-page>
</template>
<script>
import Enum from "@/scripts/enum";
import router from "@/router";
export default {
  name: "AccountInfo",
  emits: ["onClose"],
  props: ["id"],
  computed: {
    formMode: function () {
      return Enum.FormMode.UPDATE;
    },
    roleId: function () {
      if (this.user.Roles) {
        return this.user.Roles[0].RoleId;
      }
      return null;
    },
    isChange: function(){
        var newUseJson = JSON.stringify(this.user);
        if (newUseJson != this.origilUser) {
            return true;
        }else{
            return false;
        }
    }
  },
  created() {
    var userId = localStorage.getItem("user-id");
    if (userId) {
      this.maxios.get(`accounts/${userId}`).then((res) => {
        this.user = res;
        this.origilUser = JSON.stringify(res);
      });
    }
  },
  methods: {
    onClose() {
      router.push("/");
    },
    onSave() {
      this.maxios.put(`employees/${this.user.Employee.EmployeeId}`, this.user.Employee);
    },
  },
  data() {
    return {
      user: { Employee: {} },
      origilUser:null,
    };
  },
};
</script>
<style scoped>
.emp-container {
  display: flex;
  flex-direction: column;
  row-gap: 25px;
  padding-bottom: 10px;
}

.group__title {
  text-transform: uppercase;
  font-weight: 600;
  border-bottom: 1px dashed #dedede;
  border-top: 1px dashed #dedede;
  margin-bottom: 10px;
  padding: 5px 0;
  box-shadow: 0px 0px 5px #8e8e8e;
}

.footer__button{
    display: flex;
    align-items: center;
    justify-content: flex-end;
    padding: 10px 0;
    border-top: 1px dashed #dedede;
}
</style>
