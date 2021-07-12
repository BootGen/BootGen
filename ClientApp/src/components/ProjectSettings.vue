<template>
  <v-container fluid>
    <v-dialog v-model="dialog" width="400" persistent>
      <v-card class="pa-0 ma-0">
        <v-card-title>
          <span class="headline">{{ title }}</span>
        </v-card-title>
        <ValidationObserver v-slot="{ invalid }">
          <v-card-text class="pb-0">
            <ValidationProvider v-slot="{ errors }" name="Project name" rules="required">
              <v-text-field v-model="projectName" placeholder="Name your project" type="text" :error-messages="errors" required></v-text-field>
            </ValidationProvider>
            <v-select
              v-model="backend"
              :items="backends"
              label="Backend"              
            ></v-select>
            <v-select
              v-model="frontend"
              :items="frontends"
              label="Frontend"             
            ></v-select>
            <v-alert class="text-left" type="error" v-if="errorMsg">This name is already in use, please enter another name!</v-alert>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn class="mr-0 ml-3" color="primary" @click="cancel">Cancel</v-btn>
            <v-btn v-if="modify" class="mr-0 ml-3" color="primary" @click="save" :disabled="invalid">Ok</v-btn>
            <v-btn v-else class="mr-0 ml-3" color="primary" @click="save" :disabled="invalid">Create</v-btn>
          </v-card-actions>
        </ValidationObserver>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import { ValidationObserver, ValidationProvider } from 'vee-validate';
import { Project } from '../models/Project';

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
  },
  props: {
    modify: Boolean,
    title: String,
    activeProject: Object as () => Project,
    backends: {
      type: Array as () => Array<string>
    },
    frontends: {
      type: Array as () => Array<string>
    },
  },
  data: function () {
    return {
      errorMsg: false,
      dialog: true,
      projectName: 'My Project',
      backend: 'ASP.NET',
      frontend: 'Vue 2 + JS',
    };
  },
  created: function(){
    if(this.activeProject){
      this.projectName = this.activeProject.name;
      this.backend = this.activeProject.backend;
      this.frontend = this.activeProject.frontend;
    }
  },
  methods: {
    save: function(){
      if(this.modify){
        this.$emit('save', this.backend, this.frontend, this.projectName);
        this.dialog = false;
      }else if(this.existsProjectName()){
        this.errorMsg = true;
      }else{
        this.$emit('save', this.backend, this.frontend, this.projectName);
        this.dialog = false;
      }
    },
    cancel: function(){
      this.dialog = false;
      this.$emit('cancel');
    },
    existsProjectName: function(): boolean{
      for(const i in this.$store.state.projects.items){
        if(this.projectName === this.$store.state.projects.items[i].name){
          return true;
        }
      }
      return false;
    },
  }
});
</script>