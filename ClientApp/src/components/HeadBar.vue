<template>
  <v-container fluid class="pa-0 ma-0">
    <v-row class="headBar d-flex">
      <v-col cols="12" class="pa-0 d-flex align-center justify-space-between flex-wrap">
        <div class="d-flex align-center">
          <v-toolbar-title class="font-weight-light mr-2">Editor -</v-toolbar-title>
          <ValidationObserver>
            <ValidationProvider v-slot="{ errors }" name="Project name" rules="required">
              <v-text-field v-model="projectName" placeholder="Name your project" type="text" :error-messages="errors" required @input="changeName" :disabled="disabled"></v-text-field>
            </ValidationProvider>
          </ValidationObserver>
          <tool-bar v-if="$store.state.auth.jwt" :buttons="[newProjectCommand]"></tool-bar>
        </div>
        <div class="d-flex align-center mr-5">
          <span class="mr-5" v-if="!$store.state.auth.jwt">for save<router-link class="pl-2" to="/login" @click="toLogin()">sign in</router-link></span>
          <v-switch
            v-model="darkTheme"
            label="Dark Mode"
          ></v-switch>
        </div>
      </v-col>
    </v-row>
    <project-settings v-if="viewModel.newProjectDialog" @save="newProject" @cancel="viewModel.newProjectDialog = false" title="Create new project" :backends="backends" :frontends="frontends"></project-settings>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import { required } from 'vee-validate/dist/rules';
import { extend, ValidationObserver, ValidationProvider } from 'vee-validate';
import ProjectSettings from '../components/ProjectSettings.vue';
import ToolBar from '../components/ToolBar.vue';
import { ViewModel } from '../utils/ViewModel';
import { NewProjectCommand } from '../utils/Command';

extend('required', {
  ...required,
  message: '{_field_} can not be empty',
});

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
    ProjectSettings,
    ToolBar,
  },
  props: {
    viewModel: {
      type: Object as () => ViewModel
    },
    activeProjectName: String,
    backends: {
      type: Array as () => Array<string>
    },
    frontends: {
      type: Array as () => Array<string>
    },
    disabled: Boolean
  },
  data: function () {
    return {
      newProjectCommand: new NewProjectCommand(this.viewModel),
      errorMsg: false,
      projectName: 'My Project',
      darkTheme: false,
    };
  },
  mounted(){
    if(localStorage.darkTheme === 'true'){
      this.darkTheme = true;
    }else{
      this.darkTheme = false;
    }
  },
  watch:{
    darkTheme(dark){
      this.$gtag?.event('change-theme');
      localStorage.darkTheme = dark;
      this.$vuetify.theme.dark = dark;
    },
    activeProjectName(name){
      this.projectName = name;
    }
  },
  methods: {
    newProject: function(backend: string, frontend: string, name: string){
      this.$emit('new-project', name, backend, frontend);
      this.viewModel.newProjectDialog = false;
    },
    changeName: function(name: string){
      this.$emit('change-project-name', name);
    },
    toLogin: function(){
      this.$gtag?.event('editor-to-login');
    },
  }
});
</script>

<style lang="css">
  .headBar{
    margin-left: 0px;
    margin-top: 32px;
  }
    .headBar {
      margin-left: 70px;
    }
  @media screen and (max-width: 960px) {
    .headBar {
      margin-left: 85px;
    }
  }
  @media screen and (max-width: 600px) {
    .headBar {
      margin: unset;
      margin-top: 80px;
      padding: 10px;
    }
  }
</style>