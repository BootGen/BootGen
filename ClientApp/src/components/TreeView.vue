<template>
  <div>
    <v-treeview
      active-class="activeFile"
      :items="tree.children"
      :active.sync="activeNodes"
      :open.sync="tree.open"
      @update:active="selectFile"
      return-object
      activatable
      hoverable
      dense
      open-on-click
    >
     <template v-slot:prepend="{ item, open }">
        <v-icon v-if="item.type == 'folder'">
          {{ open ? 'mdi-folder-open' : 'mdi-folder' }}
        </v-icon>
        <v-icon v-else>
          {{ icons[item.type] }}
        </v-icon>
      </template>
    </v-treeview>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import { GeneratedFile } from '../models/GeneratedFile';

interface Node {
  id: number;
  name: string;
  type: string;
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
    files: function () {
      this.init();
    },
    openPath: function() {
      this.setOpenPath();
    }
  },
  data: function () {
    return {
      tree: {
        id: 0,
        name: 'Root',
        type: '',
        children: Array<Node>(),
        open: Array<Node>(),
      },
      activeNodes: Array<Node>(),
      filesById: Array<GeneratedFile>(),
      id: 0,
      icons: {
        cs: 'mdi-language-csharp ',
        vue: 'mdi-vuejs',
        ts: 'mdi-language-typescript',
        yml: 'mdi-api',
      },
    };
  },
  created: function () {
    this.init();
    this.tree.children.forEach(node => {
      if (node.name === 'restapi.yml') {
        this.activeNodes.push(node);
      }
    });
  },
  methods: {
    init: function () {
      this.tree.children = [];
      this.tree.open = [];
      const files = this.sortFiles();
      files.forEach((file) => this.addToTree(file));
      this.setOpenPath();
    },
    setOpenPath: function() {
      if (this.openPath) {
        this.tree.open = [];
        this.setOpenFolder(this.openPath.split('/'), this.tree.children);
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
      const path = file.path.split('/');
      const type = file.name.split('.')[file.name.split('.').length-1];
      let currentNode: Node = this.tree;
      if (path[0] !== '') {
        path.forEach((part) => {
          const childNode = this.getChild(currentNode, part);
          if (!childNode) {
            const node: Node = { id: ++this.id, name: part, type: 'folder' };
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
        currentNode.children = [{ id: ++this.id, name: file.name, type: type }];
      } else {
        currentNode.children.push({ id: ++this.id, name: file.name, type: type });
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
    getFile: function (node: Node): GeneratedFile {
      return this.filesById[node.id];
    },
    selectFile: function (node: Node[]) {
      if (node[0]) {
        const file = this.getFile(node[0]);
        this.$gtag?.event('select-file', {
              'event_label' : file.name
        });
        this.$emit('select-file', file);
      }
    },
    sortFiles: function (): Array<GeneratedFile> {
      this.files.sort(function (a: GeneratedFile, b: GeneratedFile) {
        if (a.path === '') {
          return b.path.localeCompare(a.path) || a.name.localeCompare(b.name);
        }
        if (b.path === '') {
          return b.path.localeCompare(a.path) || a.name.localeCompare(b.name);
        }
        return a.path.localeCompare(b.path) || a.name.localeCompare(b.name);
      });
      return this.files;
    },
  },
});
</script>

<style>
  .activeFile .v-treeview-node__content{
    color: #412fb3!important;
    font-weight: 900;
  }
  .activeFile .v-icon{
    color: #412fb3!important;
  }
  button.v-icon.notranslate {
    display: none;
  }
</style>