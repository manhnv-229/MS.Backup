import EventHandler from "./EventHandler";
import { StarterKit } from "@tiptap/starter-kit";
import { BubbleMenu } from "@tiptap/extension-bubble-menu";
import { Blockquote } from "@tiptap/extension-blockquote";
import TextAlign from "@tiptap/extension-text-align";
// import CodeBlock from "@tiptap/extension-code-block";
import CodeBlockLowlight from "@tiptap/extension-code-block-lowlight";
import { Color } from "@tiptap/extension-color";
import TextStyle from "@tiptap/extension-text-style";
import Image from "@tiptap/extension-image";
import { Editor, EditorContent } from "@tiptap/vue-3";
import toBase64 from "./toBase64";

import css from "highlight.js/lib/languages/css";
import js from "highlight.js/lib/languages/javascript";
import ts from "highlight.js/lib/languages/typescript";
import html from "highlight.js/lib/languages/xml";
// load all highlight.js languages
// import { lowlight } from "lowlight";
import {createLowlight} from 'lowlight'
const lowlight = createLowlight()
lowlight.register(html);
lowlight.register(css);
lowlight.register(js);
lowlight.register(ts);

export default {
  name: "EditorFull",
  autofocus: false,
  components: {
    EditorContent,
  },
  props: {
    modelValue: {
      type: String,
      default: "",
    },
    focus: {
      type: Boolean,
      default: false,
    },
    class: {
      type: String,
      default: "",
    },
    showImgPopup: {
      type: Boolean,
      default: true,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    fullScreen: {
      type: Boolean,
      default: false,
    },
  },

  emits: ["update:modelValue", "onBlur", "onChange", "onFocus"],
  created() {},
  watch: {
    modelValue(value) {
      // HTML
      const isSame = this.editor.getHTML() === value;

      // JSON
      // const isSame = JSON.stringify(this.editor.getJSON()) === JSON.stringify(value)

      if (isSame) {
        return;
      }

      this.editor.commands.setContent(value, false);
    },
    disabled: function (newValue) {
      this.editor.setOptions({ editable: !newValue });
    },
  },
  computed: {
    isNewLinkNotNull: function () {
      const newImageUrl = this.newImageUrl;
      if (newImageUrl != null && newImageUrl?.trim() != "") {
        return true;
      }
      return false;
    },
  },
  mounted() {
    this.editor = new Editor({
      editorProps: {},
      editable: !this.disabled,
      autofocus: false,
      extensions: [
        StarterKit.configure({
          codeBlock: false,
          blockquote: false,
          // BubbleMenu: {
          //   element: document.querySelector(".menu"),
          // },
          // Blockquote: {
          //   HTMLAttributes: {
          //     class: "m-editor-quote",
          //   },
          // },
          // CodeBlockLowlight:{
          //   lowlight,
          //   languageClassPrefix: "language-",
          //   defaultLanguage: "js",
          // },
        }),
        EventHandler,
        TextAlign.configure({
          defaultAlignment: "left",
          types: ["heading", "paragraph"],
          alignments: ["left", "center", "right", "justify"],
        }),
        Image.configure({
          allowBase64: true,
          // EventHandler,
          HTMLAttributes: {
            class: "m-custom-img",
          },
        }),
        BubbleMenu.configure({
          element: document.querySelector(".menu"),
        }),
        Blockquote.configure({
          HTMLAttributes: {
            class: "m-editor-quote",
          },
        }),
        // CodeBlock.configure({
        //   languageClassPrefix: "language-",
        //   exitOnArrowDown: false,
        //   exitOnTripleEnter: false,
        //   HTMLAttributes: {
        //     class: "m-editor-codeblock",
        //   },
        // }),
        CodeBlockLowlight.configure({
          lowlight,
          languageClassPrefix: "language-",
          defaultLanguage: "js",
        }),
        Color.configure({
          types: ["textStyle"],
        }),
        TextStyle,
      ],
      content: this.modelValue,
      onUpdate: () => {
        // HTML
        this.$emit("update:modelValue", this.editor.getHTML());
        this.$emit("onChange");
        // JSON
        // this.$emit('update:modelValue', this.editor.getJSON())
      },
      onFocus: ({ editor, event }) => {
        this.$emit("onFocus", { editor, event });
      },
      onBlur: ({ editor, event }) => {
        this.$emit("onBlur", { editor, event });
      },
    });
  },

  beforeUnmount() {
    this.editor.destroy();
  },
  methods: {
    addImage() {
      this.newImageUrl = null;
      this.showSelectImgPopup = true;
      this.$nextTick(function () {
        let input = this.$refs["inputImageLink"];
        input.focus();
      });
    },
    approImgSelected() {
      const url = this.newImageUrl;
      if (url) {
        this.editor.chain().focus().setImage({ src: url }).run();
        this.showSelectImgPopup = false;
      } else {
        this.$nextTick(function () {
          let input = this.$refs["inputImageLink"];
          input.focus();
        });
      }
    },
    async onSelectFile() {
      console.log(`event`, event);
      let files = event.target.files;
      const acceptedImageTypes = ["image/gif", "image/jpeg", "image/png"];
      // this.images = [];
      if (files) {
        for (const file of files) {
          const fileSize = file.size;
          const type = file.type;
          if (!acceptedImageTypes.includes(type)) {
            this.commonJs.showErrorMessenger(
              "Vui lòng chỉ chọn các tệp ảnh có định dạng .png, .jpg, .gif."
            );
            this.$refs.editorSelectFile.click();
            return false;
          }
          if (fileSize > 500 * 1024) {
            this.commonJs.showErrorMessenger(
              "Vui lòng chọn ảnh có kích thước nhỏ hơn 500kb."
            );
            break;
          }
          let imgBase64 = await toBase64(file);

          const img = {
            type: "image",
            attrs: {
              src: imgBase64,
            },
          };
          this.images.push(img);
        }
        // this.editor.chain().focus().setImage({ src: imgBase64 }).run();
        this.editor.chain().focus().insertContent(this.images).run();
        // this.$nextTick(function(){
        //   this.editor.chain().focus().insertContent("nvmanh").run();
        // })
        this.showSelectImgPopup = false;
      }
    },
    async uploadImage(selectedFile) {
      // Restricts maxAllowedFileSizeMb to 5
      if (selectedFile.size / 1024 / 1024 > 5) {
        // handle error
        return;
      }
      console.log("uploadImage: ", selectedFile);
      // Check the allowed content type
      if (
        !["image/jpeg", "image/png", "image/gif"].includes(selectedFile.type)
      ) {
        // handle error
        return;
      }

      // Optionally you can set a state here so as to toggle the editor to readonly mode until the image is uploaded.
      this.imageUploadInProgress = true;
    },
    setFocus() {
      this.$nextTick(function () {
        this.editor.commands.focus();
      });
    },
    changeViewFullScreenEditor() {
      this.isFullScreen = !this.isFullScreen;
      this.$nextTick(function () {
        this.editor.commands.focus();
      });
    },
  },

  data() {
    return {
      editor: null,
      showSelectImgPopup: false,
      images: [],
      newImageUrl: null,
      isFullScreen: false,
    };
  },
};
