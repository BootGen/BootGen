<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12" class="pa-0">
        <div class="d-flex align-center flex-wrap" v-if="$store.state.auth.jwt">
          <v-toolbar-title class="font-weight-light mr-2">Editor -</v-toolbar-title>
          <v-text-field v-model="activeProject.name" placeholder="Name your project" type="text" required @input="changeName"></v-text-field>
          <v-btn class="mr-0 ml-3" color="primary" small @click="newProject"><v-icon>mdi-plus</v-icon></v-btn>
        </div>
        <div class="d-flex align-center justify-space-between" v-else>
          <v-toolbar-title class="font-weight-light mr-2">Editor</v-toolbar-title>
          <div class="mr-1">for save <router-link to="/login">sign in</router-link></div>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { Project } from "../models/Project";

export default Vue.extend({
  props: {
    activeProject: {
      type: Object as () => Project
    },
  },
  data: function () {
    return {
    };
  },
  methods: {
    newProject: function(){
      this.$emit('new-project');
    },
    changeName: function(name: string){
      this.$emit('change-project-name', name);
    },
  }
});
</script>