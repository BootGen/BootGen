<template>
  <v-container fluid class="editor">
    <v-row class="d-flex align-center ma-0 pa-0">
      <v-col lg="5" md="6" sm="8" cols="12" class="pa-0 headBar" v-if="$store.state.auth.jwt">
        <head-bar :activeProject="activeProject" @new-project="createNewProject" @change-project-name="changeProjectName"></head-bar>
      </v-col>
      <v-col cols="12" class="pa-0 headBar" v-else>
        <head-bar :activeProject="activeProject" @new-project="createNewProject" @change-project-name="changeProjectName"></head-bar>
      </v-col>
    </v-row>
    <v-row class="d-flex align-center">
      <v-col cols="12" md="6" class="pr-0 pl-0">
        <base-material-generator-card>
          <template v-slot:heading>
            <div class="d-flex align-center justify-space-between pa-2">
              <div class="display-1 font-weight-light pa-2">
                JSON
              </div>
              <div class="icons">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small @click="undo" v-bind="attrs" v-on="on" :disabled="undoStack.length() < 2 && isJsonPristine">
                      <v-icon color="primary">mdi-undo</v-icon>
                    </v-btn>
                  </template>
                  <span>Rollback to the last generated state</span>
                </v-tooltip>
                <v-tooltip bottom v-if="$store.state.auth.jwt">
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small @click="save" v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-floppy</v-icon>
                    </v-btn>
                    </template>
                  <span>Save</span>
                </v-tooltip>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small :disabled="activeProject.json === ''" @click="callPrettyPrint" v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-format-align-left</v-icon>
                    </v-btn>
                  </template>
                  <span>Formatting</span>
                </v-tooltip>

                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small :disabled="isPristine || generateLoading || activeProject.name === ''" @click="validateAndGenerate()" v-bind="attrs" v-on="on">
                      <v-icon color="primary" v-if="!generateLoading">mdi-arrow-right-bold</v-icon>
                      <div v-if="generateLoading">
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
          <code-mirror cmId="cm0" v-model="activeProject.json" mode="json" :readOnly="false" :linesToColor="jsonErrors" @cursor-into-view="closeDrawer" @change-json="removeErrors"></code-mirror>
        </base-material-generator-card>
      </v-col>

      <v-col cols="12" md="6" class="pa-0">
        <base-material-generator-card>
          <template v-slot:heading>
            <div class="d-flex display-1 font-weight-light align-center justify-space-between pa-2">
              <div class="text-break" v-if="activeFile.path && activeFile.path !== ''">
                <span class="pathElement" v-for="(part, i) in activeFile.path.split('/')" :key="i" @click="openFolder(i+1)">
                  {{ part }} /
                </span>
                <span class="pathElement" @click="openFolder(activeFile.path.split('/').length)">
                  {{ activeFile.name }}
                </span>
              </div>
              <div v-else>
                <span class="pathElement" @click="openFolder(0)">
                  {{ activeFile.name }}
                </span>
              </div>
              <div class="d-flex">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn color="white" v-if="isCompare" class="mr-2" elevation="1" @click="setCompare()" fab small v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-file-compare</v-icon>
                    </v-btn>
                    <v-btn color="blue-grey" v-else class="mr-2" elevation="1" @click="setCompare()" fab small v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-file-compare</v-icon>
                    </v-btn>
                  </template>
                  <span v-if="isCompare">Show Changes: Off</span>
                  <span v-else>Show Changes: On</span>
                </v-tooltip>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn color="white" class="mr-2" elevation="1" :disabled="!isPristine || downLoading || activeProject.name === ''" @click="download" fab small v-bind="attrs" v-on="on">
                      <v-icon color="primary" v-if="!downLoading">mdi-download</v-icon>
                      <div v-if="downLoading">
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
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn color="white" @click="setDrawer()" elevation="1" fab small v-bind="attrs" v-on="on">
                      <v-icon color="primary" v-if="drawer">mdi-folder-open</v-icon>
                      <v-icon color="primary" v-else>mdi-folder</v-icon>
                    </v-btn>
                  </template>
                  <span>Files</span>
                </v-tooltip>
                <v-col class="d-flex fileSelector" v-if="drawer">
                  <tree-view :files="generatedFiles" :openPath="openPath" @select-file="selectFile"></tree-view>
                </v-col>
              </div>
            </div>
          </template>
          <code-mirror cmId="cm1" :value="activeFile.content" :mode="getMode()" :readOnly="true" :linesToColor="highlightedDifferences" @cursor-into-view="closeDrawer"></code-mirror>
        </base-material-generator-card>
      </v-col>
    </v-row>
    <snackbar v-if="snackbar.visible" :snackbar="snackbar"></snackbar>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import CodeMirror from '../components/CodeMirror.vue';
