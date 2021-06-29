<template>
  <v-container fluid class="editor">
    <v-row class="d-flex">
      <v-col cols="12" class="head pa-0">
        <head-bar v-if="$store.state.auth.jwt" :activeProjectName="viewModel.activeProject.name" :backends="backends" :frontends="frontends" :disabled="viewModel.generateLoading" @new-project="createNewProject" @change-project-name="changeProjectName"></head-bar>
        <head-bar v-else :activeProjectName="viewModel.activeProject.name" :disabled="viewModel.generateLoading" @new-project="createNewProject" @change-project-name="changeProjectName"></head-bar>
      </v-col>
    </v-row>
    <v-row class="d-flex align-center">
      <v-col xl="5" lg="5" md="12" class="pr-0 pl-0">
        <base-material-generator-card>
          <template v-slot:heading>
            <div class="d-flex align-center justify-space-between pa-2">
              <div class="d-flex">
                <span class="display-1 font-weight-light pa-2">JSON</span>
                <v-icon class="mr-1">mdi-arrow-right</v-icon>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-1 freamworkSettings" color="white" elevation="1" @click="viewModel.projectSettings = true" fab small v-bind="attrs" v-on="on" :disabled="viewModel.generateLoading">
                      <v-icon color="primary">mdi-cog</v-icon>
                    </v-btn>
                  </template>
                  <span>Project settings</span>
                </v-tooltip>
                <project-settings v-if="viewModel.projectSettings" @save="saveProjectSettings" @cancel="cancelProjectSettings" title="Project Settings" :activeProject="viewModel.activeProject" :backends="backends" :frontends="frontends"></project-settings>
                <div class="d-flex select">
                  <v-select
                    v-model="viewModel.activeProject.backend"
                    :items="backends"
                    @change="change('backend')"
                    dense
                    hide-details
                    :disabled="viewModel.generateLoading"
                  ></v-select>
                  <v-select
                    v-model="viewModel.activeProject.frontend"
                    :items="frontends"
                    @change="change('frontend')"
                    dense
                    hide-details
                    :disabled="viewModel.generateLoading"
                  ></v-select>
                </div>
              </div>
              <div>
                <tool-bar :buttons="[undoCommand, saveCommand, prettyPrintCommand, generateCommand]"></tool-bar>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-1" color="white" elevation="1" fab small @click="undo" v-bind="attrs" v-on="on" :disabled="(viewModel.undoStack.length() < 2 && viewModel.isJsonPristine) || viewModel.generateLoading">
                      <v-icon color="primary">mdi-undo</v-icon>
                    </v-btn>
                  </template>
                  <span>Rollback to the last generated state</span>
                </v-tooltip>
                <v-tooltip bottom v-if="$store.state.auth.jwt">
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-1" color="white" elevation="1" fab small @click="save" v-bind="attrs" v-on="on" :disabled="viewModel.saveDisabled || viewModel.generateLoading">
                      <v-icon color="primary">mdi-floppy</v-icon>
                    </v-btn>
                    </template>
                  <span>Save</span>
                </v-tooltip>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-1" color="white" elevation="1" fab small :disabled="viewModel.activeProject.json === '' || viewModel.generateLoading" @click="onPrettyPrintClicked" v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-format-align-left</v-icon>
                    </v-btn>
                  </template>
                  <span>Formatting</span>
                </v-tooltip>

                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-1" color="white" elevation="1" fab small :disabled="viewModel.isPristine || viewModel.generateLoading || viewModel.activeProject.name === ''" @click="onGenerateClicked" v-bind="attrs" v-on="on">
                      <v-icon color="primary" v-if="!viewModel.generateLoading">mdi-arrow-right-bold</v-icon>
                      <div v-if="viewModel.generateLoading">
                        <v-progress-circular
                          indeterminate
                          :size="25"
                          color="primary"
                        ></v-progress-circular>
                      </div>
                    </v-btn>
                    </template>
                  <span>Generate</span>
                </v-tooltip>
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
                <tool-bar :buttons="[compareCommand]"></tool-bar>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn color="white" v-if="viewModel.isCompare" class="mr-1" elevation="1" @click="setCompare()" fab small v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-file-compare</v-icon>
                    </v-btn>
                    <v-btn color="rgba(255, 255, 255, 0.2)" v-else class="mr-1" elevation="0" @click="setCompare()" fab small v-bind="attrs" v-on="on">
                      <v-icon color="rgba(255, 255, 255, 0.7)">mdi-file-compare</v-icon>
                    </v-btn>
                  </template>
                  <span v-if="viewModel.isCompare">Show Changes: On</span>
                  <span v-else>Show Changes: Off</span>
                </v-tooltip>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn color="white" class="mr-1" elevation="1" :disabled="!viewModel.isPristine || viewModel.downLoading || viewModel.activeProject.name === ''" @click="download" fab small v-bind="attrs" v-on="on">
                      <v-icon color="primary" v-if="!viewModel.downLoading">mdi-download</v-icon>
                      <div v-if="viewModel.downLoading">
                        <v-progress-circular
                          indeterminate
                          :size="25"
                          color="primary"
                        ></v-progress-circular>
                      </div>
                    </v-btn>
                  </template>
                  <span>Download</span>
                </v-tooltip>
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
    <snackbar v-if="snackbar.visible" :snackbar="snackbar"></snackbar>
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
import {Project} from '../models/Project';
import {GeneratedFile} from '../models/GeneratedFile';
import {CRC32} from 'crc_32_ts';
import axios from 'axios';
import api from '../api'
import {GenerateResponse} from '../models/GenerateResponse';
import {Compare}from '../utils/TextCompare';
import {ViewModel}from '../utils/ViewModel';
import { Command, UndoCommand, SaveCommand, PrettyPrintCommand, GenerateCommand, CompareCommand } from '../utils/Command';

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
      undoCommand: null as (Command | null),
      saveCommand: null as (Command | null),
      prettyPrintCommand: null as (Command | null),
      generateCommand: null as (Command | null),
      compareCommand: null as (Command | null),
      newProject: {id: -1, ownerId: -1, name: 'My Project', json: '', backend: 'ASP.NET', frontend: 'Vue 2 + JS'},
      snackbar: {
        dismissible: true,
        visible: false,
        type: '',
        icon: 'mdi-alert-circle',
        text: '',
        timeout: 5000,
      },
      drawer: false,
      openPath: '',
      backends: ['ASP.NET'],
      frontends: ['Vue 2 + JS', 'Vue 2 + TS'],
    };
  },
  created: async function(){
    this.undoCommand = new UndoCommand(this.viewModel);
    this.saveCommand = new SaveCommand(this.viewModel);
    this.prettyPrintCommand = new PrettyPrintCommand(this.viewModel);
    this.generateCommand = new GenerateCommand(this.viewModel);
    this.compareCommand = new CompareCommand(this.viewModel);
    this.viewModel.setSnackbar = this.setSnackbar;
    this.viewModel.setHighlightedDifferences = this.setHighlightedDifferences;
    this.newProject.json = JSON.stringify((await axios.get(`${this.$root.$data.baseUrl}/new_project_input.json`, {responseType: 'json'})).data);
    if(this.$store.state.projects.lastProject.json){
      this.viewModel.activeProject = this.$store.state.projects.lastProject;
      this.viewModel.generatedFiles = [...this.$store.state.projects.lastGeneratedFiles];
      this.viewModel.backend = this.$store.state.projects.lastProject.backend;
      this.viewModel.frontend = this.$store.state.projects.lastProject.frontend;
      this.callPrettyPrint();
      this.viewModel.undoStack.push(this.viewModel.activeProject.json);
      this.setActiveFile();
      this.viewModel.crc32Saved = this.viewModel.crc32ForSaving;
      this.viewModel.crc32ProjectName = CRC32.str(this.viewModel.activeProject.name);
    }else{
      this.viewModel.activeProject.json = (await axios.get(`${this.$root.$data.baseUrl}/example_input.json`, {responseType: 'text'})).data;
      await this.validateAndGenerate();
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
    delay: function(ms: number): Promise<void> {
      return new Promise(resolve => setTimeout(resolve, ms));
    },
    getMode: function(): string {
      if(this.viewModel.activeFile.name){
        return this.viewModel.activeFile.name.split('.')[1];
      }
      return '';
    },
    removeErrors: function(){
      this.viewModel.jsonErrors = [];
      this.hideSnackbar();
    },
    generate: async function(){
      if(!this.viewModel.generateLoading){
        this.viewModel.generateLoading = true;
        const generateResult: GenerateResponse = await api.generate({
          data: this.viewModel.activeProject.json,
          nameSpace: this.toCamelCase(this.viewModel.activeProject.name),
          backend: this.viewModel.activeProject.backend,
          frontend: this.viewModel.activeProject.frontend
        });
        if(generateResult.errorMessage){
          this.setSnackbar('orange darken-2', generateResult.errorMessage, -1);
          if(generateResult.errorLine !== null){
            this.viewModel.jsonErrors.push({line: generateResult.errorLine-1, color: 'rgba(255, 0, 0, 0.3)'});
          }
          this.viewModel.generateLoading = false;
          return;
        }
        this.viewModel.previousFiles = [...this.viewModel.generatedFiles];
        this.viewModel.generatedFiles = generateResult.generatedFiles;
        this.$store.commit('projects/setLastProject', this.viewModel.activeProject);
        this.$store.commit('projects/setLastGeneratedFiles', this.viewModel.generatedFiles);
        this.viewModel.crc32ProjectName = CRC32.str(this.viewModel.activeProject.name);
        if(this.viewModel.activeProject.backend === this.viewModel.backend && this.viewModel.activeProject.frontend === this.viewModel.frontend){
          this.viewModel.showChanges = true;
          this.viewModel.highlightedDifferences = [];
        }else{
          this.viewModel.showChanges = false;
        }
        this.viewModel.backend = this.viewModel.activeProject.backend;
        this.viewModel.frontend = this.viewModel.activeProject.frontend;
        if(this.viewModel.undoStack.top()?.content !== this.viewModel.activeProject.json){
          this.viewModel.undoStack.push(this.viewModel.activeProject.json);
        }
        this.setActiveFile();
        this.viewModel.generateLoading = false;
      }
    },
    setCompare: function(){
      if(this.viewModel.isCompare){
        this.viewModel.isCompare = false;
        this.viewModel.showChanges = false;
        this.viewModel.highlightedDifferences = [];
      }else{
        this.viewModel.isCompare = true;
        this.viewModel.showChanges = true;
        this.setHighlightedDifferences();
      }
    },
    setHighlightedDifferences: function(){
      if(this.viewModel.previousFiles.length > 0 && this.viewModel.isCompare){
        this.viewModel.highlightedDifferences = [];
        if(this.viewModel.showChanges){
          for(let i = 0; i < this.viewModel.previousFiles.length; i++){
            if(this.viewModel.previousFiles[i].name === this.viewModel.activeFile.name && this.viewModel.previousFiles[i].path === this.viewModel.activeFile.path){
              const compare = new Compare(this.viewModel.activeFile.content.split('\n'), this.viewModel.previousFiles[i].content.split('\n'));
              const changes = compare.getChanges();
              changes.forEach(v =>{
                this.viewModel.highlightedDifferences.push({ line : v, color:'rgba(0, 111, 197, 0.3)' })
              })
              break;
            }
          }
        }
      }
    },
    getJsonLength: function(json: string): number{
      json = json.replace(/ {2}/g, '');
      json = json.replace(/": /g, '":');
      json = json.replace(/[\n\t\r]/g, '');
      return json.length;
    },
    setSelectedFile: function (fileName: string, filePath: string) {
      for (let i = 0; i < this.viewModel.generatedFiles.length; i++) {
        if (this.viewModel.generatedFiles[i].name === fileName && this.viewModel.generatedFiles[i].path === filePath) {
          this.viewModel.activeFile = this.viewModel.generatedFiles[i];
          break;
        }
      }
    },
    setActiveFile: function(){
      if(!this.viewModel.activeFile.name){
        this.setSelectedFile('restapi.yml', '');
      }else{
        this.setSelectedFile(this.viewModel.activeFile.name, this.viewModel.activeFile.path);
      }
      this.setHighlightedDifferences();
    },
    onPrettyPrintClicked: function(){
      this.$gtag?.event('pretty-print');
      this.callPrettyPrint();
    },
    callPrettyPrint: function(){
      const result = validateJson(this.viewModel.activeProject.json);
      if (result.error) {
        this.viewModel.jsonErrors.push({line: result.line, color: 'rgba(255, 0, 0, 0.3)'});
        this.setSnackbar('orange darken-2', result.message, -1);
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
      this.save();
      this.viewModel.undoStack.clear();
      await this.generate();
      this.viewModel.highlightedDifferences = [];
    },
    onGenerateClicked: function () {
      this.$gtag?.event('generate');
      this.validateAndGenerate();
    },
    validateAndGenerate: async function() {
      this.viewModel.jsonErrors = [];
      const result = validateJson(this.viewModel.activeProject.json);
      if(!result.error) {
        this.callPrettyPrint();
        const jsonLength = this.getJsonLength(this.viewModel.activeProject.json);
        if(jsonLength > 2000) {
          this.setSnackbar('orange darken-2', `Exceeded character limit: ${jsonLength} / 2000`, -1);
          return;
        }
        await this.generate();
      } else {
        this.viewModel.jsonErrors.push({line: result.line, color: 'rgba(255, 0, 0, 0.3)'});
        this.setSnackbar('orange darken-2', result.message, -1);
      }
    },
    setSnackbar: function(type: string, text: string, timeout: number){
      this.snackbar.dismissible = true;
      this.snackbar.timeout = timeout;
      this.snackbar.type = type;
      this.snackbar.text = text;
      this.snackbar.visible = true;
    },
    hideSnackbar: function(){
      if(this.snackbar.type === 'error'){
        this.snackbar.visible = false;
      }
    },
    getProjectByName: function(projectName: string): Project | null{
      for(const i in this.$store.state.projects.items){
        if(projectName === this.$store.state.projects.items[i].name){
          return this.$store.state.projects.items[i];
        }
      }
      return null;
    },
    save: async function (){
      if (!this.$store.state.auth.jwt)
        return;
      this.$gtag?.event('save-project');
      if(this.viewModel.activeProject.name){
        const project = this.getProjectByName(this.viewModel.activeProject.name);
        if (!project && this.viewModel.activeProject.id === -1) {
          this.setSnackbar('success', 'The new project was successfully created!', 5000);
          this.viewModel.crc32Saved = this.viewModel.crc32ForSaving;
          this.viewModel.activeProject.id = 0;
          this.viewModel.activeProject.ownerId = this.$store.state.auth.user.id;
          this.viewModel.activeProject = await this.$store.dispatch('projects/addProject', this.viewModel.activeProject);
        } else if(project && project.id !== this.viewModel.activeProject.id) {
          this.setSnackbar('error', 'This name is already in use, please enter another name!', 5000);
        } else {
          this.viewModel.crc32Saved = this.viewModel.crc32ForSaving;
          this.setSnackbar('success', 'Project updated successfully!', 5000);
          await this.$store.dispatch('projects/updateProject', this.viewModel.activeProject);
          this.setSnackbar('success', 'Project updated successfully!', 5000);
        }
      }else{
        this.setSnackbar('error', 'This name is incorrect!', 5000);
      }
    },
    undo: async function () {
      this.$gtag?.event('undo');
      if (this.viewModel.isPristine) {
        this.viewModel.undoStack.pop();
      }
      const top = this.viewModel.undoStack.top();
      if(top) {
        this.viewModel.activeProject.json = top.content;
      }
      this.setSnackbar('info', 'Everything restored to its previous generated state', 5000);
    },
    change: function(page: string){
      this.$gtag?.event(`change-${page}`);
    },
    changeProjectName: function(name: string){
      this.$gtag?.event('change-project-name');
      this.viewModel.activeProject.name = name.trim();
    },
    download: async function() {
      if(!this.viewModel.downLoading){
        this.viewModel.downLoading = true;
        this.$gtag?.event('download');
        await Promise.all([
          this.delay(3000),
          api.download({
            data: this.viewModel.activeProject.json,
            nameSpace: this.toCamelCase(this.viewModel.activeProject.name),
            backend: this.viewModel.activeProject.backend,
            frontend: this.viewModel.activeProject.frontend
          })
        ]);
        this.viewModel.downLoading = false;
      }
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
      this.setHighlightedDifferences();
    },
    toCamelCase: function(str: string): string {
      const nameSpace = str.replace(/(?:^\w|[A-Z]|\b\w)/g, function(word, index) {
        return index === 0 ? word.toLowerCase() : word.toUpperCase();
      }).replace(/\s+/g, '');
      return `${nameSpace.charAt(0).toUpperCase()}${nameSpace.slice(1)}`;
    }
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
  .freamworkSettings{
    display: none;
  }
  .pathElement{
    cursor: pointer!important;
  }
  .pathElement:hover{
    opacity: 0.6;
  }
  .select{
    max-width: 220px;
  }
  @media screen and (max-width: 600px) {
    .d-flex.select{
      display: none!important;
    }
    .freamworkSettings{
      display: flex;
    }
  }
</style>
