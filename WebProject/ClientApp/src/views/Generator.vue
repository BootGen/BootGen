<template>
  <v-container fluid>
    <options :project="activeProject" @select-project="selectProject"></options>
    <v-row>
      <v-col cols="5">
        <v-textarea 
          v-model="activeProject.json"
          placeholder="json"
          rows="10"
          required
        ></v-textarea>
        <v-btn class="mr-4" large :disabled="activeProject.json == ''" @click="setJson(activeProject.json)">Generation</v-btn>
      </v-col>
      <v-col cols="7">
        <file-reader :files="generatedFiles"></file-reader>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import FileReader from "@/components/FileReader.vue";
import Options from "@/components/Options.vue";
import { Project } from "../models/Project";

export default Vue.extend({
  components: {
    FileReader,
    Options,
  },
  created: async function(){
    this.setJson(this.activeProject.json);
  },
  data: function () {
    return {
      generatedFiles: [],
      activeProject: {name: "New Project", json: "{ users: [{'userName': 'Test User', 'email': 'aa@bb@cc'}], tasks: [{'title': 'Task Title', 'description': 'Task des'}] }"},
    };
  },
  methods: {
    setJson: async function(json: string) {
      const generate = await this.$store.dispatch("generate", {data: json});
      this.generatedFiles = generate.generatedFiles;
      this.activeProject.json = json;
    },
    selectProject: function(project: Project){
      //TODO: save changes
      this.activeProject = project;
      this.setJson(this.activeProject.json);
    }
  },
});
</script>
