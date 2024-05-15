<template>
  <div
    class="mnote-item"
    :style="`background-color:${theme?.MainColor}`"
  >
    <button class="mnote__button --delete" @click="onDeleteItem(note)"></button>
    <div
      class="mnote-item-time"
      v-html="commonJs.formatDateTimeHTMLInline(note?.NoteDateTime)"
    ></div>
    <m-editor
      :ref="note.NoteId"
      class="editor"
      :disabled="true"
      v-model="note.Content"
    />
  </div>
</template>
<script>
export default {
  name: "MNoteItem",
  props: ["noteInput"],
  emits: [],
  created() {
    this.note = this.noteInput;
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
  watch: {},
  methods: {},
  data() {
    return { note: {} };
  },
};
</script>
