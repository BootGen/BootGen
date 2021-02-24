<template>
  <div>
    <v-treeview
      class="text-break"
      active-class="info--text"
      :items="tree.children"
      :open.sync="tree.open"
      @update:active="selectFile"
      return-object
      activatable
      hoverable
      dense
      open-on-click
    ></v-treeview>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { GeneratedFile } from "../models/GeneratedFile";

interface Node {
  id: number;
  name: string;
  children?: Array<Node>;
}

export default Vue.extend({
  props: {
    files: {
      type: Array as () => GeneratedFile[],
    },
    openPath: {
      type: String,
    },
  },
  watch: {
    openPath: {
      handler(openPath: string) {
        this.init();
      },
    },
  },
  data: function () {
    return {
      tree: {
        id: 0,
        name: "Root",
        children: Array<Node>(),
        open: Array<Node>(),
      },
      filesById: Array<GeneratedFile>(),
      id: 0,
    };
  },
  created: function () {
    const files = this.sortFiles();
    files.forEach((file) => this.addToTree(file));
    this.init();
  },
  methods: {
    init: function () {
      if (this.openPath) {
        this.tree.open = [];
        this.setOpenFolder(this.openPath.split("/"), this.tree.children);
      }
    },
    getChild: function (node: Node, name: string): Node | null {
      if (!node.children) {
        return null;
      }
      for (let j = 0; j < node.children.length; j++) {
        if (node.children[j] && node.children[j].name === name) {
          return node.children[j];
        }
      }
      return null;
    },
    addToTree: function (file: GeneratedFile) {
      const path = file.path.split("/");
      let currentNode: Node = this.tree;
      if (path[0] !== "") {
        path.forEach((part) => {
          const childNode = this.getChild(currentNode, part);
          if (!childNode) {
            const node: Node = { id: ++this.id, name: part };
            if (!currentNode.children) {
              currentNode.children = [node];
            } else {
              currentNode.children.push(node);
            }
            currentNode = node;
          } else {
            currentNode = childNode;
          }
        });
      }
      if (!currentNode.children) {
        currentNode.children = [{ id: ++this.id, name: file.name }];
      } else {
        currentNode.children.push({ id: ++this.id, name: file.name });
      }
      this.filesById[this.id] = file;
    },
    setOpenFolder: function (path: string[], node: Node[]) {
      if (!this.tree.open) {
        this.tree.open = [];
      }
      for (let j = 0; j < node.length; j++) {
        if (path[0] === node[j].name) {
          this.tree.open.push(node[j]);
          path.shift();
          const nodeChildren = node[j].children;
          if (nodeChildren && path.length > 0) {
            this.setOpenFolder(path, nodeChildren);
          }
        }
      }
    },
    getFile: function (node: Node) {
      return this.filesById[node.id];
    },
    selectFile: function (node: Node[]) {
      if (node[0]) {
        this.$emit("select-file", this.getFile(node[0]));
      }
    },
    sortFiles: function () {
      this.files.sort(function (a: GeneratedFile, b: GeneratedFile) {
        if (a.path === "") {
          return b.path.localeCompare(a.path) || a.name.localeCompare(b.name);
        }
        if (b.path === "") {
          return b.path.localeCompare(a.path) || a.name.localeCompare(b.name);
        }
        return a.path.localeCompare(b.path) || a.name.localeCompare(b.name);
      });
      return this.files;
    },
  },
});
</script>
