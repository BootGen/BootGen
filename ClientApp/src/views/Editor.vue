<template>
  <v-container fluid class="editor">
    <v-row class="d-flex">
      <v-col cols="12" class="head pa-0">
        <head-bar :viewModel="viewModel" :commandStore="commandStore" :activeProjectName="viewModel.activeProject.name" :backends="backends" :frontends="frontends" @new-project="createNewProject" @change-project-name="changeProjectName"></head-bar>
      </v-col>
    </v-row>
    <v-row class="d-flex align-center">
      <v-col xl="5" lg="5" md="12" class="pr-0 pl-0">
        <base-material-generator-card>
          <template v-slot:heading>
            <div class="d-flex align-center justify-space-between pa-2">
              <div class="d-flex select">
                <span class="display-1 font-weight-light pa-2">JSON</span>
                <v-icon class="mr-1">mdi-arrow-right</v-icon>
                <div class="d-flex">
                  <v-select
                    v-model="viewModel.activeProject.backend"
                    :items="backends"
                    @change="changeFramework('backend')"
                    dense
                    hide-details
                    :disabled="viewModel.generateLoading"
                  ></v-select>
                  <v-select
                    v-model="viewModel.activeProject.frontend"
                    :items="frontends"
                    @change="changeFramework('frontend')"
                    dense
                    hide-details
                    :disabled="viewModel.generateLoading"
                  ></v-select>
                </div>
              </div>
              <div class="d-flex">
                <tool-bar class="freamworkSettings" :buttonTypes="[CommandType.OpenSettingsDialog]" :commandStore="commandStore"></tool-bar>
                <project-settings v-if="viewModel.projectSettings" :modify="true" @save="saveProjectSettings" @cancel="cancelProjectSettings" title="Project Settings" :activeProject="viewModel.activeProject" :backends="backends" :frontends="frontends"></project-settings>

                <tool-bar v-if="$store.state.auth.jwt" :buttonTypes="[CommandType.Undo, CommandType.Save, CommandType.PrettyPrint, CommandType.Generate]" :commandStore="commandStore"></tool-bar>
                <tool-bar v-else :buttonTypes="[CommandType.Undo, CommandType.PrettyPrint, CommandType.Generate]" :commandStore="commandStore"></tool-bar>
              </div>
            </div>
          </template>
          <code-mirror cmId="cm0" v-model="viewModel.activeProject.json" mode="json" :readOnly="viewModel.generateLoading" :linesToColor="viewModel.jsonErrors" @change-json="removeErrors"></code-mirror>
        </base-material-generator-card>
      </v-col>
      <v-col xl="5" lg="5" md="8" sm="8" class="pr-0 pl-0">
        <base-material-generator-card>
          <template v-slot:heading>
            <div class="d-flex display-1 font-weight-light align-center justify-space-between pa-2">
              <div class="text-break" v-if="viewModel.activeFile.path && viewModel.activeFile.path !== ''">
                <span class="pathElement" v-for="(part, i) in viewModel.activeFile.path.split('/')" :key="i" @click="openFolder(i+1)">
                  {{ part }} /
                </span>
                <span class="pathElement" @click="openFolder(viewModel.activeFile.path.split('/').length)">
                  {{ viewModel.activeFile.name }}
                </span>
              </div>
              <div v-else>
                <span class="pathElement" @click="openFolder(0)">
                  {{ viewModel.activeFile.name }}
                </span>
              </div>
              <div class="d-flex">
                <tool-bar :buttonTypes="[CommandType.Compare, CommandType.Download]" :commandStore="commandStore"></tool-bar>
              </div>
            </div>
          </template>
          <code-mirror cmId="cm1" :value="viewModel.activeFile.content" :mode="getMode()" :readOnly="true" :linesToColor="viewModel.highlightedDifferences"></code-mirror>
        </base-material-generator-card>
      </v-col>
      <v-col xl="2" lg="2" md="4" sm="4" class="pr-0 pl-0">
        <base-material-generator-card>
          <template v-slot:heading>
            <div class="d-flex align-center justify-space-between pa-2">
              <div class="display-1 font-weight-light pa-2">Generated Files</div>
            </div>
          </template>
          <file-browser :files="viewModel.generatedFiles" :previousFiles="viewModel.previousFiles" :openPath="openPath" :isCompare="viewModel.showChanges" @select-file="selectFile"></file-browser>
        </base-material-generator-card>
      </v-col>
    </v-row>
    <snackbar v-if="viewModel.snackbar.visible" :snackbar="viewModel.snackbar"></snackbar>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import CodeMirror from '../components/CodeMirror.vue';
import FileBrowser from '../components/FileBrowser.vue';
import {validateJson, prettyPrint} from '../utils/PrettyPrint';
import HeadBar from '../components/HeadBar.vue';
import Snackbar from '../components/Snackbar.vue';
import ProjectSettings from '../components/ProjectSettings.vue';
import ToolBar from '../components/ToolBar.vue';
import {GeneratedFile} from '../models/GeneratedFile';
import {CRC32} from 'crc_32_ts';
import axios from 'axios';
import {ViewModel}from '../commands/ViewModel';
import { CommandStore, CommandType } from '../commands/CommandStore';

