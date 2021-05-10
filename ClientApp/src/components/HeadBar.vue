<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12" class="pa-0 d-flex align-center">
        <v-toolbar-title class="font-weight-light mr-2">Editor -</v-toolbar-title>
        <ValidationObserver>
          <ValidationProvider v-slot="{ errors }" name="Project name" rules="required">
            <v-text-field v-model="activeProject.name" placeholder="Name your project" type="text" :error-messages="errors" required @input="changeName"></v-text-field>
          </ValidationProvider>
        </ValidationObserver>
        <v-btn class="mr-0 ml-3" color="primary" small @click="dialog = true" v-if="$store.state.auth.jwt">New project</v-btn>
        <v-spacer v-if="!$store.state.auth.jwt"></v-spacer>
        <div class="d-flex align-center" @click="toLogin()" v-if="!$store.state.auth.jwt"><span class="pr-1">for save</span><router-link to="/login">sign in</router-link></div>
      </v-col>
    </v-row>
    <v-dialog v-model="dialog" width="400" persistent>
      <v-card class="pa-0 ma-0">
        <v-card-title>
          <span class="headline">Create new project</span>
        </v-card-title>
        <ValidationObserver v-slot="{ invalid }">
          <v-card-text class="pb-0">
            <ValidationProvider v-slot="{ errors }" name="Project name" rules="required">
              <v-text-field v-model="projectName" placeholder="Name your project" type="text" :error-messages="errors" required></v-text-field>
            </ValidationProvider>
            <v-alert class="text-left" type="error" v-if="errorMsg">This name is already in use, please enter another name!</v-alert>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn class="mr-0 ml-3" color="primary" @click="dialog = false">Cancel</v-btn>
            <v-btn class="mr-0 ml-3" color="primary" @click="newProject" :disabled="invalid">Save</v-btn>
          </v-card-actions>
        </ValidationObserver>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import { Project } from '../models/Project';
import { required } from 'vee-validate/dist/rules';
import { extend, ValidationObserver, ValidationProvider } from 'vee-validate';

extend('required', {
  ...required,
  message: '{_field_} can not be empty',
});

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
  },
  props: {
    activeProject: {
      type: Object as () => Project
    },
  },
  data: function () {
    return {
      errorMsg: false,
      dialog: false,
      projectName: 'My Project'
    };
  },
  methods: {
    newProject: function(){
      if(this.existsProjectName()){
        this.errorMsg = true;
      }else{
        this.$emit('new-project', this.projectName);
        this.dialog = false;
      }
    },
    changeName: function(name: string){
      this.$emit('change-project-name', name);
    },
    toLogin: function(){
      this.$gtag?.event('editor-to-login');
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
<style scoped>

.theme--dark a{
  color: #8b949e!important;
}
</style>