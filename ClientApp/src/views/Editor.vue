<template>
  <v-container fluid class="editor">
    <v-row class="d-flex align-center ma-0 pa-0">
      <v-col lg="5" md="6" sm="8" cols="12" class="pa-0 headBar" v-if="$store.state.auth.jwt">
        <head-bar :activeProjectName="activeProject.name" :backends="backends" :frontends="frontends" @new-project="createNewProject" @change-project-name="changeProjectName"></head-bar>
      </v-col>
      <v-col cols="12" class="pa-0 headBar" v-else>
        <head-bar :activeProjectName="activeProject.name" @new-project="createNewProject" @change-project-name="changeProjectName"></head-bar>
      </v-col>
    </v-row>
    <v-row class="d-flex align-center">
      <v-col xl="5" lg="5" md="12" class="pr-0 pl-0">
        <base-material-generator-card>
          <template v-slot:heading>
            <div class="d-flex align-center justify-space-between pa-2">
              <div class="d-flex">
                <span class="display-1 font-weight-light pa-2">JSON</span>
                <v-icon class="mr-2">mdi-arrow-right</v-icon>
                <div class="d-flex select">
                  <v-select
                    v-model="activeProject.backend"
                    :items="backends"
                    @change="change('backend')"
                    dense
                    hide-details              
                  ></v-select>
                  <v-select
                    v-model="activeProject.frontend"
                    :items="frontends"
                    @change="change('frontend')"
                    dense
                    hide-details                
                  ></v-select>
                </div>
              </div>
              <div>
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
                    <v-btn class="mr-2" color="white" elevation="1" fab small @click="save" v-bind="attrs" v-on="on" :disabled="saveDisabled">
                      <v-icon color="primary">mdi-floppy</v-icon>
                    </v-btn>
                    </template>
                  <span>Save</span>
                </v-tooltip>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small :disabled="activeProject.json === ''" @click="onPrettyPrintClicked" v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-format-align-left</v-icon>
                    </v-btn>
                  </template>
                  <span>Formatting</span>
                </v-tooltip>

                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small :disabled="isPristine || generateLoading || activeProject.name === ''" @click="onGenerateClicked" v-bind="attrs" v-on="on">
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
          <code-mirror cmId="cm0" v-model="activeProject.json" mode="json" :readOnly="false" :linesToColor="jsonErrors" @change-json="removeErrors"></code-mirror>
        </base-material-generator-card>
      </v-col>
      <v-col xl="5" lg="5" md="8" sm="8" class="pr-0 pl-0">
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
                  <span v-if="isCompare">Show Changes: On</span>
                  <span v-else>Show Changes: Off</span>
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
              </div>
            </div>
          </template>
          <code-mirror cmId="cm1" :value="activeFile.content" :mode="getMode()" :readOnly="true" :linesToColor="highlightedDifferences"></code-mirror>
        </base-material-generator-card>
      </v-col>
      <v-col xl="2" lg="2" md="4" sm="4" class="pr-0 pl-0">
        <file-browser :generatedFiles="generatedFiles" :previousFiles="previousFiles" :openPath="openPath" :isCompare="isCompare" @select-file="selectFile"></file-browser>
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
    FileBrowser,
  },
  data: function () {
    return {
      generatedFiles: Array<GeneratedFile>(),
      previousFiles: Array<GeneratedFile>(),
      undoStack: new UndoStack(),
      crc32ProjectName: CRC32.str('My Project'),
      crc32Saved: 0,
      activeProject: {id: -1, ownerId: -1, name: 'My Project', json: '', backend: 'ASP.NET', frontend: 'Vue 2 + JS'},
      newProject: {id: -1, ownerId: -1, name: 'My Project', json: '', backend: 'ASP.NET', frontend: 'Vue 2 + JS'},
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
      backends: ['ASP.NET'],
      frontends: ['Vue 2 + JS', 'Vue 2 + TS', 'Vue 3', 'React'],
      backend: 'ASP.NET',
      frontend: 'Vue 2 + JS'
    };
  },
  created: async function(){
    this.newProject.json = JSON.stringify((await axios.get(`${this.$root.$data.baseUrl}/new_project_input.json`, {responseType: 'json'})).data);
    if(this.$store.state.projects.lastProject.json){
      this.activeProject = this.$store.state.projects.lastProject;
      this.generatedFiles = [...this.$store.state.projects.lastGeneratedFiles];
      this.backend = this.$store.state.projects.lastProject.backend;
      this.frontend = this.$store.state.projects.lastProject.frontend;
      this.callPrettyPrint();
      this.undoStack.push(this.activeProject.json);
      this.setActiveFile();
      this.crc32Saved = this.crc32ForSaving;
      this.crc32ProjectName = CRC32.str(this.activeProject.name);
    }else{
      this.activeProject.json = (await axios.get(`${this.$root.$data.baseUrl}/example_input.json`, {responseType: 'text'})).data;
      await this.validateAndGenerate();
    }
  },
  computed: {
    isPristine: function(): boolean {
      const top = this.undoStack.top();
      if(top){
        if((top.crc32 === CRC32.str(this.activeProject.json)) &&
          (this.crc32ProjectName === CRC32.str(this.activeProject.name)) &&
          (this.backend === this.activeProject.backend) &&
          (this.frontend === this.activeProject.frontend)
        ){
          return true;
        }
      }
      return false;
    },
    isJsonPristine: function(): boolean {
      const top = this.undoStack.top();
      if(top){
        if(top.crc32 === CRC32.str(this.activeProject.json)){
          return true;
        }
      }
      return false;
    },
    crc32ForSaving: function(): number {
      return CRC32.str(this.activeProject.name + this.activeProject.json);
    },
    saveDisabled: function(): boolean {
      return this.crc32Saved === this.crc32ForSaving;
    }
  },
  methods: {
    delay: function(ms: number): Promise<void> {
      return new Promise(resolve => setTimeout(resolve, ms));
    },
    getMode: function(): string {
      if(this.activeFile.name){
        return this.activeFile.name.split('.')[1];
      }
      return '';
    },
    removeErrors: function(){
      this.jsonErrors = [];
      this.hideSnackbar();
    },
    generate: async function(){
      if(!this.generateLoading){
        this.generateLoading = true;
        const generateResult: GenerateResponse = await api.generate({
          data: this.activeProject.json,
          nameSpace: this.toCamelCase(this.activeProject.name),
          backend: this.activeProject.backend,
          frontend: this.activeProject.frontend
        });
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
        this.backend = this.activeProject.backend;
        this.frontend = this.activeProject.frontend;
        this.undoStack.push(this.activeProject.json);
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
            const compare = new Compare(this.activeFile.content.split('\n'), this.previousFiles[i].content.split('\n'));
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
    setSelectedFile: function (fileName: string, filePath: string) {
      for (let i = 0; i < this.generatedFiles.length; i++) {
        if (this.generatedFiles[i].name === fileName && this.generatedFiles[i].path === filePath) {
          this.activeFile = this.generatedFiles[i];
          break;
        }
      }
    },
    setActiveFile: function(){
      if(!this.activeFile.name){
        this.setSelectedFile('restapi.yml', '');
      }else{
        this.setSelectedFile(this.activeFile.name, this.activeFile.path);
      }
      this.setHighlightedDifferences();
    },
    onPrettyPrintClicked: function(){
      this.$gtag?.event('pretty-print');
      this.callPrettyPrint();
    },
    callPrettyPrint: function(){
      const result = validateJson(this.activeProject.json);
      if (result.error) {
        this.jsonErrors.push({line: result.line, color: 'red'});
        this.setSnackbar('orange darken-2', result.message, -1);
      }
      this.activeProject.json = prettyPrint(this.activeProject.json);
      this.hideSnackbar();
    },
    createNewProject: async function(name: string, backend: string, frontend: string){
      this.$gtag?.event('create-new-project');
      this.newProject.name = name;
      this.newProject.backend = backend;
      this.newProject.frontend = frontend;
      this.activeProject = {...this.newProject};
      this.callPrettyPrint();
      this.save();
      this.undoStack.clear();
      await this.generate();
      this.highlightedDifferences = [];
    },
    onGenerateClicked: function () {
      this.$gtag?.event('generate');
      this.validateAndGenerate();
    },
    validateAndGenerate: async function() {
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
      if(this.activeProject.name){
        const project = this.getProjectByName(this.activeProject.name);
        if (!project && this.activeProject.id === -1) {
          this.setSnackbar('success', 'The new project was successfully created!', 5000);
          this.crc32Saved = this.crc32ForSaving;
          this.activeProject.id = 0;
          this.activeProject.ownerId = this.$store.state.auth.user.id;
          this.activeProject = await this.$store.dispatch('projects/addProject', this.activeProject);
        } else if(project && project.id !== this.activeProject.id) {
          this.setSnackbar('error', 'This name is already in use, please enter another name!', 5000);
        } else {
          this.crc32Saved = this.crc32ForSaving;
          this.setSnackbar('success', 'Project updated successfully!', 5000);
          await this.$store.dispatch('projects/updateProject', this.activeProject);
          this.setSnackbar('success', 'Project updated successfully!', 5000);
        }
      }else{
        this.setSnackbar('error', 'This name is incorrect!', 5000);
      }
    },
    undo: async function () {
      this.$gtag?.event('undo');
      if (this.isPristine) {
        this.undoStack.pop();
      }
      const top = this.undoStack.top();
      if(top) {
        this.activeProject.json = top.content;
      }
      this.setSnackbar('info', 'Everything restored to its previous generated state', 5000);
    },
    change: function(page: string){
      this.$gtag.event(`change-${page}`);
    },
    changeProjectName: function(name: string){
      this.$gtag?.event('change-project-name');
      this.activeProject.name = name.trim();
    },
    download: async function() {
      if(!this.downLoading){
        this.downLoading = true;
        this.$gtag?.event('download');
        await Promise.all([
          this.delay(3000),
          api.download({
            data: this.activeProject.json,
            nameSpace: this.toCamelCase(this.activeProject.name),
            backend: this.activeProject.backend,
            frontend: this.activeProject.frontend
          })
        ]);
        this.downLoading = false;
      }
    },
    openFolder: function(idx: number){
      this.$gtag?.event('click-on-path-element');
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
      this.activeFile = data;
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
  }
  .pathElement{
    cursor: pointer!important;
  }
  .pathElement:hover{
    opacity: 0.6;
  }
  .select{
    max-width: 250px;
  }
</style>
