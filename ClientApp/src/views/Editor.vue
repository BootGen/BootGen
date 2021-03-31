<template>
  <v-container fluid class="editor">
    <v-row class="d-flex align-center ma-0 pa-0">
      <v-col lg="5" md="6" sm="8" cols="12" class="pa-0 headBar" v-if="$store.state.auth.jwt">
        <head-bar :activeProject="activeProject" @new-project="newProject" @change-project-name="changeProjectName"></head-bar>
      </v-col>
      <v-col cols="12" class="headBar pl-0 pt-6" v-else>
        <head-bar :activeProject="activeProject" @new-project="newProject" @change-project-name="changeProjectName"></head-bar>
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
                    <v-btn class="mr-2" color="white" elevation="1" fab small @click="openHelp = true" v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-help</v-icon>
                    </v-btn>
                    </template>
                  <span>Help</span>
                </v-tooltip>
                <help-dialog v-if="openHelp" @close-help="openHelp = false"></help-dialog>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small @click="undo" v-bind="attrs" v-on="on" :disabled="undoStack.length() < 2 && isPristine">
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
                <v-tooltip bottom v-if="$store.state.auth.jwt">
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small @click="openExplorer = true" v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-file-multiple</v-icon>
                    </v-btn>
                    </template>
                  <span>Files</span>
                </v-tooltip>
                <file-explorer v-if="$store.state.auth.jwt && openExplorer" @select-project="selectProject" @close-explorer="openExplorer = false"></file-explorer>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small :disabled="activeProject.json == ''" @click="callPrettyPrint" v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-format-align-left</v-icon>
                    </v-btn>
                  </template>
                  <span>Formatting</span>
                </v-tooltip>
                
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small :disabled="isPristine" @click="validateAndGenerate()" v-bind="attrs" v-on="on">
                      <v-icon color="primary" v-if="!generateLoading">mdi-arrow-right-bold</v-icon>
                      <div v-if="generateLoading">
                        <v-progress-circular
                          indeterminate  
                          :size="35" 
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
          <code-mirror cmId="cm0" v-model="activeProject.json" mode="json" :readOnly="false" :linesToColor="cmLinesToColor" @on-scroll="validateAndGenerate" @cursor-into-view="closeDrawer"></code-mirror>
        </base-material-generator-card>
      </v-col>

      <v-col cols="12" md="6" class="pa-0">
        <base-material-generator-card>
          <template v-slot:heading>
            <div class="d-flex display-1 font-weight-light align-center justify-space-between pa-2">
              <div class="text-break" v-if="activeFile.path && activeFile.path !== ''">
                <span v-for="(part, i) in activeFile.path.split('/')" :key="i" @click="openFolder(i+1)">
                  {{ part }}/
                </span>
                <span @click="openFolder(activeFile.path.split('/').length)">
                  {{ activeFile.name }}
                </span>
              </div>
              <div v-else>
                <span @click="openFolder(0)">
                  {{ activeFile.name }}
                </span>
              </div>
              <div class="d-flex">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn color="white" class="mr-2" elevation="1" :disabled="!isPristine" @click="download" fab small v-bind="attrs" v-on="on">
                      <v-icon color="primary" v-if="!downLoading">mdi-download</v-icon>
                      <div v-if="downLoading">
                        <v-progress-circular
                          indeterminate  
                          :size="35" 
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
          <code-mirror cmId="cm1" :value="activeFile.content" :mode="getMode()" :readOnly="true" @cursor-into-view="closeDrawer"></code-mirror>
        </base-material-generator-card>
      </v-col>
    </v-row>
    <snackbar v-if="snackbar.visible" :snackbar="snackbar"></snackbar>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import CodeMirror from "../components/CodeMirror.vue";
import TreeView from "../components/TreeView.vue";
import {validateJson, prettyPrint} from "../utils/PrettyPrint";
import {UndoStack} from "../utils/UndoStack";
import FileExplorer from "../components/FileExplorer.vue";
import HelpDialog from "../components/HelpDialog.vue";
import HeadBar from "../components/HeadBar.vue";
import Snackbar from "../components/Snackbar.vue";
import {Project} from "../models/Project";
import {GeneratedFile} from "../models/GeneratedFile";
import {CRC32} from 'crc_32_ts';
import axios from 'axios';

