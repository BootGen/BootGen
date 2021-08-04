<template>
  <v-treeview
    :items="tree.children"
    :active.sync="activeNodes"
    :open.sync="tree.open"
    @update:active="selectFile"
    color="primary"
    return-object
    activatable
    hoverable
    dense
    open-on-click
  >
    <template v-slot:prepend="{ item, open }">
      <v-icon :color="$vuetify.theme.dark ? '#eee' : '#3C4C72'" v-if="item.type == 'folder'">
        {{ open ? 'mdi-chevron-down ' : 'mdi-chevron-right' }}
      </v-icon>
      <v-icon :color="darkColors[item.type]" v-else-if="$vuetify.theme.dark">
        {{ getIcon(item) }}
      </v-icon>
      <v-icon :color="colors[item.type]" v-else>
        {{ getIcon(item) }}
      </v-icon>
    </template>
    <template v-slot:append="{ item }">
      <p class="primary--text pa-0 ma-0 mr-1" v-if="item.change == ChangeType.Folder"><v-icon color="textOrange">mdi-circle-medium</v-icon></p>
      <p class="textOrange--text pa-0 ma-0 mr-2" v-else-if="item.change == ChangeType.Edited">M</p>
      <p class="textGreen--text pa-0 ma-0 mr-2" v-else-if="item.change == ChangeType.Created">A</p>
    </template>
    <template v-slot:label="{ item }">
      <p class="textOrange--text pa-0 ma-0" v-if="item.change == ChangeType.Folder">{{item.name}}</p>
      <p class="textOrange--text pa-0 ma-0" v-else-if="item.change == ChangeType.Edited">{{item.name}}</p>
      <p class="textGreen--text pa-0 ma-0" v-else-if="item.change == ChangeType.Created">{{item.name}}</p>
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
    openPath: String,
    showDifferences: Boolean,
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
      colors: {
        cs: '#267F99',
        vue: '#008000',
        ts: '#267F99',
        yml: '#AF00DB',
        js: '#dbc021',
        html: '#D16969',
        md: '#0000FF',
        json: '#cf8e16',
        gitignore: '#FF0000',
        browserslistrc: '#dbc021',
        sh: '#FF0000',
        csproj: '#AF00DB',
      },
      darkColors: {
        cs: '#569CD6',
        vue: '#6A9955',
        ts: '#569CD6',
        yml: '#AF00DB',
        js: '#DCDC8B',
        html: '#CE9178',
        md: '#569CD6',
        json: '#DCDC8B',
        gitignore: '#D16969',
        browserslistrc: '#DCDC8B',
        sh: '#D16969',
        csproj: '#AF00DB',
      },
      icons: {
        cs: 'mdi-language-csharp',
        vue: 'mdi-vuejs',
        ts: 'mdi-language-typescript',
        js: 'mdi-language-javascript',
        yml: 'mdi-exclamation-thick ',
        md: 'mdi-language-markdown',
        json: 'mdi-code-braces',
        html: 'mdi-code-tags',
        gitignore: 'mdi-git',
        csproj: 'mdi-rss',
        editorconfig: 'mdi-cog'
      },
      ChangeType: ChangeType,
    };
  },
  watch: {
    files(){
      this.init();
      this.setDefaultNodes();
    },
    openPath(){
      this.setOpenPath();
    }
  },
  created: function() {
    this.init();
    this.setDefaultNodes();
  },
  methods: {
    getIcon: function(item: Node): string {
      const icon = (this.icons as any)[item.type];
      if (icon)
        return icon;
      return 'mdi-text'
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
    init: function () {
      this.tree.children = [];
      this.tree.open = [];
      if(this.showDifferences){
        const changedFiles = this.getFileDifferences();
        if(this.previousFiles.length > 0){
          changedFiles.forEach((node) => {
            this.addToTree(node.file, node.changes);
          });
        }else{
          this.files.forEach((file) => {
            this.addToTree(file, ChangeType.Unchanged);
          });
        }
      }else{
        this.files.forEach((file) => {
          this.addToTree(file, ChangeType.Unchanged);
        });
      } 
      this.tree.children = this.sortNodes(this.tree.children);
      this.setOpenPath();
    },
    
    getFileDifferences: function(): { file: GeneratedFile; changes: ChangeType }[] {
        const changedFiles: { file: GeneratedFile; changes: ChangeType }[] = [];
        if(this.previousFiles.length>0) {
          this.files.forEach((file: GeneratedFile) => {
            let exists=false;
            for(let j=0;j<this.previousFiles.length;j++) {
              if(file.name===this.previousFiles[j].name&&file.path===this.previousFiles[j].path) {
                if(file.content!==this.previousFiles[j].content) {
                  changedFiles.push({ file: file, changes: ChangeType.Edited });
                } else {
                  changedFiles.push({ file: file, changes: ChangeType.Unchanged });
                }
                exists=true;
                break;
              }
            }
            if(!exists) {
              changedFiles.push({ file: file, changes: ChangeType.Created });
            }
          });
        }
        return changedFiles;
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
    addChildNode: function(node: Node, child: Node) {
      if (node.children) {
        node.children.push(child);
      } else {
        node.children = [ child ];
      }
    },
    addToTree: function (file: GeneratedFile, change: ChangeType) {
      const path = file.path.split('/');
      let currentNode: Node = this.tree;
      if (path[0] !== '') {
        path.forEach((part) => {
          const childNode = this.getChild(currentNode, part);
          if (!childNode) {
            const node: Node = { id: ++this.id, name: part, type: 'folder', change: change };
            this.addChildNode(currentNode, node);
            currentNode = node;
          } else {
            currentNode = childNode;
          }
          if(change !== ChangeType.Unchanged){
            currentNode.change = ChangeType.Folder;
          }
        });
      }
      const nameParts = file.name.split('.');
      const extension = nameParts[nameParts.length-1];
      const newNode: Node = { id: ++this.id, name: file.name, type: extension, change: change };
      this.addChildNode(currentNode, newNode);
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
    sortNodes: function (nodes: Node[]): Node[] {
      nodes.sort((a: Node, b: Node): number => {
        if (a.type === 'folder' && b.type !== 'folder')
          return -1;
        if (a.type !== 'folder' && b.type === 'folder')
          return 1;
        return a.name.localeCompare(b.name);
      });

      nodes.forEach((n: Node) => {
        if (n.children)
          n.children = this.sortNodes(n.children);
      })

      return nodes;
    },
  },
});
</script>

<style>
.v-treeview {
    overflow: auto;
    height: calc(100% - 40px);
}
button.v-icon.notranslate {
  display: none;
}

</style>