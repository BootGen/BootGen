<template>
  <v-container fluid>
    <div class="toolBar d-flex align-center justify-space-between">
      <div class="d-flex align-center">
        <v-toolbar-title class="font-weight-light">Editing -</v-toolbar-title>
        <v-text-field v-model="activeProject.name" placeholder="Name your project" type="text" required></v-text-field>
        <div class="d-flex align-center pa-1" @click="save">
          <v-btn class="mr-4" color="primary" small>Save</v-btn>
        </div>
        <div class="d-flex align-center pa-1" @click="close">
          <v-btn class="mr-4" color="primary" small>Cancel</v-btn>
        </div>
      </div>
      <v-btn class="ml-2" v-if="this.$store.state.jwt" min-width="0" text to="/profile">
        <v-icon>mdi-account</v-icon>
      </v-btn>
    </div>
    <v-snackbar v-model="snackbar.visible" :color="snackbar.type" timeout="5000" top>
      <v-layout align-center justify-space-between>
        <div class="d-flex align-center">
          <v-icon class="pr-3" dark large>{{ snackbar.icon }}</v-icon>
          <div>{{ snackbar.text }}</div>
        </div>
        <v-btn icon @click="snackbar.visible = false">
          <v-icon>mdi-close-thick</v-icon>
        </v-btn>
      </v-layout>
    </v-snackbar>
    <options :project="activeProject" @select-project="selectProject"></options>
    <v-row class="d-flex align-center">
      <v-col cols="12" md="6" class="pr-0 pl-0">
        <base-material-generator-card>
          <template v-slot:heading>
            <div class="d-flex align-center justify-space-between pa-2">
              <div class="display-1 font-weight-light pa-2">
                JSON
              </div>
              <div>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn color="white" elevation="1" fab small :disabled="activeProject.json == ''" @click="prettyPrint(activeProject.json)" v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-format-align-left</v-icon>
                    </v-btn>
                  </template>
                  <span>Formatting</span>
                </v-tooltip>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn class="ml-2" color="white" elevation="1" fab small :disabled="activeProject.json == ''" @click="setJson(activeProject.json)" v-bind="attrs" v-on="on">
                      <v-icon color="primary">mdi-arrow-right-bold</v-icon>
                    </v-btn>
                    </template>
                  <span>Generation</span>
                </v-tooltip>
              </div>
            </div>
          </template>
          <codemirror v-model="activeProject.json" :options="cmOptions" />
        </base-material-generator-card>
      </v-col>
      <v-col cols="12" md="6" class="pa-0">
        <file-reader :files="generatedFiles"></file-reader>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import FileReader from "../components/FileReader.vue";
import Options from "../components/Options.vue";
import { Project } from "../models/Project";
import { codemirror } from 'vue-codemirror'
import 'codemirror/lib/codemirror.css'
import "codemirror/mode/javascript/javascript.js";
import 'codemirror/theme/material.css'

export default Vue.extend({
  components: {
    FileReader,
    Options,
    codemirror
  },
  created: async function(){
    this.prettyPrint(this.activeProject.json);
    this.setJson(this.activeProject.json);
  },
  data: function () {
    return {
      generatedFiles: [],
      activeProject: {id: -1, ownerId: -1, name: "", json: '{ "users": [{"userName": "Test User", "email": "aa@bb@cc"}], "tasks": [{"title": "Task Title", "description": "Task des"}] }'},
      cmOptions: {
        theme: 'material',
        tabSize: 2,
        mode: 'text/javascript',
        lineNumbers: true,
        line: true,
      },
      snackbar: {
        visible: false,
        type: "",
        icon: "mdi-alert-circle",
        text: "",
      },
    };
  },
  methods: {
    setJson: async function(json: string) {
      const generate = await this.$store.dispatch("generate", {data: json});
      this.generatedFiles = generate.generatedFiles;
      this.activeProject.json = json;
      if(this.$root.$data.user && this.activeProject.id >= 0){
        await this.$store.dispatch("updateProject", this.activeProject);
      }
    },
    prettyPrint: function(json: string){
      json = json.replace(/'/g, "\"");
      this.activeProject.json = JSON.stringify(JSON.parse(json),null,'\t');
    },
    selectProject: function(project: Project){
      let select = true
      if(this.activeProject.id === -1){
        select = confirm("changes will not be saved!");
      }
      if(select){
        this.activeProject = project;
        this.setJson(this.activeProject.json);
      }
    },
    existsProjectName: function(){
      for(const i in this.$store.state.projects){
        if(this.activeProject.name === this.$store.state.projects[i].name){
          return this.$store.state.projects[i];
        }
      }
      return null;
    },
    save: async function (){
      if(this.activeProject.name){
        const exists = this.existsProjectName();
        if(!exists && this.activeProject.id === -1){
          this.snackbar.type = "success",
          this.snackbar.text = "The new project was successfully created!"
          this.activeProject.id = 0;
          this.activeProject.ownerId = this.$root.$data.user.id;
          this.activeProject = await this.$store.dispatch("addProject", this.activeProject);
        }else if(exists && exists.id !== this.activeProject.id){
          this.snackbar.type = "error",
          this.snackbar.text = "This name is already in use, please enter another name!"
        }else{   
          this.snackbar.type = "success",
          this.snackbar.text = "Project updated successfully!"
          await this.$store.dispatch("updateProject", this.activeProject);
        }
      }else{
        this.snackbar.type = "error",
        this.snackbar.text = "This name is incorrect!"
      }
      this.snackbar.visible = true;
    },
    close: function (){
      console.log("close");
    }
  },
});
</script>

<style lang="css">
  .vue-codemirror{
    width: 100%;
    height: calc(100% - 41px);
  }
  .CodeMirror-scroll{
    margin: 0!important;
    overflow: auto!important;
  }
  .CodeMirror{
    height: 100%;z-index: 0;
  }
  .toolBar {
    position: absolute;
    top: -70px;
    left: 85px;
    width: calc(100% - 85px);
    z-index: 5;
  }

  .toolBar .v-input {
    max-width: 200px;
    min-width: 200px;
    padding-right: 10px;
    padding-left: 10px;
  }
  
</style>