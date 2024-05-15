<template>
  <MDialogDragable
    v-show="!hideNoteList"
    class="mnote"
    :overlay="false"
    :showFooter="false"
    @click="onNoteListClick"
    @onClose="onCloseNoteList"
    :style="`z-index:${999 + zindex || 0}`"
  >
    <template #header>
      <button class="btn--add-note" @click="onAddNote"></button>
    </template>
    <template #content>
      <div class="note__title">Ghi chú</div>
      <div class="note__search">
        <input
          type="text"
          v-model="keySearch"
          placeholder="Tìm kiếm..."
          class="note__input--search"
        />
      </div>
      <div class="note-list">
        <MNoteItem
          v-for="item in itemsFilter"
          :key="item.NoteId"
          @click="onSelectNote(item)"
          :noteInput="item"
        ></MNoteItem>
      </div>
    </template>
  </MDialogDragable>
  <MNoteDetail
    v-for="note in notesSelected"
    :key="note.NoteId"
    :inputNote="note"
    @onCloseDetail="onCloseItemDetail"
    @onDeleteItemDetail="processAfterDelete"
    @click="onNoteItemDetailClick(note)"
    @onAddNote="onAddNote"
  ></MNoteDetail>
  <div class="mnote--minize" v-if="hideNoteList" @click="hideNoteList = false">
    <button class="mnote__button--icon"></button>
    <div class="mnote__number">{{ items.length }}</div>
  </div>
</template>
<script>
import commonJs from "../../../scripts/common";
import MNoteDetail from "./MNoteDetail.vue";
import debounce from "../../../scripts/debounce";
import MNoteItem from "./MNoteItem.vue";
export default {
  name: "MNote",
  components: { MNoteDetail, MNoteItem },
  props: [],
  emits: [],
  created() {
    this.loadData();
  },
  computed: {},
  watch: {
    keySearch: debounce(function (newValue) {
      var key = this.commonJs.change_alias(newValue);
      this.itemsFilter = this.items.filter((item) =>
        this.commonJs.change_alias(item.Content).includes(key),
      );
    }, 500),
  },
  methods: {
    loadData() {
      this.maxios.get("/notes").then((data) => {
        this.items = data;
        this.itemsFilter = data;
      });
    },
    onAddNote() {
      const employeeId = localStorage.getItem("employeeId");
      var note = { EmployeeId: employeeId };
      this.maxios.post("/notes", note, null, false).then((res) => {
        this.items.push(res);
        const noteExits = this.itemsFilter.find(
          (i) => i["NoteId"] == res.NoteId,
        );
        if (!noteExits) {
          this.itemsFilter.push(res);
        }
        this.notesSelected.push(res);
        this.setZIndexTop(res);
      });
    },
    onSelectNote(item) {
      // Kiểm tra xem note đã được chọn chưa:
      const isExits = commonJs.checkObjectExistInArray(
        this.notesSelected,
        item,
        "NoteId",
      );
      if (!isExits) {
        this.notesSelected.push(item);
      } else {
        const noteMaxZIndex = commonJs.getObjectMaxValueInArray(
          this.notesSelected,
          "OrderZIndex",
        );
        item["OrderZIndex"] = (noteMaxZIndex["OrderZIndex"] || 0) + 1;
      }
    },
    onCloseItemDetail(item) {
      const indexOfItem = this.notesSelected.indexOf(item);
      this.notesSelected.splice(indexOfItem, 1);
    },
    onDeleteItem(item) {
      event.stopPropagation();
      this.commonJs.showConfirm(
        "Bạn có chắc chắn muốn xoá ghi chú này không?",
        () => {
          this.maxios.delete(`/notes/${item?.NoteId}`);
          this.processAfterDelete(item);
        },
      );
    },
    processAfterDelete(item) {
      const indexOfItem = this.items.indexOf(item);
      if (indexOfItem >= 0) {
        this.items.splice(indexOfItem, 1);
      }

      const indexOfItemFilter = this.itemsFilter.indexOf(item);
      if (indexOfItemFilter >= 0) {
        this.itemsFilter.splice(indexOfItemFilter, 1);
      }

      const indexOfItemSelected = this.notesSelected.indexOf(item);
      if (indexOfItemSelected >= 0) {
        this.notesSelected.splice(indexOfItemSelected, 1);
      }
    },

    onNoteItemDetailClick(note) {
      this.setZIndexTop(note);
    },

    setZIndexTop(note) {
      const noteMaxZIndex = commonJs.getObjectMaxValueInArray(
        this.notesSelected,
        "OrderZIndex",
      );
      note["OrderZIndex"] = (noteMaxZIndex["OrderZIndex"] || 0) + 1;
    },
    onNoteListClick() {
      const noteMaxZIndex = commonJs.getObjectMaxValueInArray(
        this.notesSelected,
        "OrderZIndex",
      );
      this.zindex = (noteMaxZIndex?.OrderZIndex || 0) + 1;
    },
    onCloseNoteList() {
      this.hideNoteList = true;
    },
  },
  data() {
    return {
      items: [],
      itemsFilter: [],
      notesSelected: [],
      zindex: 0,
      keySearch: "",
      hideNoteList: true,
    };
  },
};
</script>
<style scoped>
@import url(./mnote.css);
</style>
