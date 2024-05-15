<template>
  <div class="tab">
    <m-button-icon iconCls="icon--add" @click="onAddNewNavbar"
      >Thêm mới</m-button-icon
    >
    <div class="navbar-list--setting">
      <draggable
        class="navbar-group"
        :list="organization.NavbarItem"
        :animation="300"
        item-key="NavbarItemId"
        @change="onChangeNavbarItem"
        group="nav"
      >
        <template #item="{ element: navItem, index }">
          <a
            class="navbar-item"
            @click="updateNavItem(navItem)"
            :class="{
              '--deleted': navItem.EntityState === Enum.EntityState.DELETE,
            }"
          >
            <div class="navbar-item__main">
              <a class="nav__container">
                <span
                  v-if="navItem.UseIconFont"
                  class="navbar-item__icon-font"
                  v-html="navItem.IconFont"
                ></span>
                <div
                  v-else
                  class="nav-item__icon"
                  :class="navItem.IconCls"
                ></div>
                <span class="item__text-label">{{ navItem.Label }}</span>
                <div class="item__index">{{ navItem.Sort }}</div>
              </a>
              <button
                class="nav__action nav__button--delete icon icon-16 icon--remove"
                v-if="navItem.EntityState !== Enum.EntityState.DELETE"
                @click="onDeleteMenuItem(navItem)"
              ></button>
              <button
                class="nav__action nav__button--undo icon icon-16 icon--undo30"
                v-if="navItem.EntityState === Enum.EntityState.DELETE"
                @click="onUndoDelete(navItem)"
              ></button>
            </div>
            <div
              class="nav-sub"
              :class="{
                '--deleted': navItem.EntityState === Enum.EntityState.DELETE,
              }"
            >
              <draggable
                class="navbar-group-sub"
                @click="onSubGroupClick(navItem)"
                :list="navItem.NavbarSubItems"
                :animation="300"
                item-key="NavbarItemId"
                @change="onChangeNavbarItem"
                group="nav"
              >
                <template #item="{ element: navSubItem, index }">
                  <a
                    class="navbar-item"
                    @click="updateSubNavItem(navSubItem, navItem)"
                  >
                    <div class="nav__container">
                      <span
                        v-if="navSubItem.UseIconFont"
                        class="navbar-item__icon-font"
                        v-html="navSubItem.IconFont"
                      ></span>
                      <div
                        v-else
                        class="nav-item__icon"
                        :class="navSubItem.IconCls"
                      ></div>
                      <span class="item__text-label">{{
                        navSubItem.Label
                      }}</span>
                      <div class="item__index">
                        {{ navSubItem.Sort }}
                      </div>
                    </div>

                    <button
                      class="nav__action nav__button--delete icon icon-16 icon--remove"
                      @click="onDeleteMenuItem(navSubItem)"
                      v-if="
                        navSubItem.EntityState !== Enum.EntityState.DELETE &&
                        navItem.EntityState !== Enum.EntityState.DELETE
                      "
                    ></button>
                    <button
                      class="nav__action nav__button--undo icon icon-16 icon--undo30"
                      v-if="
                        navSubItem.EntityState === Enum.EntityState.DELETE &&
                        navItem.EntityState !== Enum.EntityState.DELETE
                      "
                      @click="onUndoDelete(navSubItem)"
                    ></button>
                  </a>
                </template>
              </draggable>
            </div>
            <!--v-if-->
          </a>
        </template>
      </draggable>
    </div>
  </div>
  <NavbarItem
    v-if="showNavbarForm"
    :inputObject="navItemUpdate"
    @onClose="onCloseForm"
    @onSaveNavbarItemSuccess="onSaveNavbarItemSuccess"
  >
  </NavbarItem>
