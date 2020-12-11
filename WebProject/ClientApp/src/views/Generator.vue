<template>
  <v-container fluid>
    <options :project="activeProject" @select-project="selectProject"></options>
    <v-row>
      <v-col class="d-flex pa-0">
        <v-btn class="mr-4" color="primary" :disabled="activeProject.json == ''" @click="prettyPrint(activeProject.json)">Pretty Print</v-btn>
        <v-btn class="mr-4" color="primary" :disabled="activeProject.json == ''" @click="setJson(activeProject.json)">Generation</v-btn>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12" md="4" class="pr-0 pl-0">
        <base-material-generator-card>
          <template v-slot:heading>
            <div class="display-1 font-weight-light pa-2">
              JSON
            </div>
          </template>
          <codemirror v-model="activeProject.json" :options="cmOptions" />
        </base-material-generator-card>
      </v-col>
      <v-col cols="12" md="8" class="pr-0 pl-0 pt-0">
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

// import base style
import 'codemirror/lib/codemirror.css'
import "codemirror/mode/javascript/javascript.js";

export default Vue.extend({
  components: {
    FileReader,
    Options,
    codemirror
  },
  created: async function(){
    this.setJson(this.activeProject.json);
  },
  data: function () {
    return {
      generatedFiles: [],
      activeProject: {id: -1, name: "New Project", json: '{ "users": [{"userName": "Test User", "email": "aa@bb@cc"}], "tasks": [{"title": "Task Title", "description": "Task des"}] }'},
      cmOptions: {
        tabSize: 4,
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