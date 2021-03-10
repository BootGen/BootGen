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
                    <v-btn class="mr-2" color="white" elevation="1" fab small @click="undoAll" v-bind="attrs" v-on="on" :disabled="previousJson.length < 2">
                      <v-icon color="primary">mdi-replay</v-icon>
                    </v-btn>
                    </template>
                  <span>Rollback to the last saved state</span>
                </v-tooltip>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="mr-2" color="white" elevation="1" fab small @click="undo" v-bind="attrs" v-on="on" :disabled="previousJson.length < 2">
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
          <code-mirror cmId="cm0" :activeProject="activeProject" :error="error" @set-snackbar="setSnackbar"></code-mirror>
        </base-material-generator-card>
      </v-col>

      <v-col cols="12" md="6" class="pa-0">
        <file-reader :json="this.activeProject.json" :jsonName="this.activeProject.name" :files="generatedFiles"></file-reader>
      </v-col>
    </v-row>
    <snackbar v-if="snackbar.visible" :snackbar="snackbar"></snackbar>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import CodeMirror from "../components/CodeMirror.vue";
import { prettyPrint, formatJson, jsonError } from "../utils/PrettyPrint";
import FileReader from "../components/FileReader.vue";
import FileExplorer from "../components/FileExplorer.vue";
import HelpDialog from "../components/HelpDialog.vue";
import HeadBar from "../components/HeadBar.vue";
import Snackbar from "../components/Snackbar.vue";
import { Project } from "../models/Project";
import { GeneratedFile } from "../models/GeneratedFile";
import axios from 'axios';

export default Vue.extend({
  components: {
    FileReader,
    FileExplorer,
    HelpDialog,
    HeadBar,
    Snackbar,
    CodeMirror,
  },
  created: async function(){
    this.initialProject.json = (await axios.get("example_input.json", {responseType: "text"})).data;
    this.activeProject = {...this.initialProject};
    await this.setJson(this.activeProject.json);
  },
  data: function () {
    return {
      openExplorer: false,
      openHelp: false,
      generatedFiles: Array<GeneratedFile>(),
      initialProject: {id: -1, ownerId: -1, name: "", json: "{}"},
      previousJson: Array<string>(),
      activeProject: {id: -1, ownerId: -1, name: "", json: ""},
      snackbar: {
        dismissible: true,
        visible: false,
        type: "",
        icon: "mdi-alert-circle",
        text: "",
        timeout: 5000,
      },
      projectName: "",
      error: {line: -1, message: ""},
    };
  },
  methods: {
    generate: async function(json: string){
      let nameSpace = "Test"
      if(this.activeProject.name !== ""){
        nameSpace = this.activeProject.name;
      }
      const generate = await this.$store.dispatch("generate", {data: json, nameSpace: this.camalize(nameSpace)});
      this.generatedFiles = generate.generatedFiles;
      this.activeProject.json = json;
      let prevJson = "";
      if(this.previousJson[this.previousJson.length-1]){
        prevJson = this.previousJson[this.previousJson.length-1];
      }
      if(formatJson(prevJson) !== formatJson(this.activeProject.json)){
        this.previousJson.push(this.activeProject.json);
      }
      this.callPrettyPrint();
    },
    callPrettyPrint: function(){
      const result = prettyPrint(this.activeProject.json);
      if(typeof(result) === "string"){
        this.activeProject.json = result;
      }else{
        this.error = result;
        this.setSnackbar("orange darken-2", this.error.message, true, -1);
      }
    },
    newProject: async function(){
      this.activeProject = {...this.initialProject};
      this.previousJson = [];
      this.generate(this.activeProject.json);
    },
    setJson: async function(json: string) {
      const error = jsonError(json);
      if(error === false){
        this.error = {line: -1, message: ""};
        this.setSnackbar();
        this.generate(json);
      }else{
        this.error = error;
        this.setSnackbar("orange darken-2", error.message, true, -1);
      }
    },
    setSnackbar: function(type = "", text = "", visible = false, timeout = -1){
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
      this.previousJson = [];
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
        this.generate(this.activeProject.json);
      }else{
        this.setSnackbar("error", "This name is incorrect!", true, 5000);
      }
    },
    undo: async function (){
      this.activeProject.json = this.previousJson[this.previousJson.length-2];
      this.previousJson.pop();
      this.generate(this.activeProject.json);
      this.setSnackbar("info", "Everything restored to its previous generated state", true, 5000);
    },
    undoAll: async function(){
      if(this.activeProject.id !== -1){
        await this.$store.dispatch("projects/getProject", this.activeProject.id);
      }else{
        this.activeProject = {...this.initialProject};
      }
      this.setSnackbar("info", "Everything restored to its previous saved state", true, 5000);
      this.previousJson = [this.activeProject.json];
    },
    changeProjectName: function(name: string){
      this.projectName = name;
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
</style>