export default Vue.extend({
  components: {
    FileExplorer,
    HelpDialog,
    HeadBar,
    Snackbar,
    CodeMirror,
    TreeView,
  },
  created: async function(){
    this.initialProject.json = (await axios.get(`${this.$root.$data.baseUrl}/example_input.json`, {responseType: "text"})).data;
    if(this.$store.state.projects.lastProject.json){
      this.activeProject = {...this.$store.state.projects.lastProject};
      this.generatedFiles = [...this.$store.state.projects.lastGeneratedFiles];
      this.setActiveFile();
    }else{
      this.activeProject = {...this.initialProject};
      await this.validateAndGenerate();
    }
  },
  computed: {
    isPristine: function(){
        const top = this.undoStack.top();
        if(top){
          if(top.crc32 === CRC32.str(this.activeProject.json)){
            return true;
          }
        }
      return false;
    },
  },
  data: function () {
    return {
      openExplorer: false,
      openHelp: false,
      generatedFiles: Array<GeneratedFile>(),
      initialProject: {id: -1, ownerId: -1, name: "", json: "{}"},
      undoStack: new UndoStack(),
      activeProject: {id: -1, ownerId: -1, name: "", json: ""},
      activeFile: {} as GeneratedFile,
      snackbar: {
        dismissible: true,
        visible: false,
        type: "",
        icon: "mdi-alert-circle",
        text: "",
        timeout: 5000,
      },
      projectName: "",
      cmLinesToColor: Array<{line: number; color: string}>(),
      drawer: false,
      openPath: "",
      generateLoading: false,
      downLoading: false,
    };
  },
  methods: {
    getMode: function(){
      if(this.activeFile.name){
        return this.activeFile.name.split('.')[1];
      }
    },
    generate: async function(){
      if(!this.generateLoading){
        this.generateLoading = true;
        let nameSpace = "Test"
        if(this.activeProject.name !== ""){
          nameSpace = this.activeProject.name;
        }
        const generate = await this.$store.dispatch("generate", {data: this.activeProject.json, nameSpace: this.camalize(nameSpace)});
        if(generate.errorMessage){
          this.setSnackbar("orange darken-2", generate.errorMessage, -1);
          if(generate.errorLine !== ""){
            this.cmLinesToColor.push({line: generate.errorLine-1, color: "red"});
          }
          this.generateLoading = false;
          return;
        }
        this.generatedFiles = generate.generatedFiles;
        this.$store.commit("projects/setLastProject", this.activeProject);
        this.$store.commit("projects/setLastGeneratedFiles", this.generatedFiles);
        this.undoStack.push(this.activeProject.json);
        this.setActiveFile();
        this.generateLoading = false;
      }
    },
    getJsonLength: function(json: string): number{
      json = json.replace(/ {2}/g, "");
      json = json.replace(/": /g, "\":");
      json = json.replace(/[\n\t\r]/g, "");
      return json.length;
    },
    setActiveFile: function(){
      if(!this.activeFile.name){
        for(let i = 0; i < this.generatedFiles.length; i++){
          if(this.generatedFiles[i].name === "restapi.yml" && this.generatedFiles[i].path === ""){
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
    },
    callPrettyPrint: function(){
      this.$gtag.event('pretty-print');
      const result = validateJson(this.activeProject.json);
      if (result.error) {
        this.cmLinesToColor.push({line: result.line, color: "red"});
        this.setSnackbar("orange darken-2", result.message, -1);
      }
      this.activeProject.json = prettyPrint(this.activeProject.json);
      this.hideSnackbar();
    },
    newProject: async function(){
      this.$gtag.event('create-new-project');
      this.activeProject = {...this.initialProject};
      this.undoStack.clear();
      await this.generate();
    },
    validateAndGenerate: async function() {
      this.$gtag.event('generate');
      this.cmLinesToColor = [];
      const result = validateJson(this.activeProject.json);
      if(!result.error) {
        this.hideSnackbar();
        this.callPrettyPrint();
        const jsonLength = this.getJsonLength(this.activeProject.json);
        if(jsonLength > 2000) {
          this.setSnackbar("orange darken-2", `Exceeded character limit: ${jsonLength} / 2000`, -1);
          return;
        }
        await this.generate();
      } else {
        this.cmLinesToColor.push({line: result.line, color: "red"});
        this.setSnackbar("orange darken-2", result.message, -1);
      }
    },
    setSnackbar: function(type: string, text: string, timeout: number){
      this.snackbar.dismissible = true,
      this.snackbar.timeout = timeout;
      this.snackbar.type = type;
      this.snackbar.text = text;
      this.snackbar.visible = true;
    },
    hideSnackbar: function(){
      this.snackbar.visible = false;
    },
    selectProject: function(project: Project){
      this.$gtag.event('select-project');
      let select = true;
      if(this.activeProject.id === -1){
        select = confirm("changes will not be saved!");
      }
      if(select){
        project.json = project.json.split("'").join('"');
        this.activeProject = project;
        this.validateAndGenerate();
      }
      this.undoStack.clear();
      this.openExplorer = false;
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
      if(this.projectName !== ""){
        this.activeProject.name = this.projectName;
      }
      if(this.activeProject.name){
        const exists = this.existsProjectName();
        if (!exists && this.activeProject.id === -1) {
          this.setSnackbar("success", "The new project was successfully created!", 5000);
          this.activeProject.id = 0;
          this.activeProject.ownerId = this.$store.state.auth.user.id;
          this.activeProject = await this.$store.dispatch("projects/addProject", this.activeProject);
        } else if(exists && exists.id !== this.activeProject.id) {
          this.setSnackbar("error", "This name is already in use, please enter another name!", 5000);
        } else {
          this.setSnackbar("success", "Project updated successfully!", 5000);
          await this.$store.dispatch("projects/updateProject", this.activeProject);
        }
      }else{
        this.setSnackbar("error", "This name is incorrect!", 5000);
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
      this.setSnackbar("info", "Everything restored to its previous generated state", 5000);
    },
    changeProjectName: function(name: string){
      this.$gtag.event('change-project-name');
      this.projectName = name;
    },
    download: async function() {
      if(!this.downLoading){
        this.downLoading = true;
        this.$gtag.event('download');
        let nameSpace = "Test"
        if(this.activeProject.name !== ""){
          nameSpace = this.activeProject.name;
        }
        await this.$store.dispatch("download", {data: this.activeProject.json, nameSpace: this.camalize(nameSpace)});
        this.downLoading = false;
      }
    },
    openFolder: function(idx: number){
      this.$gtag.event('open-folder');
      this.openPath = "";
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
    },
    setDrawer: function(){
      this.$gtag.event('set-drawer');
      this.drawer = !this.drawer;
      if(!this.drawer){
        this.openPath = "";
      }
    },
    closeDrawer: function(){
      if(this.drawer){
        this.$gtag.event('close-drawer');
        this.drawer = false;
        this.openPath = "";
      }
    },
    camalize: function(str: string) {
      return str.replace(/(?:^\w|[A-Z]|\b\w)/g, function(word, index) {
        return index === 0 ? word.toLowerCase() : word.toUpperCase();
      }).replace(/\s+/g, '');
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
</style>
