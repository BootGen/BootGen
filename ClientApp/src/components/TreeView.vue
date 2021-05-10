<template>
  <v-treeview
    dark
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
      <v-icon color="#eee" v-if="item.type == 'folder'">
        {{ open ? 'mdi-chevron-down ' : 'mdi-chevron-right' }}
      </v-icon>
      <v-icon :color="color[item.type]" v-else>
        {{ icons[item.type] }}
      </v-icon>
    </template>
    <template v-slot:append="{ item }">
      <p class="purple--text pa-0 ma-0 mr-1" v-if="item.change == ChangeType.Folder"><v-icon color="orange">mdi-circle-medium </v-icon></p>
      <p class="purple--text pa-0 ma-0 mr-2" v-if="item.change == ChangeType.Edited && activeNodes[0] == item">M</p>
      <p class="orange--text pa-0 ma-0 mr-2" v-else-if="item.change == ChangeType.Edited">M</p>
      <p class="purple--text pa-0 ma-0 mr-2" v-if="item.change == ChangeType.Created  && activeNodes[0] == item">A</p>
      <p class="green--text pa-0 ma-0 mr-2" v-else-if="item.change == ChangeType.Created">A</p>
    </template>
    <template v-slot:label="{ item }">
      <p class="orange--text pa-0 ma-0" v-if="item.change == ChangeType.Folder">{{item.name}}</p>
      <p class="purple--text pa-0 ma-0" v-if="item.change == ChangeType.Edited && activeNodes[0] == item">{{item.name}}</p>
      <p class="orange--text pa-0 ma-0" v-else-if="item.change == ChangeType.Edited">{{item.name}}</p>
      <p class="purple--text pa-0 ma-0" v-if="item.change == ChangeType.Created && activeNodes[0] == item">{{item.name}}</p>
      <p class="green--text pa-0 ma-0" v-else-if="item.change == ChangeType.Created">{{item.name}}</p>
      <p class="purple--text pa-0 ma-0" v-if="item.change == ChangeType.Unchanged && activeNodes[0] == item">{{item.name}}</p>
      <p class="pa-0 ma-0" v-else-if="item.change == ChangeType.Unchanged">{{item.name}}</p>
    </template>
  </v-treeview>
</template>

<script lang="ts">
import Vue from 'vue';
import { GeneratedFile } from '../models/GeneratedFile';

enum ChangeType {
  Created,
  Edited,
  Folder,
  Unchanged,
}

interface Node {
  id: number;
  name: string;
  type: string;
  change: ChangeType;
  children?: Array<Node>;
}

export default Vue.extend({
  props: {
    files: {
      type: Array as () => GeneratedFile[],
    },
    previousFiles: {
      type: Array as () => GeneratedFile[],
    },
    openPath: {
      type: String,
    },
    isCompare: Boolean,
  },
  watch: {
    files: function () {
      this.init();
    },
    openPath: function() {
      this.setOpenPath();
    },
    isCompare: function() {
      this.init();
    }
  },
  data: function () {
    return {
      tree: {
        id: 0,
        name: 'Root',
        type: '',
        change: ChangeType.Unchanged,
        children: Array<Node>(),
        open: Array<Node>(),
      },
      activeNodes: Array<Node>(),
      filesById: Array<GeneratedFile>(),
      id: 0,
      color: {
        cs: '#82AAFF',
        vue: '#C3E88D',
        ts: '#82AAFF',
        yml: '#F07178',
      },
      icons: {
        cs: 'mdi-language-csharp',
        vue: 'mdi-vuejs',
        ts: 'mdi-language-typescript',
        yml: 'mdi-api',
      },
      ChangeType: ChangeType,
    };
  },
  created: function () {
    this.init();
    this.setDefaultNodes();
  },
  methods: {
    init: function () {
      this.tree.children = [];
      this.tree.open = [];
      const files = this.sortedFiles();
      if(this.isCompare){
        const changedFiles: {file: GeneratedFile; changes: ChangeType}[] = [];
        if(this.previousFiles.length > 0){
          files.forEach(file => {
            let exists = false;
            for(let j = 0; j < this.previousFiles.length; j++){
              if(file.name === this.previousFiles[j].name && file.path === this.previousFiles[j].path){
                if(file.content !== this.previousFiles[j].content){
                  changedFiles.push({file: file, changes: ChangeType.Edited});
                }else{
                  changedFiles.push({file: file, changes: ChangeType.Unchanged});
                }
                exists = true;
                break;
              }
            }
            if(!exists){
              changedFiles.push({file: file, changes: ChangeType.Created});
            }
          });
        }
        if(this.previousFiles.length > 0){
          changedFiles.forEach((node) => {
            this.addToTree(node.file, node.changes);
          });
        }else{
          files.forEach((file) => {
            this.addToTree(file, ChangeType.Unchanged);
          });
        } 
      }else{
        files.forEach((file) => {
          this.addToTree(file, ChangeType.Unchanged);
        });
      } 
      this.setOpenPath();
    },
    setDefaultNodes: function(){
      this.tree.children.forEach(node => {
        if (node.name === 'restapi.yml') {
          this.activeNodes.push(node);
        }
      });
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
    addToTree: function (file: GeneratedFile, change: ChangeType) {
      const path = file.path.split('/');
      const type = file.name.split('.')[file.name.split('.').length-1];
      let currentNode: Node = this.tree;
      if (path[0] !== '') {
        path.forEach((part) => {
          const childNode = this.getChild(currentNode, part);
          if (!childNode) {
            const node: Node = { id: ++this.id, name: part, type: 'folder', change: change };
            if (!currentNode.children) {
              currentNode.children = [node];
            } else {
              currentNode.children.push(node);
            }
            currentNode = node;
          } else {
            currentNode = childNode;
          }
          if(change !== ChangeType.Unchanged){
            currentNode.change = ChangeType.Folder;
          }
        });
      }
      if (!currentNode.children) {
        currentNode.children = [{ id: ++this.id, name: file.name, type: type, change: change }];
      } else {
        currentNode.children.push({ id: ++this.id, name: file.name, type: type, change: change });
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
    sortedFiles: function (): Array<GeneratedFile> {
      return [...this.files].sort(function (a: GeneratedFile, b: GeneratedFile) {
        if (a.path === '') {
          return b.path.localeCompare(a.path) || a.name.localeCompare(b.name);
        }
        if (b.path === '') {
          return b.path.localeCompare(a.path) || a.name.localeCompare(b.name);
        }
        return a.path.localeCompare(b.path) || a.name.localeCompare(b.name);
      });
    },
  },
});
</script>

<style>
  .activeFile .v-treeview-node__content{
    font-weight: 900;
  }
  button.v-icon.notranslate {
    display: none;
  }
  .theme--light .v-treeview {
    background-color: #263238;
  }
  .theme--dark .v-treeview {
    background-color: #222;
  }
</style>