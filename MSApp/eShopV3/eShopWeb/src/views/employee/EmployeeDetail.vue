<template>
  <m-dialog title="Thông tin nhân viên" @onClose="$emit('onClose')">
    <template v-slot:content>
      <form id="frm-detail">
        <el-tabs v-model="tabActive" type="border-card" @tab-click="tabOnClick">
          <el-tab-pane label="Thông tin chung" name="general">
            <div class="form-group">
              <div class="m-row">
                <div class="m-col">
                  <m-input
                    label="Mã nhân viên"
                    ref="txtEmployeeCode"
                    v-model="employee.EmployeeCode"
                    :validated="isValidated"
                    :required="true"
                    :isDisabled="false"
                    :isFocus="true"
                  >
                  </m-input>
                </div>
                <div class="m-col">
                  <m-input
                    label="Họ và tên"
                    ref="txtFullName"
                    :validated="isValidated"
                    v-model="employee.FullName"
                    :required="true"
                  >
                  </m-input>
                </div>
              </div>
              <div class="m-row">
                <div class="m-col">
                  <label for="">Ngày sinh:</label>
                  <el-date-picker
                    v-model="employee.DateOfBirth"
                    type="date"
                    format="DD-MM-YYYY"
                  />
                </div>
                <div class="m-col">
                  <m-combobox
                    label="Giới tính"
                    v-model="employee.Gender"
                    url="/dictionarys/gender"
                    propText="Text"
                    propValue="Value"
                  ></m-combobox>
                </div>
              </div>
              <div class="m-row">
                <div class="m-col">
                  <m-input
                    label="Điện thoại"
                    v-model="employee.Mobile"
                    type="tel"
                    ref="txtMobile"
                    :validated="isValidated"
                    :required="true"
                    :onlyNumberChar="true"
                  >
                  </m-input>
                </div>
                <div class="m-col">
                  <m-input label="Email" v-model="employee.Email"> </m-input>
                </div>
              </div>
              <div class="m-row">
                <div class="m-col">
                  <m-input
                    label="Số CMTND"
                    v-model="employee.IdentityNumber"
                    :required="false"
                  >
                  </m-input>
                </div>
                <div class="m-col"></div>
              </div>
              <div class="m-row">
                <m-input
                  label="Địa chỉ"
                  v-model="employee.Address"
                  @onBlur="addressOnBlur"
                >
                </m-input>
              </div>
            </div>
          </el-tab-pane>
          <el-tab-pane label="Công việc" name="work">
            <div class="form-group">
              <div class="m-row">
                <div class="m-col">
                  <m-combobox
                    label="Vị trí"
                    ref="cbxPosition"
                    v-model="employee.PositionId"
                    url="/positions"
                    propText="PositionName"
                    propValue="PositionId"
                  ></m-combobox>
                </div>
                <div class="m-col">
                  <label for="">Ngày bắt đầu làm việc:</label>
                  <el-date-picker
                    v-model="employee.JoinDate"
                    type="date"
                    format="DD-MM-YYYY"
                  />
                </div>
              </div>
              <div class="m-row">
                <div class="m-col">
                  <m-combobox
                    label="Tình trạng công việc hiện tại"
                    v-model="employee.WorkStatus"
                    url="/dictionarys/work-status"
                    propText="Text"
                    propValue="Value"
                  ></m-combobox>
                </div>
                <div class="m-col">
                  <m-input
                    label="Mức lương"
                    :onlyNumberChar="true"
                    v-model="employee.Salary"
                  >
                  </m-input>
                </div>
              </div>
              <div class="m-row">
                <div class="m-col">
                  <m-input
                    label="Số tài khoản ngân hàng"
                    v-model="employee.BankAccountNumber"
                  >
                  </m-input>
                </div>
                <div class="m-col"></div>
              </div>
              <div class="m-row">
                <div class="m-col">
                  <m-input label="Ngân hàng" v-model="employee.BankName"> </m-input>
                </div>
                <div class="m-col">
                  <m-input
                    label="Chi nhánh ngân hàng"
                    v-model="employee.BankBranchName"
                    @onBlur="bankBranchOnBlur"
                  >
                  </m-input>
                </div>
              </div>
            </div>
          </el-tab-pane>
          <el-tab-pane label="Tài khoản" name="account">
            <div class="m-row">
              <div class="m-col">
                <m-input
                  label="Tài khoản đăng nhập"
                  v-model="employee.User.UserName"
                  ref="txtUserName"
                  :required="true"
                  :validated="isValidated"
                >
                </m-input>
              </div>
              <div class="m-col">
                <m-combobox
                  label="Quyền trên Website"
                  @onSelected="onChangePermission"
                  v-model="employee.RoleId"
                  url="/roles"
                  propText="OtherName"
                  propValue="RoleId"
                ></m-combobox>
              </div>
            </div>
            <div class="m-row" v-if="formMode == Enum.FormMode.ADD">
              <div class="m-col">
                <m-input
                  label="Mật khẩu"
                  type="password"
                  ref="txtPassword"
                  :required="true"
                  :validated="isValidated"
                  v-model="employee.User.Password"
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
                  v-model="employee.User.RePassword"
                >
                </m-input>
              </div>
            </div>
            <div class="m-row" v-if="formMode == Enum.FormMode.ADD">
              <div class="note">
                <ul>
                  <li>
                    Mặc định tài khoản sẽ là số điện thoại của nhân viên. Bạn có thể nhập
                    tài khoản khác theo ý muốn.
                  </li>
                  <li>
                    Nếu bạn không thiết lập mật khẩu, thì mật khẩu mặc định sẽ là:
                    <b>12345678</b>
                  </li>
                </ul>
              </div>
            </div>
            <div v-if="role" class="m-row">
              <div class="permission-info">
                <label for="">Thông tin quyền hạn của tài khoản: </label>
                <li>{{ role.OtherName }}: {{ role.Description }}</li>
              </div>
            </div>
          </el-tab-pane>
        </el-tabs>
        <input
          hidden
          type="file"
          ref="fileAvatar"
          name="avatar"
          @change="fileAvatarOnChange"
          id="fileAvatar"
        />
      </form>
    </template>
    <template v-slot:footer>
      <button class="btn btn--default" @click="submit" style="order: 2">Lưu</button>
      <button
        class="btn btn--close"
        @click="$emit('onClose')"
        style="order: 1"
        @blur="btnCloseOnBlur"
      >
        Hủy
      </button>
    </template>
  </m-dialog>
