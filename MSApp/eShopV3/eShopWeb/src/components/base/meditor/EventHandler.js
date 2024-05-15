/* eslint-disable no-unused-vars */
// import { Plugin } from '@tiptap/pm/state'
// import Image from "@tiptap/extension-image";
/**
 * Plugin for the tiptap editor that adds images to the editor
 * @see https://github.com/scrumpy/tiptap/blob/0f17abeee6df1a8b40c6c96413a158918ec45d34/packages/tiptap-extensions/src/nodes/Image.js
 * This class overwrites the default `image`. You need to make sure to **not** use the original class.
 */
// export default class ImageUpload extends Image {
//   constructor(options = {}) {
//     super(options);
//     this.uploader = options.uploader;
//   }
//   get name() {
//     return "image";
//   }
//   get plugins() {
//     const uploader = this.uploader;
//     return [
//       new Plugin({
//         // key: new PluginKey('eventHandler'),
//         props: {
//           handleDOMEvents: {
//             paste(view, event) {
//               const items = (
//                 event.clipboardData || event.originalEvent.clipboardData
//               ).items;
//               if (!uploader) {
//                 return;
//               }
//               items.forEach(async (item) => {
//                 const { schema } = view.state;
//                 const image = item.getAsFile();
//                 // Return here, otherwise copying texts won't possible anymore
//                 if (!image) {
//                   return;
//                 }
//                 event.preventDefault();
//                 const imageSrc = await uploader(image);
//                 const node = schema.nodes.image.create({
//                   src: imageSrc,
//                 });
//                 const transaction = view.state.tr.replaceSelectionWith(node);
//                 view.dispatch(transaction);
//               });
//             },
//             drop(view, event) {
//               const hasFiles = event.dataTransfer.files.length > 0;
//               if (!hasFiles) {
//                 return;
//               }
//               event.preventDefault();
//               const images = event.dataTransfer.files;
//               const { schema } = view.state;
//               const coordinates = view.posAtCoords({
//                 left: event.clientX,
//                 top: event.clientY,
//               });
//               images.forEach(async (image) => {
//                 if (!uploader) {
//                   return;
//                 }
//                 const imageSrc = await uploader(image);
//                 if (imageSrc) {
//                   const node = schema.nodes.image.create({
//                     src: imageSrc,
//                   });
//                   const transaction = view.state.tr.insert(
//                     coordinates.pos,
//                     node
//                   );
//                   view.dispatch(transaction);
//                 }
//               });
//             },
//           },
//         },
//       }),
//     ];
//   }
// }

import { Extension } from "@tiptap/core";
import { Plugin, PluginKey } from "@tiptap/pm/state";
import toBase64 from "./toBase64";
import { EditorState, NodeSelection } from "prosemirror-state";
const EventHandler = Extension.create({
  name: "eventHandler",
  addProseMirrorPlugins() {
    return [
      new Plugin({
        key: new PluginKey("eventHandler"),
        props: {
          handleDOMEvents: {
            /**
             * paster ảnh vào editor
             * @param {*} view
             * @param {*} event
             * @returns
             */
            async paste(view, event) {
              var items = (
                event.clipboardData || event.originalEvent.clipboardData
              ).items;
              const itemCopys = [...items];
              // if (!uploader) {
              //   return;
              // }
              for (const item of itemCopys) {
                console.log(`itemCopys`, itemCopys);
                const { schema } = view.state;
                const image = item.getAsFile();
                const imageCopy = Object.assign({}, image);
                // Return here, otherwise copying texts won't possible anymore
                if (!image) {
                  return;
                }

                // Chuyển sang base64 rồi đẩy vào editor - cách này paster được nhiều ảnh:
                var filereader = new FileReader();
                filereader.readAsDataURL(image);
                filereader.onload = function (evt) {
                  var base64 = evt.target.result;
                  const node = schema.nodes.image.create({
                    src: base64,
                  });
                  const tr = view.state.tr;
                  tr.replaceSelectionWith(node);
                 
                  // let nodeSize = tr.selection.$anchor.nodeBefore?.nodeSize;
                  // const resolvedPos = tr.doc.resolve(
                  //   tr.selection.anchor - nodeSize
                  // );
                  // tr.setSelection(new NodeSelection(resolvedPos));

                  // const pos = view.state.selection.to;
                  // const curSelection = view.state.selection;
                  // const transaction = view.state.tr.replaceSelectionWith(node);
                  // const transaction = view.state.tr.insert(pos, node);
                  // transaction.setSelection(transaction);
                  // const transactionP = view.state.tr.insertText("nvmanh");

                  view.dispatch(tr);
                  view.focus();
                };

                // Cách cũ - không paster được nhiều ảnh do gọi đến even chuyển base 64 thì mất con trỏ this
                // const imageBase64 = await toBase64(image);
                // console.log(`filereader`, filereader);
                // // const imageSrc = await uploader(image);
                // const node = schema.nodes.image.create({
                //   // src: imageSrc,
                //   src: filereader.result,
                // });
                // let transaction = view.state.tr.replaceSelectionWith(node);
                // // const transactionP = view.state.tr.insertText("nvmanh");
                // view.dispatch(transaction);
                // event.preventDefault();
              }
              event.preventDefault();
            },
            /**
             * Thực hiện kéo thả ảnh vào editor
             * @param {*} view
             * @param {*} event
             * @returns
             */
            async drop(view, event) {
              const hasFiles = event.dataTransfer.files.length > 0;
              if (!hasFiles) {
                return;
              }
              event.preventDefault();
              const images = event.dataTransfer.files;
              const { schema } = view.state;
              const coordinates = view.posAtCoords({
                left: event.clientX,
                top: event.clientY,
              });
              for (const item of images) {
                const imageBase64 = await toBase64(item);
                // const imageSrc = await uploader(image);
                const node = schema.nodes.image.create({
                  // src: imageSrc,
                  src: imageBase64,
                });
                const transaction = view.state.tr.replaceSelectionWith(node);
                view.dispatch(transaction);
              }
            },
          },
          // … and many, many more.
          // Here is the full list: https://prosemirror.net/docs/ref/#view.EditorProps
        },
      }),
    ];
  },
});
export default EventHandler;
