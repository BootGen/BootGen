<template>
  <v-container fluid>
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
      activeProject: {id: -1, name: "New Project", json: '{ "users": [{"userName": "Test User", "email": "aa@bb@cc"}], "tasks": [{"title": "Task Title", "description": "Task des"}] }'},
      cmOptions: {
        theme: 'material',
        tabSize: 2,
        mode: 'text/javascript',
        lineNumbers: true,
        line: true,
      }
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
</style>