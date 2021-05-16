<template>
  <v-container class="pt-0 pb-0" fluid style="height:100vh">
    <v-row>
      <v-col cols="12" class="pt-0 pb-0">
        <v-data-table
          :headers="headers"
          :items="$store.state.projects.items"
          :search="search"
          class="projectsTable elevation-1"
          @click:row="openProject"
        >
          <template v-slot:top>
            <div class="d-flex align-end mx-4">
              <v-toolbar-title>Projects</v-toolbar-title>
                <v-progress-circular
                  v-if="inLoading"
                  indeterminate
                  :size="25"
                  color="primary"
                  class="ml-2"
                ></v-progress-circular>
              <v-spacer></v-spacer>
              <v-text-field class="mx-4" v-model="search" label="Search" append-icon="mdi-magnify" single-line hide-details></v-text-field>
            </div>
          </template>
          <template v-slot:[`item.actions`]="{ item }">
            <v-icon @click="deleteItem(item)">mdi-delete</v-icon>
          </template>
        </v-data-table>
        <v-dialog v-model="dialog" max-width="500">
          <v-card>
            <v-card-title>Are you sure you want to delete this project?</v-card-title>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" text @click="closeDelete">Cancel</v-btn>
              <v-btn color="blue darken-1" text @click="deleteItemConfirm">OK</v-btn>
              <v-spacer></v-spacer>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import { GenerateResponse } from '../models/GenerateResponse';
import { Project } from '../models/Project';
import api from '../api';

export default Vue.extend({
  data: function () {
    return {
      search: '',
      headers: [
        {
          text: 'Name',
          align: 'start',
          sortable: true,
          value: 'name',
        },
        { text: 'Actions', value: 'actions', sortable: false, align: 'end' },
      ],
      dialog: false,
      deletableProject: null as (null | Project),
      inLoading: false,
      projectInLoading: null as (null | Project),
    };
  },
  created: async function(){
    await this.$store.dispatch('projects/getProjects');
  },
  methods: {
    openProject: async function(project: Project){
      if(!this.dialog && !this.inLoading){
        this.inLoading = true;
        this.projectInLoading = project;
        const generateResult: GenerateResponse = await api.generate({
          data: project.json,
          nameSpace: this.toCamelCase(project.name),
          backend: project.backend,
          frontend: project.frontend
        });
        this.$store.commit('projects/setLastProject', project);
        this.$store.commit('projects/setLastGeneratedFiles', generateResult.generatedFiles);
        this.$router.push('/editor');
      }
    },
    deleteItem (project: Project) {
      this.deletableProject = project;
      this.dialog = true;
    },
    deleteItemConfirm () {
      this.$store.dispatch('projects/deleteProject', this.deletableProject);
      this.closeDelete();
    },
    closeDelete () {
      this.dialog = false;
    },
    toCamelCase: function(str: string) {
      const nameSpace = str.replace(/(?:^\w|[A-Z]|\b\w)/g, function(word, index) {
        return index === 0 ? word.toLowerCase() : word.toUpperCase();
      }).replace(/\s+/g, '');
      return `${nameSpace.charAt(0).toUpperCase()}${nameSpace.slice(1)}`;
    }
  }
});
</script>

<style scoped>
  .projectsTable >>> tbody tr{
    cursor: pointer;
  }
  .projectCard{
    cursor: pointer;
  }
</style>
