<template>
  <div class="m-editor" :class="{ 'full-screen': isFullScreen }">
    <div class="editor-toolbar" v-if="!disabled">
      <div class="--color">
        <label>A</label>
        <input
          type="color"
          @input="editor.chain().focus().setColor($event.target.value).run()"
        />
      </div>

      <button class="--bold" @click="editor.chain().focus().toggleBold().run()"></button>
      <button
        class="--italic"
        @click="editor.chain().focus().toggleItalic().run()"
      ></button>
      <button
        class="--strike"
        @click="editor.chain().focus().toggleStrike().run()"
      ></button>
      <button
        class="align--left"
        @click="editor.chain().focus().setTextAlign('left').run()"
      ></button>
      <button
        class="align--center"
        @click="editor.chain().focus().setTextAlign('center').run()"
      ></button>
      <button
        class="align--right"
        @click="editor.chain().focus().setTextAlign('right').run()"
      ></button>
      <button
        class="align--justify"
        @click="editor.chain().focus().setTextAlign('justify').run()"
      ></button>
      <button class="--add-img" @click="addImage()"></button>
      <button
        class="--quote"
        @click="editor.chain().focus().toggleBlockquote().run()"
      ></button>
      <button
        class="--code"
        @click="editor.chain().focus().toggleCodeBlock().run()"
      ></button>
      <div class="feature-extend">
        <button
          v-if="!isFullScreen"
          @click="changeViewFullScreenEditor"
          class="--fullscreen"
          title="Mở rộng"
        >
          <i class="icofont-maximize"></i>
        </button>
        <button
          title="Thu nhỏ"
          v-else
          @click="changeViewFullScreenEditor"
          class="--exit-fullscreen"
        >
          <i class="icofont-maximize"></i>
        </button>
      </div>
    </div>
    <editor-content
      ref="meditor"
      class="editor-full"
      :editor="editor"
      :disabled="disabled"
    />
    <div class="editor-img__popup" v-if="showSelectImgPopup && showImgPopup">
      <div class="editor-img__popup-wrapper">
        <button
          class="editor-img__popup-button"
          @click="showSelectImgPopup = false"
        ></button>
        <div class="editor-img__popup-body">
          <div class="input-url" v-if="images.length == 0">
            <p>Vui lòng nhập đường dẫn hình ảnh</p>
            <input class="input" ref="inputImageLink" v-model="newImageUrl" type="text" />
          </div>
          <div class="imgs" v-else>
            <img
              class="editor-img-item"
              v-for="(img, index) in images"
              :src="img"
              alt=""
              :key="index"
            />
          </div>
          <p>Hoặc chọn ảnh từ máy tính:</p>
          <input ref="editorSelectFile" type="file" multiple @change="onSelectFile" />
        </div>
        <div class="editor-img__popup-footer">
          <button
            class="btn--accept"
            :disabled="!isNewLinkNotNull"
            @click="approImgSelected"
          >
            Đồng ý
          </button>
          <button class="btn--cancel" @click="showSelectImgPopup = false">Hủy bỏ</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import MEditorVue from "./meditor.js";
export default MEditorVue;
</script>
<style>
@import url(./meditor.css);
</style>
