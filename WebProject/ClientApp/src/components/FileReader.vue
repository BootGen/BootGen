<template>
  <base-material-generator-card>
    <template v-slot:heading>
      <div class="d-flex display-1 font-weight-light align-center justify-space-between pa-2">
        <span class="text-break" v-if="activeFile && activeFile.name">{{ activeFile.path }}/{{ activeFile.name }}</span>
        <span v-else>Select a file</span>
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
          <v-col class="d-flex fileSelector" v-if="drawer" v-click-outside="closeTreeView">
            <tree-view :files="files" @select-file="selectFile"></tree-view>
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
    };
  },
  methods: {
    selectFile: function(data: GeneratedFile){
      this.activeFile = data;
      if(this.activeFile.name.split('.')[1] === "cs"){
        this.cmOptions.mode = 'text/x-csharp';
      }else{
        this.cmOptions.mode = 'text/typescript';
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
    position: absolute;
    background:#412fb3;
    top: 55px;
    right: 0px;
    width: fit-content;
    border-radius: 3px;
    word-wrap: break-word!important;
    z-index: 1;
  }
</style>