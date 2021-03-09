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
                    <v-btn class="mr-2" color="white" elevation="1" fab small :disabled="activeProject.json == ''" @click="prettyPrint(activeProject.json)" v-bind="attrs" v-on="on">
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
          <codemirror id="cm0" v-model="activeProject.json" :options="cmOptions" @scroll="onScroll"/>
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
import FileReader from "../components/FileReader.vue";
import FileExplorer from "../components/FileExplorer.vue";
import HelpDialog from "../components/HelpDialog.vue";
import HeadBar from "../components/HeadBar.vue";
import Snackbar from "../components/Snackbar.vue";
import { Project } from "../models/Project";
import { GeneratedFile } from "../models/GeneratedFile";
import { codemirror } from 'vue-codemirror'
import 'codemirror/lib/codemirror.css'
import "codemirror/mode/javascript/javascript.js";
import 'codemirror/theme/material.css'
import axios from 'axios'

export default Vue.extend({
  components: {
    FileReader,
    FileExplorer,
    HelpDialog,
    HeadBar,
    Snackbar,
    codemirror
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
      cmOptions: {
        theme: 'material',
        tabSize: 2,
        mode: 'text/javascript',
        lineNumbers: true,
        line: true,
      },
      snackbar: {
        dismissable: true,
        visible: false,
        type: "",
        icon: "mdi-alert-circle",
        text: "",
        timeout: 5000,
      },
      projectName: "",
      minLine: 5000000,
      maxLine: -1,
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
      if(this.formatJson(prevJson) !== this.formatJson(this.activeProject.json)){
        this.previousJson.push(this.activeProject.json);
      }
      this.prettyPrint(this.activeProject.json);
    },
    onScroll: function() {
      this.snackbar.visible = false;
      this.unsetHighlight(0, "CodeMirror-line");
      const elementById = document.getElementById("cm0");
      if(!elementById){
        return;
      }
      const list = elementById.getElementsByClassName("CodeMirror-linenumber");
      this.minLine = 5000000;
      this.maxLine = -1;
      for (let i = 1; i < list.length; i++) {
        const textval = list[i].textContent;
        if (textval) {
          const lineNum = parseInt(textval, 10);
          if (lineNum < this.minLine)
            this.minLine = lineNum;
          if (lineNum > this.maxLine)
            this.maxLine = lineNum;
        }
      }
      const jsonError = this.jsonError(this.activeProject.json);
      if(jsonError !== false){
        this.highlightLine(0, jsonError.line, jsonError.message, "red");
      }
    },
    newProject: async function(){
      this.activeProject = {...this.initialProject};
      this.previousJson = [];
      this.generate(this.activeProject.json);
    },
    setJson: async function(json: string) {
      const jsonError = this.jsonError(json);
      if(jsonError === false){
        this.snackbar.visible = false;
        this.unsetHighlight(0, "CodeMirror-line");
        this.generate(json);
      }else{
        this.highlightLine(0, jsonError.line, jsonError.message, "red");
      }
    },
    highlightLine: function(cmId: number, line: number, errorMessage: string, color: string){
      this.snackbar.dismissable = true,
      this.snackbar.timeout = -1;
      this.snackbar.type = "orange darken-2";
      this.snackbar.text = errorMessage;
      this.snackbar.visible = true;
      this.unsetHighlight(0, "CodeMirror-line");
      const elementById = document.getElementById("cm" + cmId);
      if(!elementById || (this.minLine > line && this.minLine < 5000000) || (this.maxLine < line && this.maxLine > -1)){
        return;
      }
      if(this.minLine < line && line < this.maxLine){
        elementById.getElementsByClassName("CodeMirror-line")[line-this.minLine+2].setAttribute("style", `background-color:${color};`);
      }else{
        elementById.getElementsByClassName("CodeMirror-line")[line].setAttribute("style", `background-color:${color};`);
      }
    },
    getLine: function(idx: number, str: string): number{
      if(navigator.userAgent.indexOf("Firefox") != -1){
        return idx;
      }
      let charCount = 0;
      const strArray: string[] = str.split("\n");
      for(let i = 0; i < strArray.length; i++){
        charCount += strArray[i].length+1;
        if(charCount >= idx){
          return i;
        }
      }
      return -1;
    },
    jsonError: function (text: string) {
      const json = text.split("\n").filter(line => !line.trim().startsWith("//")).join("\n");
      try {
        JSON.parse(json);
        return false;
      } catch (err) {
        const idx = err.message.match(/\d+/g)[0]-1;
        const lines = text.split("\n");
        let errorLine = this.getLine(idx, json);
        for(let i = 0; i < errorLine; i++){
          if(lines[i].includes("//")){
            errorLine++;
          }
        }
        return {line: errorLine, message: err.message};
      }
    },
    unsetHighlight(cmId: number, from: string){
      const elementById = document.getElementById("cm" + cmId);
      if(!elementById){
        return;
      }
      const e = elementById.getElementsByClassName(from);
      for(let i = 0; i < e.length; i++){
        e[i].setAttribute("style", "background-color: unset;");
      }
    },
    prettyPrint: function(json: string){
      const jsonError = this.jsonError(json);
      if(jsonError !== false){
        this.highlightLine(0, jsonError.line, jsonError.message, "red");
      }else{
        json = json.split("\r").join("");
        const comments = json.match(/(\/\/.*)(\n)/g);
        json = json.replace(/(\/\/.*)(\n)/g, "#>comment<");
        json = this.formatJson(json);
        let lines = this.indentLines(json.split("\n"));
        if(comments){
          lines = this.replaceToComment(comments, lines);
        }
        this.activeProject.json = lines.join("\n");
      }
    },
    formatJson: function(json: string): string{
      json = json.split("\r").join("");
      json = json.replace(/( )*(}|]|{|\[|,)/g, "$2");
      json = json.replace(/( {2})*/g, "");
      json = json.replace(/(: *)/g, ": ");
      json = json.split("'").join("\"");
      json = json.split("\t").join("");
      json = json.split("\n").join("");
      json = json.split("#>comment<").join("#>comment<\n");
      json = json.split("{").join("{\n");
      json = json.split("}").join("}\n");
      json = json.split("[").join("[\n");
      json = json.split("]").join("]\n");
      json = json.split(",").join(",\n");
      json = json.split("\n,").join(",");
      json = json.split("\"}").join("\"\n}");
      json = json.split("\"]").join("\"\n]");
      return json;
    },
    indentLines: function(lines: string[]): string[]{
      let tabCount = 0;
      for(let i = 0; i < lines.length; i++){
        if(lines[i].includes("{") || lines[i].includes("[")){
          lines[i] = "\t".repeat(tabCount).concat(lines[i].trim());
          tabCount++;
        }else if(lines[i].includes("}") || lines[i].includes("]")){
          --tabCount;
          lines[i] = "\t".repeat(tabCount).concat(lines[i].trim());
        }else{
          lines[i] = "\t".repeat(tabCount).concat(lines[i].trim());
        }
      }
      return lines;
    },
    replaceToComment: function(comments: string[], lines: string[]): string[]{
      comments.forEach(comment => {
        for(let i = 0; i < lines.length; i++){
          if(lines[i].includes("#>comment<")){
            lines[i] = lines[i].replace(/#>comment</g, comment.slice(0, -1));
            break;
          }
        }
      });
      return lines;
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
          this.snackbar.type = "success";
          this.snackbar.text = "The new project was successfully created!";
          this.activeProject.id = 0;
          this.activeProject.ownerId = this.$store.state.auth.user.id;
          this.activeProject = await this.$store.dispatch("projects/addProject", this.activeProject);
        }else if(exists && exists.id !== this.activeProject.id){
          this.snackbar.type = "error";
          this.snackbar.text = "This name is already in use, please enter another name!";
        }else{   
          this.snackbar.type = "success";
          this.snackbar.text = "Project updated successfully!";
          await this.$store.dispatch("projects/updateProject", this.activeProject);
        }
        this.generate(this.activeProject.json);
      }else{
        this.snackbar.type = "error";
        this.snackbar.text = "This name is incorrect!";
      }
      this.snackbar.visible = true;
    },
    undo: async function (){
      this.activeProject.json = this.previousJson[this.previousJson.length-2];
      this.previousJson.pop();
      this.generate(this.activeProject.json);
      this.snackbar.type = "info";
      this.snackbar.text = "Everything restored to its previous saved state";
      this.snackbar.visible = true;
    },
    undoAll: async function(){
      if(this.activeProject.id !== -1){
        console.log(await this.$store.dispatch("projects/getProject", this.activeProject.id));
      }else{
        this.activeProject = {...this.initialProject};
      }
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
  .vue-codemirror{
    width: 100%;
    height: calc(100% - 40px);
    margin-bottom: 0!important;
    padding-bottom: 0!important;
  }
  .CodeMirror-scroll{
    overflow: auto!important;
    margin: 0!important;
    padding: 0!important;
  }
  .CodeMirror{
    height: 100%;
    z-index: 0;
  }
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