import TreeView from '../components/TreeView.vue';
import {validateJson, prettyPrint} from '../utils/PrettyPrint';
import {UndoStack} from '../utils/UndoStack';
import HeadBar from '../components/HeadBar.vue';
import Snackbar from '../components/Snackbar.vue';
import {Project} from '../models/Project';
import {GeneratedFile} from '../models/GeneratedFile';
import {CRC32} from 'crc_32_ts';
import axios from 'axios';
import api from '../api'
import {GenerateResponse} from '../models/GenerateResponse';
import {Compare}from '../utils/TextCompare'

export default Vue.extend({
  components: {
    HeadBar,
    Snackbar,
    CodeMirror,
    TreeView,
  },
  data: function () {
    return {
      generatedFiles: Array<GeneratedFile>(),
      previousFiles: Array<GeneratedFile>(),
      undoStack: new UndoStack(),
      crc32ProjectName: CRC32.str('My Project'),
      activeProject: {id: -1, ownerId: -1, name: 'My Project', json: ''},
      newProject: {id: -1, ownerId: -1, name: 'My Project', json: ''},
      activeFile: {} as GeneratedFile,
      snackbar: {
        dismissible: true,
        visible: false,
        type: '',
        icon: 'mdi-alert-circle',
        text: '',
        timeout: 5000,
      },
      jsonErrors: Array<{line: number; color: string}>(),
      highlightedDifferences: Array<{line: number; color: string}>(),
      drawer: false,
      openPath: '',
      generateLoading: false,
      downLoading: false,
      isCompare: true,
    };
  },
  created: async function(){
    this.newProject.json = JSON.stringify((await axios.get(`${this.$root.$data.baseUrl}/new_project_input.json`, {responseType: 'json'})).data);
    if(this.$store.state.projects.lastProject.json){
      this.activeProject = {...this.$store.state.projects.lastProject};
      this.generatedFiles = [...this.$store.state.projects.lastGeneratedFiles];
      this.callPrettyPrint();
      this.setActiveFile();
    }else{
      this.activeProject.json = (await axios.get(`${this.$root.$data.baseUrl}/example_input.json`, {responseType: 'text'})).data;
    }
    await this.validateAndGenerate();
  },
  computed: {
    isPristine: function(){
      const top = this.undoStack.top();
      if(top){
        if((top.crc32 === CRC32.str(this.activeProject.json)) && (this.crc32ProjectName === CRC32.str(this.activeProject.name))){
          return true;
        }
      }
      return false;
    },
    isJsonPristine: function(){
      const top = this.undoStack.top();
      if(top){
        if(top.crc32 === CRC32.str(this.activeProject.json)){
          return true;
        }
      }
      return false;
    },
  },
  methods: {
    delay: function(ms: number) {
      return new Promise(resolve => setTimeout(resolve, ms));
    },
    getMode: function(){
      if(this.activeFile.name){
        return this.activeFile.name.split('.')[1];
      }
    },
    removeErrors: function(){
      this.jsonErrors = [];
      this.hideSnackbar();
    },
    generate: async function(){
      if(!this.generateLoading){
        this.generateLoading = true;
        const generateResult: GenerateResponse = await api.generate({data: this.activeProject.json, nameSpace: this.toCamelCase(this.activeProject.name)});
        if(generateResult.errorMessage){
          this.setSnackbar('orange darken-2', generateResult.errorMessage, -1);
          if(generateResult.errorLine !== null){
            this.jsonErrors.push({line: generateResult.errorLine-1, color: 'red'});
          }
          this.generateLoading = false;
          return;
        }
        this.previousFiles = [...this.generatedFiles];
        this.generatedFiles = generateResult.generatedFiles;
        this.$store.commit('projects/setLastProject', this.activeProject);
        this.$store.commit('projects/setLastGeneratedFiles', this.generatedFiles);
        this.crc32ProjectName = CRC32.str(this.activeProject.name);
        if(this.undoStack.length() > 0){
          if(!this.isJsonPristine){
            this.undoStack.push(this.activeProject.json);
          }
        }else{
          this.undoStack.push(this.activeProject.json);
        }
        this.setActiveFile();
        this.setHighlightedDifferences();
        this.generateLoading = false;
      }
    },
    setCompare: function(){
      if(this.isCompare){
        this.isCompare = false;
        this.highlightedDifferences = [];
      }else{
        this.isCompare = true;
        this.setHighlightedDifferences();
      }
    },
    setHighlightedDifferences: function(){
      if(this.previousFiles.length > 0 && this.isCompare){
        this.highlightedDifferences = [];
        for(let i = 0; i < this.previousFiles.length; i++){
          if(this.previousFiles[i].name === this.activeFile.name && this.previousFiles[i].path === this.activeFile.path){
            const compare = new Compare(this.previousFiles[i].content.split('\n'), this.activeFile.content.split('\n'));
            const changes = compare.getChanges();
            changes.forEach(v =>{
              this.highlightedDifferences.push({ line : v, color:'#412fb3' })
            })
            break;
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
    setActiveFile: function(){
      if(!this.activeFile.name){
        for(let i = 0; i < this.generatedFiles.length; i++){
          if(this.generatedFiles[i].name === 'restapi.yml' && this.generatedFiles[i].path === ''){
            this.activeFile = this.generatedFiles[i];
            break;
          }
        }
      }else{
        for(let i = 0; i < this.generatedFiles.length; i++){
          if(this.generatedFiles[i].name === this.activeFile.name && this.generatedFiles[i].path === this.activeFile.path){
            this.activeFile = this.generatedFiles[i];
            break;
          }
        }
      }
      this.setHighlightedDifferences();
    },
    callPrettyPrint: function(){
      this.$gtag.event('pretty-print');
      const result = validateJson(this.activeProject.json);
      if (result.error) {
        this.jsonErrors.push({line: result.line, color: 'red'});
        this.setSnackbar('orange darken-2', result.message, -1);
      }
      this.activeProject.json = prettyPrint(this.activeProject.json);
      this.hideSnackbar();
    },
    createNewProject: async function(name: string){
      this.$gtag.event('create-new-project');
      this.newProject.name = name;
      this.activeProject = {...this.newProject};
      this.callPrettyPrint();
      this.save();
      this.undoStack.clear();
      await this.generate();
      this.highlightedDifferences = [];
    },
    validateAndGenerate: async function() {
      this.$gtag.event('generate');
      this.jsonErrors = [];
      const result = validateJson(this.activeProject.json);
      if(!result.error) {
        this.hideSnackbar();
        this.callPrettyPrint();
        const jsonLength = this.getJsonLength(this.activeProject.json);
        if(jsonLength > 2000) {
          this.setSnackbar('orange darken-2', `Exceeded character limit: ${jsonLength} / 2000`, -1);
          return;
        }
        await this.generate();
      } else {
        this.jsonErrors.push({line: result.line, color: 'red'});
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
      this.snackbar.visible = false;
    },
    existsProjectName: function(): Project | null{
      for(const i in this.$store.state.projects.items){
        if(this.activeProject.name === this.$store.state.projects.items[i].name){
          return this.$store.state.projects.items[i];
        }
      }
      return null;
    },
    save: async function (){
      this.$gtag.event('save-project');
      if(this.activeProject.name){
        const exists = this.existsProjectName();
        if (!exists && this.activeProject.id === -1) {
          this.setSnackbar('success', 'The new project was successfully created!', 5000);
          this.activeProject.id = 0;
          this.activeProject.ownerId = this.$store.state.auth.user.id;
          this.activeProject = await this.$store.dispatch('projects/addProject', this.activeProject);
        } else if(exists && exists.id !== this.activeProject.id) {
          this.setSnackbar('error', 'This name is already in use, please enter another name!', 5000);
        } else {
          this.setSnackbar('success', 'Project updated successfully!', 5000);
          await this.$store.dispatch('projects/updateProject', this.activeProject);
        }
      }else{
        this.setSnackbar('error', 'This name is incorrect!', 5000);
      }
    },
    undo: async function () {
      this.$gtag.event('undo');
      if (this.isPristine) {
        this.undoStack.pop();
      }
      const top = this.undoStack.top();
      if(top) {
        this.activeProject.json = top.content;
      }
      this.setSnackbar('info', 'Everything restored to its previous generated state', 5000);
    },
    changeProjectName: function(name: string){
      this.$gtag.event('change-project-name');
      this.activeProject.name = name;
    },
    download: async function() {
      if(!this.downLoading){
        this.downLoading = true;
        this.$gtag.event('download');
        await Promise.all([
          this.delay(3000),
          api.download({data: this.activeProject.json, nameSpace: this.toCamelCase(this.activeProject.name)})
        ]);
        this.downLoading = false;
      }
    },
    openFolder: function(idx: number){
      this.$gtag.event('open-folder');
      this.openPath = '';
      for(let i = 0; i < idx; i++){
        if(this.activeFile.path.split('/')[i]){
          this.openPath += `${this.activeFile.path.split('/')[i]}/`;
        }
      }
      this.openPath = this.openPath.slice(0, -1);
      this.drawer = true;
    },
    selectFile: function(data: GeneratedFile){
      this.$gtag.event('select-file');
      this.activeFile = data;
      this.closeDrawer();
      this.setHighlightedDifferences();
    },
    setDrawer: function(){
      this.$gtag.event('set-drawer');
      this.drawer = !this.drawer;
      if(!this.drawer){
        this.openPath = '';
      }
    },
    closeDrawer: function(){
      if(this.drawer){
        this.$gtag.event('close-drawer');
        this.drawer = false;
        this.openPath = '';
      }
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
  .headBar{
    margin-left: 40px;
    margin-top: 17px;
    z-index: 99;
  }
  @media screen and (max-width: 960px) {
    .headBar {
      margin-left: 70px;
    }
  }
  @media screen and (max-width: 600px) {
    .headBar {
      margin-top: 60px;
      margin-left: unset;
    }
    .icons button{
      margin-top: 5px;
    }
  }
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
  .pathElement{
    cursor: pointer!important;
  }
  .pathElement:hover{
    opacity: 0.6;
  }
</style>