export default Vue.extend({
  components: {
    HeadBar,
    ProjectSettings,
    Snackbar,
    CodeMirror,
    FileBrowser,
    ToolBar
  },
  data: function () {
    return {
      viewModel: new ViewModel(),
      commandStore: null as (CommandStore | null),
      CommandType: CommandType,
      newProject: {id: -1, ownerId: -1, name: 'My Project', json: '', backend: 'ASP.NET', frontend: 'Vue 2 + JS'},
      drawer: false,
      openPath: '',
      backends: ['ASP.NET'],
      frontends: ['Vue 2 + JS', 'Vue 2 + TS'],
    };
  },
  created: async function(){
    this.commandStore = new CommandStore(this.viewModel, this.$gtag);
    this.newProject.json = JSON.stringify((await axios.get(`${this.$root.$data.baseUrl}/new_project_input.json`, {responseType: 'json'})).data);
    if(this.$store.state.projects.lastProject.json){
      this.viewModel.activeProject = this.$store.state.projects.lastProject;
      this.viewModel.generatedFiles = [...this.$store.state.projects.lastGeneratedFiles];
      this.viewModel.backend = this.$store.state.projects.lastProject.backend;
      this.viewModel.frontend = this.$store.state.projects.lastProject.frontend;
      this.callPrettyPrint();
      this.viewModel.undoStack.push(this.viewModel.activeProject.json);
      this.viewModel.setActiveFile();
      this.viewModel.crc32Saved = this.viewModel.crc32ForSaving;
      this.viewModel.crc32ProjectName = CRC32.str(this.viewModel.activeProject.name);
    }else{
      this.viewModel.activeProject.json = (await axios.get(`${this.$root.$data.baseUrl}/example_input.json`, {responseType: 'text'})).data;
      this.commandStore.do(CommandType.Generate);
    }
  },
  methods: {
    saveProjectSettings: function(backend: string, frontend: string, name: string){
      this.$gtag?.event('save-project-settings');
      this.viewModel.activeProject.backend = backend;
      this.viewModel.activeProject.frontend = frontend;
      this.viewModel.activeProject.name = name;
      this.viewModel.projectSettings = false;
    },
    cancelProjectSettings: function(){
      this.$gtag?.event('cancel-project-settings');
      this.viewModel.projectSettings = false;
    },
    getMode: function(): string {
      if(this.viewModel.activeFile.name){
        return this.viewModel.activeFile.name.split('.')[1];
      }
      return '';
    },
    removeErrors: function(){
      this.viewModel.jsonErrors = [];
      this.viewModel.hideSnackbar();
    },
    callPrettyPrint: function(){
      const result = validateJson(this.viewModel.activeProject.json);
      if (result.error) {
        this.viewModel.jsonErrors.push({line: result.line, color: 'rgba(255, 0, 0, 0.3)'});
        this.viewModel.setSnackbar('orange darken-2', result.message, -1);
      }
      this.viewModel.activeProject.json = prettyPrint(this.viewModel.activeProject.json);
    },
    createNewProject: async function(name: string, backend: string, frontend: string){
      this.$gtag?.event('create-new-project');
      this.newProject.name = name;
      this.newProject.backend = backend;
      this.newProject.frontend = frontend;
      this.viewModel.activeProject = {...this.newProject};
      this.callPrettyPrint();
      if(this.commandStore){
        this.commandStore.do(CommandType.Save);
      }
      this.viewModel.undoStack.clear();
      if(this.commandStore){
        this.commandStore.do(CommandType.Generate);
      }
      this.viewModel.highlightedDifferences = [];
    },
    changeFramework: function(page: string){
      this.$gtag?.event(`change-${page}`);
    },
    changeProjectName: function(name: string){
      this.$gtag?.event('change-project-name');
      this.viewModel.activeProject.name = name.trim();
    },
    openFolder: function(idx: number){
      this.$gtag?.event('click-on-path-element');
      this.openPath = '';
      for(let i = 0; i < idx; i++){
        if(this.viewModel.activeFile.path.split('/')[i]){
          this.openPath += `${this.viewModel.activeFile.path.split('/')[i]}/`;
        }
      }
      this.openPath = this.openPath.slice(0, -1);
      this.drawer = true;
    },
    selectFile: function(data: GeneratedFile){
      this.viewModel.activeFile = data;
      this.viewModel.setHighlightedDifferences();
    },
  },
});
</script>

<style lang="css">
  .editor{
    margin: 0!important;
    padding: 0!important;
    position: relative;
    top: -30px;
  }
  .head{
    z-index: 99;
  }
  .container.freamworkSettings {
    display: none!important;
  }
  .pathElement{
    cursor: pointer!important;
  }
  .pathElement:hover{
    opacity: 0.6;
  }
  .select{
    max-width: 300px;
  }
  @media screen and (max-width: 600px) {
    .d-flex.select{
      display: none!important;
    }
    .container.freamworkSettings{
      display: flex!important;
    }
  }
</style>