</template>
<script>
import Enum from "@/scripts/enum";
export default {
  name: "EmployeeDetail",
  emits: ["onClose", "onSaveSuccess"],
  props: {
    id: {
      type: String,
      default: null,
      required: false,
    },
  },
  created() {
    if (this.id) {
      this.formMode = Enum.FormMode.UPDATE;
      this.maxios.get(`employees/${this.id}`).then((res) => {
        this.employee = res;
        console.log(`res`, res);
      });
    } else {
      this.getNewCode();
      this.employee.RoleId = "2c1f24fc-4f7b-11ed-9483-00163e06abee";
    }
  },
  watch: {
    // "employee.Mobile": function (newValue) {
    //   if (this.employee.User) {
    //     this.employee.User.UserName = newValue;
    //   }
    // },
  },
  methods: {
    btnCloseOnBlur() {
      this.tabActive = "general";
      this.$nextTick(function () {
        this.$refs.txtFullName.setFocus();
      });
    },
    addressOnBlur() {
      this.tabActive = "work";
      this.$nextTick(function () {
        this.$refs.cbxPosition.setFocus();
      });
    },
    bankBranchOnBlur() {
      this.tabActive = "account";
      this.$nextTick(function () {
        this.$refs.txtUserName.setFocus();
      });
    },
    tabOnClick(tab) {
      var tabName = tab.props.name;
      switch (tabName) {
        case "general":
          this.$nextTick(function () {
            this.$refs.txtFullName.setFocus();
          });
          break;
        case "work":
          this.$nextTick(function () {
            this.$refs.cbxPosition.setFocus();
          });
          break;
        case "account":
          this.$nextTick(function () {
            this.$refs.txtUserName.setFocus();
          });
          break;
        default:
          break;
      }
    },
    submit() {
      var isValid = this.onValidate();
      if (isValid) {
        var formData = new FormData();
        var file = this.$refs.fileAvatar.files[0];
        if (file) {
          formData.append("file", file);
        }

        formData.append("employee", JSON.stringify(this.employee));
        if (!this.id) {
          this.maxios.post("employees/create", formData).then(() => {
            this.$emit("onClose");
            this.$emit("onSaveSuccess");
          });
        } else {
          this.maxios.put(`employees/update`, formData).then(() => {
            this.$emit("onClose");
            this.$emit("onSaveSuccess");
          });
        }
      }
    },
    onValidate() {
      var errors = [];
      if (!this.commonJs.checkRequired(this.employee.EmployeeCode)) {
        errors.push("Mã nhân viên không được phép để trống.");
        this.inputInvalids.push(this.$refs.txtEmployeeCode);
        this.tabActive = "general";
      }
      if (!this.commonJs.checkRequired(this.employee.FullName)) {
        errors.push("Họ tên nhân viên không được phép để trống.");
        this.inputInvalids.push(this.$refs.txtFullName);
        this.tabActive = "general";
      }
      if (!this.commonJs.checkRequired(this.employee.Mobile)) {
        errors.push("Số điện thoại nhân viên không được phép để trống.");
        this.inputInvalids.push(this.$refs.txtMobile);
        this.tabActive = "general";
      }
      if (!this.commonJs.checkRequired(this.employee.User?.UserName)) {
        if (errors.length == 0)
          this.$nextTick(() => {
            this.tabActive = "account";
          });
        errors.push("Tài khoản không được phép để trống.");
        this.inputInvalids.push(this.$refs.txtUserName);
      } else {
        var userNameTemplate = /^[a-zA-Z0-9]+$/;
        const userNameValid = this.employee.User?.UserName.match(userNameTemplate);
        if (!userNameValid) {
          if (errors.length == 0)
            this.$nextTick(() => {
              this.tabActive = "account";
            });
          errors.push("Tên tài khoản không được chứa các kỹ tự đặc biệt.");
          this.inputInvalids.push(this.$refs.txtUserName);
        }
      }

      if (this.$refs.txtPassword) {
        if (!this.commonJs.checkRequired(this.employee.User?.Password)) {
          if (errors.length == 0)
            this.$nextTick(() => {
              this.tabActive = "account";
            });
          errors.push("Mật khẩu không được phép để trống.");
          this.inputInvalids.push(this.$refs.txtPassword);
        }
      }

      if (errors.length > 0) {
        for (const input of this.inputInvalids) {
          if (input) {
            input.setInValidState(true);
          }
        }
        this.commonJs.showMessenger({
          title: "Dữ liệu không hợp lệ",
          msg: errors,
          type: Enum.MsgType.Error,
          confirm: () => {
            this.inputInvalids[0].setFocus();
          },
        });
        return false;
      }
      return true;
    },
    onChangePermission(value, text, item) {
      this.role = item;
    },
    avatarOnClick() {
      this.$refs.fileAvatar.click();
    },
    fileAvatarOnChange(e) {
      const name = e.target.name,
        file = e.target.files[0];
      const hasName = ["avatar"].includes(name);
      if (hasName) {
        // this.avatar = file;
        this.employee.AvatarFullPath = URL.createObjectURL(file);
      }
    },
    getNewCode() {
      this.maxios.get("employees/new-code").then((res) => {
        this.employee.EmployeeCode = res;
      });
    },
  },
  data() {
    return {
      employee: {
        User: { UserName: "", Password: "12345678", RePassword: "12345678" },
      },
      formMode: Enum.FormMode.ADD,
      isValidated: false,
      activeUserInfo: true,
      role: {},
      inputInvalids: [],
      tabActive: "general",
    };
  },
};
</script>
<style scoped>
#frm-detail {
  height: 460px;
  width: 530px;
}
.el-tabs {
  height: 100% !important;
}
.form-group {
}
.form-group + .form-group {
  margin-top: 22px;
}
/* .input-wrapper:first-child {
  margin-top: 0;
} */
input {
  min-width: 200px;
}
.note {
  color: #ff0000;
}

.permission-info {
  margin-top: 10px;
}
.permission-info label {
  font-weight: 700;
  text-decoration: underline;
}
/* @media (max-width: 500px) {
  .form-group {
    width: 100%;
  }
} */
</style>
