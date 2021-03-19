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
                    <v-btn class="mr-2" color="white" elevation="1" fab small @click="undo" v-bind="attrs" v-on="on" :disabled="undoStack.length < 2">
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
                    <v-btn class="mr-2" color="white" elevation="1" fab small :disabled="activeProject.json == ''" @click="setJson(activeProject.json)" v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-arrow-right-bold</v-icon>
                    </v-btn>
                    </template>
                  <span>Generate</span>
                </v-tooltip>
              </div>
            </div>
          </template>
          <code-mirror cmId="cm0" :content="activeProject.json" mode="json" :readOnly="false" :errorLine="errorLine" @change-content="changeProjectContent" @set-snackbar="setSnackbar" @cursor-into-view="closeDrawer"></code-mirror>
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
                    <v-btn color="white" class="mr-2" elevation="1" :disabled="failedGenerate" @click="download" fab small v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-download</v-icon>
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
          <code-mirror cmId="cm1" :content="activeFile.content" :mode="getMode()" :readOnly="true" @set-snackbar="setSnackbar" @cursor-into-view="closeDrawer"></code-mirror>
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
import { prettyPrint, formatJson, jsonError } from "../utils/PrettyPrint";
import FileExplorer from "../components/FileExplorer.vue";
import HelpDialog from "../components/HelpDialog.vue";
import HeadBar from "../components/HeadBar.vue";
import Snackbar from "../components/Snackbar.vue";
import { Project } from "../models/Project";
import { GeneratedFile } from "../models/GeneratedFile";
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
      this.undoStack.push(this.activeProject.json);
      this.failedGenerate = false;
      this.setActiveFile();
    }else{
      this.activeProject = {...this.initialProject};
      await this.setJson(this.activeProject.json);
    }
  },
  data: function () {
    return {
      openExplorer: false,
      openHelp: false,
      generatedFiles: Array<GeneratedFile>(),
      initialProject: {id: -1, ownerId: -1, name: "", json: "{}"},
      undoStack: Array<string>(),
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
      errorLine: -1,
      drawer: false,
      openPath: "",
      failedGenerate: true
    };
  },
  methods: {
    changeProjectContent: function(content: string){
      if(formatJson(content) !== formatJson(this.activeProject.json)){
        if(this.undoStack.length > 0 && this.undoStack[this.undoStack.length-1] !== "Ungenerated change"){
          this.undoStack.push("Ungenerated change");
        }
      }
      this.activeProject.json = content;
    },
    getMode: function(){
      if(this.activeFile.name){
        return this.activeFile.name.split('.')[1];
      }
    },
    generate: async function(json: string){
      const jsonLength = this.getJsonLength(json);
      if(jsonLength > 2000){
        this.setSnackbar("orange darken-2", `Exceeded character limit: ${jsonLength} / 2000`, true, -1);
        return;
      }
      let nameSpace = "Test"
      if(this.activeProject.name !== ""){
        nameSpace = this.activeProject.name;
      }
      const generate = await this.$store.dispatch("generate", {data: json, nameSpace: this.camalize(nameSpace)});
      if(generate.errorMessage){
        this.setSnackbar("orange darken-2", generate.errorMessage, true, -1);
        if(generate.errorLine !== ""){
          this.errorLine = generate.errorLine-1;
        }
        return;
      }
      this.generatedFiles = generate.generatedFiles;
      this.failedGenerate = false;
      this.activeProject.json = json;
      this.$store.commit("projects/setLastProject", this.activeProject);
      this.$store.commit("projects/setLastGeneratedFiles", this.generatedFiles);
      let prevJson = "";
      if(this.undoStack[this.undoStack.length-1] === "Ungenerated change"){
        this.undoStack.pop();
      }
      if(this.undoStack.length > 0){
        prevJson = this.undoStack[this.undoStack.length-1];
      }
      if(formatJson(prevJson) !== formatJson(this.activeProject.json)){
        this.undoStack.push(this.activeProject.json);
      }
      this.setActiveFile();
      this.callPrettyPrint();
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
      const result = prettyPrint(this.activeProject.json);
      if(typeof(result) === "string"){
        this.activeProject.json = result;
      }else{
        this.errorLine = result.line;
        this.setSnackbar("orange darken-2", result.message, true, -1);
      }
    },
    newProject: async function(){
      this.activeProject = {...this.initialProject};
      this.undoStack = [];
      await this.generate(this.activeProject.json);
    },
    setJson: async function(json: string) {
      const error = jsonError(json);
      if(error === false){
        this.errorLine = -1;
        this.setSnackbar();
        await this.generate(json);
      }else{
        this.errorLine = error.line;
        this.setSnackbar("orange darken-2", error.message, true, -1);
      }
    },
    setSnackbar: function(type = "", text = "", visible = false, timeout = -1){
      if(type === ""){
        this.failedGenerate = false;
      }else if(type === "orange darken-2"){
        this.failedGenerate = true;
      }
      this.snackbar.dismissible = true,
      this.snackbar.timeout = timeout;
      this.snackbar.type = type;
      this.snackbar.text = text;
      this.snackbar.visible = visible;
    },
    selectProject: function(project: Project){
      let select = true;
      if(this.activeProject.id === -1){
        select = confirm("changes will not be saved!");
      }
      if(select){
        project.json = project.json.split("'").join('"');
        this.activeProject = project;
        this.setJson(this.activeProject.json);
      }
      this.undoStack = [];
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
      if(this.projectName !== ""){
        this.activeProject.name = this.projectName;
      }
      if(this.activeProject.name){
        const exists = this.existsProjectName();
        if(!exists && this.activeProject.id === -1){
          this.setSnackbar("success", "The new project was successfully created!", true, 5000);
          this.activeProject.id = 0;
          this.activeProject.ownerId = this.$store.state.auth.user.id;
          this.activeProject = await this.$store.dispatch("projects/addProject", this.activeProject);
        }else if(exists && exists.id !== this.activeProject.id){
          this.setSnackbar("error", "This name is already in use, please enter another name!", true, 5000);
        }else{   
          this.setSnackbar("success", "Project updated successfully!", true, 5000);
          await this.$store.dispatch("projects/updateProject", this.activeProject);
        }
        await this.generate(this.activeProject.json);
      }else{
        this.setSnackbar("error", "This name is incorrect!", true, 5000);
      }
    },
    undo: async function (){
      this.undoStack.pop();
      this.activeProject.json = this.undoStack[this.undoStack.length-1];
      await this.generate(this.activeProject.json);
      this.setSnackbar("info", "Everything restored to its previous generated state", true, 5000);
    },
    changeProjectName: function(name: string){
      this.projectName = name;
    },
    download: function() {
      let nameSpace = "Test"
      if(this.activeProject.name !== ""){
        nameSpace = this.activeProject.name;
      }
      this.$store.dispatch("download", {data: this.activeProject.json, nameSpace: this.camalize(nameSpace)});
    },
    openFolder: function(idx: number){
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
      this.activeFile = data;
      this.closeDrawer();
    },
    setDrawer: function(){
      this.drawer = !this.drawer;
      if(!this.drawer){
        this.openPath = "";
      }
    },
    closeDrawer: function(){
      this.drawer = false;
      this.openPath = "";
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