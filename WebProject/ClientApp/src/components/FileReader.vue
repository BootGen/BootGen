<template>
  <base-material-generator-card>
    <template v-slot:heading>
      <div class="d-flex display-1 font-weight-light align-center justify-space-between pa-2">
        <div class="text-break" v-if="activeFile && activeFile.name">
          <span v-for="(part, i) in activeFile.path.split('/')" :key="i" @click="openFolder(i+1)">
            {{ part }}/
          </span>
          <span @click="openFolder(activeFile.path.split('/').length)">
            {{ activeFile.name }}
          </span>
        </div>
        <div v-else @click="drawer = true">Select a file</div>
        <div class="d-flex">
          <v-tooltip bottom>
            <template v-slot:activator="{ on, attrs }">
              <v-btn color="white" class="mr-2" elevation="1" fab small v-bind="attrs" v-on="on">
                <v-icon color="primary">mdi-download</v-icon>
              </v-btn>
            </template>
            <span>Download</span>
          </v-tooltip>
          <v-tooltip bottom>
            <template v-slot:activator="{ on, attrs }">
              <v-btn color="white" @click="drawer = !drawer" elevation="1" fab small v-bind="attrs" v-on="on">
                <v-icon color="primary" v-if="drawer">mdi-folder-open</v-icon>
                <v-icon color="primary" v-else>mdi-folder</v-icon>
              </v-btn>
            </template>
            <span>Files</span>
          </v-tooltip>                
          <v-tooltip bottom>
            <template v-slot:activator="{ on, attrs }">
              <v-btn class="ml-2 mr-2" color="white" elevation="1" fab small @click="openSettings = true" v-bind="attrs" v-on="on">
                <v-icon color="primary">mdi-cog</v-icon>
              </v-btn>
              </template>
            <span>Settings</span>
          </v-tooltip>
          <settings-dialog v-if="openSettings" @close-settings="openSettings = false"></settings-dialog>
          <v-col class="d-flex fileSelector" v-if="drawer"><!--v-click-outside="closeTreeView"-->
            <tree-view :files="files" :openPath="openPath" @select-file="selectFile"></tree-view>
          </v-col>
        </div>
      </div>
    </template>
    <codemirror v-model="activeFile.content" :options="cmOptions" />
  </base-material-generator-card>
</template>

<script lang="ts">
import Vue from "vue";
import SettingsDialog from "../components/SettingsDialog.vue";
import TreeView from "../components/TreeView.vue";

import { codemirror } from 'vue-codemirror'

import 'codemirror/lib/codemirror.css'
import "codemirror/mode/clike/clike.js";
import "codemirror/mode/yaml/yaml.js";
import "codemirror/mode/javascript/javascript.js";
import 'codemirror/theme/material.css'
import { GeneratedFile } from "../models/GeneratedFile";

export default Vue.extend({
  props: {
    files: {
			type: Array as () => GeneratedFile[]
		},
  },
  components: {
    SettingsDialog,
    TreeView,
    codemirror
  },
  watch: {
    files: {
      handler(files: GeneratedFile[]) {
        if(this.activeFile){
          const name: string = this.activeFile.name;
          const result = files.find(file => file.name === name);
          if(result) {
            this.activeFile = result;  
          } else {
            this.activeFile = {
              name: "",
              path: "",
              content: ""
            };
          }
        }
      }
    }
  },
  data: function () {
    return {
      openSettings: false,
      activeFile: {
        name: "",
        path: "",
        content: ""
      },
      drawer: false,
      cmOptions: {
        theme: 'material',
        tabSize: 2,
        mode: 'text/x-csharp',
        lineNumbers: true,
        line: true,
        readOnly: true,
      },
      openPath: "",
    };
  },
  methods: {
    openFolder: function(idx: number){
      this.openPath = "";
      for(let i = 0; i < idx; i++){
        if(this.activeFile.path.split('/')[i]){
          this.openPath += this.activeFile.path.split('/')[i]+"/";
        }
      }
      this.openPath = this.openPath.slice(0, -1);
      this.drawer = true;
    },
    selectFile: function(data: GeneratedFile){
      this.activeFile = data;
      const type = this.activeFile.name.split('.')[1];
      if(type === "cs"){
        this.cmOptions.mode = 'text/x-csharp';
      }else if(type === "ts"){
        this.cmOptions.mode = 'text/typescript';
      }else{
        this.cmOptions.mode = 'text/x-yaml';
      }
    },
    closeTreeView: function(){
      this.drawer = false;
    }
  },
});
</script>

<style lang="css">
  .fileSelector {
    max-height: calc(100vh - 230px);
    overflow: auto;
    position: absolute;
    background:#412fb3;
    top: 60px;
    right: 0px;
    width: fit-content;
    border-radius: 3px;
    word-wrap: break-word!important;
    z-index: 1;
  }
</style>