</template>
<script>
import draggable from "vuedraggable";
import store from "@/store";
import { UPDATE_ORGANIZATION_INFO } from "@/store/actions/signalR";
import NavbarItem from "./NavbarItemForm.vue";
export default {
  name: "NavbarSetting",
  props: [],
  emits: [],
  components: { draggable, NavbarItem },
  inject: ["organization"],
  created() {
    console.log("org:", this.organization);
  },
  computed: {
    dragOptions() {
      return {
        animation: 200,
        group: "description",
        disabled: false,
        ghostClass: "ghost",
      };
    },
  },
  data() {
    return {
      showNavbarForm: false,
      navbarItem: { UseIconFont: false, IsSytem: false },
      drag: false,
      navItemUpdate: {},
    };
  },
  methods: {
    onUndoDelete(navItem) {
      event.stopPropagation();
      this.commonJs.showConfirm(`Khôi phục <b>${navItem.Label}</b>?`, () => {
        navItem.EntityState = this.Enum.EntityState.UPDATE;
        for (const item of navItem.NavbarSubItems) {
          item.EntityState = this.Enum.EntityState.UPDATE;
        }
      });
    },
    onDeleteMenuItem(navItem) {
      event.stopPropagation();
      this.commonJs.showConfirm(
        `Xóa <b>${navItem.Label}</b> khỏi danh sách?`,
        () => {
          navItem.EntityState = this.Enum.EntityState.DELETE;
          for (const item of navItem.NavbarSubItems) {
            item.EntityState = this.Enum.EntityState.DELETE;
          }
        }
      );
    },
    onSubGroupClick(navItem) {
      this.showNavbarForm = true;
      this.navItemUpdate = { ParentId: navItem.NavbarItemId };
      event.stopPropagation();
    },
    updateSubNavItem(navSubItem, navItem) {
      this.navItemUpdate = navSubItem;
      this.showNavbarForm = true;
      event.stopPropagation();
    },
    updateNavItem(navItem) {
      this.navItemUpdate = navItem;
      this.showNavbarForm = true;
      event.stopPropagation();
    },
    onSaveNavbarItemSuccess(item) {
      if (item.EntityState === this.Enum.EntityState.ADD) {
        if (!item.ParentId) {
          this.organization.NavbarItem.push(item);
        } else {
          var parent = this.organization.NavbarItem.find(
            (i) => i.NavbarItemId === item.ParentId
          );
          parent.EntityState = this.Enum.EntityState.UPDATE;
          parent.NavbarSubItems.push(item);
        }
      }
      //   store.dispatch(UPDATE_ORGANIZATION_INFO);
    },
    onAddNavbarItem(evt) {
      window.console.log("add:", evt);
    },
    onChangeNavbarItem(evt) {
      const moved = evt.moved;
      const added = evt.added;
      const removed = evt.removed;
      let navBarItem = {};
      if (moved) {
        navBarItem = moved.element;
        const newIndex = moved.newIndex;
        const oldIndex = moved.oldIndex;
        window.console.log("moved:", moved);
      } else if (added) {
        window.console.log("added:", added);
        navBarItem = added.element;
        const newIndex = added.newIndex;
      } else if (removed) {
        window.console.log("removed:", removed);
        navBarItem = removed.element;
        navBarItem.ParentId = null;
        const oldIndex = removed.oldIndex;
      } else {
        window.console.log(evt);
      }
      navBarItem.EntityState = this.Enum.EntityState.UPDATE;
      this.updateIndex(navBarItem);
      // console.log(this.organization.NavbarItem);
    },
    updateIndex(navBarItem, newIndex, oldIndex) {
      const parentId = navBarItem.ParentId;
      let items = [];
      if (parentId === null) {
        items = this.organization.NavbarItem;
      } else {
        const navbarItem = this.organization.NavbarItem.find(
          (item) => item.NavbarItemId === parentId
        );
        items = navbarItem.NavbarSubItems;
      }

      for (let index = 0; index < items.length; index++) {
        items[index].Sort = index;
        // Chỉ update cho các item không ở trạng thái xóa:
        if (items[index].EntityState !== this.Enum.EntityState.DELETE) {
          items[index].EntityState = this.Enum.EntityState.UPDATE;
        }
      }
      console.log(items);
    },
    onAddNewNavbar() {
      this.showNavbarForm = true;
      this.navItemUpdate = {};
    },
    onCloseForm() {
      this.showNavbarForm = false;
      this.navItemUpdate = {};
    },
  },
};
</script>
<style scoped>
.navbar-item {
  border: 1px solid #ddd;
  height: 30px;
  background-color: #000;
  color: #fff;
  padding: 0;
}

.navbar-list--setting {
  position: relative;
  width: 250px;
  margin: 12px;
}
.item__text-label {
  flex: 1;
}
.navbar-list--setting div > .navbar-item {
  padding: 0;
  flex-direction: column;
  height: unset;
  background-color: #fff;
  border: unset;
}

.navbar-list--setting .navbar-item__main {
  width: 100%;
  flex: 0 0 30px;
  box-sizing: border-box;
  border-bottom: 1px solid #fff;
}

.navbar-list--setting .nav-sub {
  padding-top: 0;
  position: relative;
  padding-left: 10px;
  padding-right: 0px;
  width: 100%;
  left: 0;
  top: unset;
  background-color: #fff;
  box-sizing: border-box;
  z-index: 99;
}

.navbar-list--setting .nav-sub .navbar-item {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  column-gap: 2px;
  flex-direction: row;
  height: 30px;
  padding: 0;
  border-bottom: 1px solid #fff;
}

.navbar-list--setting .navbar-item__main:hover,
.navbar-list--setting .nav-sub .navbar-item:hover {
  position: relative;
  left: -4px;
}

.navbar-item__main {
  display: flex;
  align-items: center;
  column-gap: 4px;
}

.navbar-item__main .nav__container {
  flex: 1;
  display: flex;
  padding: 0 12px;
  align-items: center;
  height: 100%;
  background-color: #000;
}

.navbar-group-sub:after,
.navbar-group-sub:empty {
  content: "";
  display: block;
  width: 100%;
  /* background-color: #ddd; */
  min-height: 28px;
  background-image: url(../../assets/icon/add-50.png);
  background-position: 12px center;
  background-repeat: no-repeat;
  background-size: 16px;
  border: 1px dashed #dedede;
  box-sizing: border-box;
}

.navbar-group-sub:empty::after {
  content: unset;
}

.navbar-group {
  display: flex;
  row-gap: 10px;
  flex-direction: column;
}

.nav-sub:empty {
}

.nav-sub .nav__container {
  background-color: #3f3f3f;
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex: 1;
  height: 100%;
  padding: 0 12px;
}

.nav__action {
  width: 30px;
  height: 30px;
  flex: 0 0 30px;
  cursor: pointer;
  background-color: unset;
  border: 1px dashed #dedede;
}

.nav__button--undo {
  pointer-events: all !important;
}

.--deleted {
  text-decoration: line-through;
  pointer-events: none;
  color: #ff0000;
}
</style>