<template>
  <MDialogDragable
    class="mnote-detail"
    :ref="`mdialog_${note.NoteId}`"
    :overlay="false"
    :showFooter="false"
    :style="`z-index:${999 + note.OrderZIndex || 0}`"
    :headerBgColor="theme.HeaderColor"
    :contentBgColor="theme.MainColor"
    @onClose="onClose"
  >
    <template #header>
      <button class="btn--add-note" @click="onAddNewNote"></button>
      <button class="btn--more" @click="onShowMoreAction"></button>
      <div class="note-info">
        {{ commonJs.convertDateTimeToStringDateTime(note.CreatedDate) }}
      </div>
      <div
        class="note-toolbar"
        v-if="showMoreAction"
        v-clickoutside="hideMoreAction"
      >
        <div class="list-color">
          <button
            v-for="(color, index) in colors"
            :key="index"
            class="btn-color"
            :class="[
              { '--selected': note.ThemeColor == color.MainColor },
              `--${color.MainColor}`,
            ]"
            :style="{ backgroundColor: color.MainColor }"
            @click="onSelectColor(color)"
          ></button>
        </div>
        <button class="btn--showlist" @click="onShowList">
          Danh sách ghi chú
        </button>
        <button class="btn--delete" @click="onDeleteNote">Xoá ghi chú</button>
      </div>
    </template>
    <template #content>
      <div class="note-detail">
        <m-editor
          ref="txtNoteContent"
          class="editor"
          :disabled="false"
          @onChange="onChangeContent"
          v-model="note.Content"
        />
      </div>
    </template>
  </MDialogDragable>
</template>
<script>
/* eslint-disable */
/**
 * Gán sự kiện nhấn click chuột ra ngoài combobox data (ẩn data list đi)
 * NVMANH (31/07/2022)
 */
const clickoutside = {
  mounted(el, binding, vnode, prevVnode) {
    el.clickOutsideEvent = (event) => {
      // Nếu element hiện tại không phải là element đang click vào
      // Hoặc element đang click vào không phải là button trong combobox hiện tại thì ẩn đi.
      if (
        !(
          (
            el === event.target || // click phạm vi của combobox__data
            el.contains(event.target) || //click vào element con của combobox__data
            el.previousElementSibling.contains(event.target) ||
            el.previousElementSibling.previousElementSibling.contains(
              event.target,
            )
          ) //click vào element button trước combobox data (nhấn vào button thì hiển thị)
        )
      ) {
        // Thực hiện sự kiện tùy chỉnh:
        binding.value(event, el);
        // vnode.context[binding.expression](event); // vue 2
      }
    };
    document.body.addEventListener("click", el.clickOutsideEvent);
  },
  beforeUnmount: (el) => {
    document.body.removeEventListener("click", el.clickOutsideEvent);
  },
};
import debounce from "@/scripts/debounce";

export default {
  name: "MNoteDetail",
  props: ["inputNote"],
  directives: {
    clickoutside,
  },
  computed: {
    theme: function () {
      const theme = this.note?.ThemeColor;
      if (theme) {
        try {
          return JSON.parse(theme);
        } catch (error) {
          console.log(error);
          return {};
        }
      } else {
        return {};
      }
    },
  },
  emits: ["onCloseDetail", "onDeleteItemDetail", "onAddNote"],
  created() {
    this.note = this.inputNote;
  },
  methods: {
    onAddNewNote() {
      this.$emit("onAddNote");
    },
    onShowMoreAction() {
      this.showMoreAction = !this.showMoreAction;
    },
    onShowList() {
      this.$emit("onCloseDetail");
    },
    onDeleteNote() {
      this.commonJs.showConfirm(
        "Bạn có chắc chắn muốn xoá ghi chú này không?",
        () => {
          this.maxios.delete(`/notes/${this.note?.NoteId}`);
          this.$emit("onDeleteItemDetail", this.note);
        },
      );
    },
    onClose() {
      this.$emit("onCloseDetail", this.note);
      // Xoá ghi chú nếu rỗng nội dung:
      const element = document.createElement("div");
      element.innerHTML = this.note?.Content || "";
      if (element.textContent === "" && this.note?.NoteId) {
        this.maxios.delete(`/notes/${this.note?.NoteId}`);
        this.$emit("onDeleteItemDetail", this.note);
      }
    },
    onChangeContent: debounce(function () {
      if (!this.note?.NoteId) {
        this.maxios.post("/notes", this.note, null, true).then((res) => {
          this.note = res;
        });
      } else {
        this.maxios
          .put(`/notes/${this.note.NoteId}`, this.note, null, true)
          .then((res) => {
            console.log(res);
          });
      }
    }, 500),
    onSelectColor(color) {
      this.note.ThemeColor = JSON.stringify(color);
      this.showMoreAction = !this.showMoreAction;
      this.maxios.put(`/notes/${this.note.NoteId}`, this.note, null, true);
    },
    hideMoreAction() {
      this.showMoreAction = false;
    },
  },
  data() {
    return {
      note: {},
      showMoreAction: false,
      colors: [
        { HeaderColor: "#196519", MainColor: "green", ThemeType: 1 },
        { HeaderColor: "#ffe48a", MainColor: "#fff7d1", ThemeType: 1 },
        { HeaderColor: "#c255c2", MainColor: "violet", ThemeType: 1 },
        { HeaderColor: "#4b4baf", MainColor: "blue", ThemeType: 1 },
        { HeaderColor: "#a0a040", MainColor: "yellow", ThemeType: 1 },
        { HeaderColor: "#626161", MainColor: "gray", ThemeType: 1 },
      ],
      zindex: 0,
    };
  },
};
</script>
<style>
@import url(./mnote.css);
</style>
