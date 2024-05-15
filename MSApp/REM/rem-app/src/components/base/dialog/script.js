export default {
  name: "MDialogDragable",
  emits: ["onSubmit", "onClose"],
  props: {
    title: {
      type: String,
      required: false,
    },
    headerBgColor: {
      type: String,
      required: false,
    },
    contentBgColor: {
      type: String,
      required: false,
    },
    footerBgColor: {
      type: String,
      required: false,
    },
   
    showButton: {
      type: Boolean,
      default: false,
      required: false,
    },
    submitText: {
      type: String,
      default: "Lưu",
      required: false,
    },
    width: {
      type: String,
      required: false,
    },
    height: {
      type: String,
      required: false,
    },
    overlay: {
      type: Boolean,
      default: true,
      required: false,
    },
    showHeader:{
      type: Boolean,
      default: true,
      required: false,
    },
    showFooter: {
      type: Boolean,
      default: true,
      required: false,
    },
    dragable: {
      type: Boolean,
      default: true,
      required: false,
    },
  },

  created() {},
  computed: {},
  watch: {},
  methods: {
    onSubmit() {
      this.$emit("onSubmit");
    },
    onClose() {
      this.$emit("onClose");
    },
    setZIndex(value) {
      this.zindex + value;
    },
  },
  data() {
    return {
      zindex: 0,
    };
  },
};
