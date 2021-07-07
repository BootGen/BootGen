<template>
  <v-container fluid class="pa-0 ma-0">
    <v-row class="headBar d-flex">
      <v-col cols="12" class="pa-0 d-flex align-center justify-space-between flex-wrap">
        <div class="d-flex align-center">
          <v-toolbar-title class="font-weight-light mr-2">Editor -</v-toolbar-title>
          <ValidationObserver>
            <ValidationProvider v-slot="{ errors }" name="Project name" rules="required">
              <v-text-field v-model="projectName" placeholder="Name your project" type="text" :error-messages="errors" required @input="changeName" :disabled="commandStore.command(CommandType.OpenNewProjectDialog).disabled"></v-text-field>
            </ValidationProvider>
          </ValidationObserver>
          <v-btn class="mr-0 ml-3" color="primary" small @click="commandStore.do(CommandType.OpenNewProjectDialog)" v-if="$store.state.auth.jwt" :disabled="commandStore.command(CommandType.OpenNewProjectDialog).disabled">{{ commandStore.command(CommandType.OpenNewProjectDialog).text }}</v-btn>
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
    <project-settings v-if="viewModel.newProjectDialog" :modify="false" @save="newProject" @cancel="viewModel.newProjectDialog = false" title="Create new project" :backends="backends" :frontends="frontends"></project-settings>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import { required } from 'vee-validate/dist/rules';
import { extend, ValidationObserver, ValidationProvider } from 'vee-validate';
import ProjectSettings from '../components/ProjectSettings.vue';
import { ViewModel } from '../commands/ViewModel';
import { CommandStore, CommandType } from '../commands/CommandStore';

extend('required', {
  ...required,
  message: '{_field_} can not be empty',
});

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
    ProjectSettings,
  },
  props: {
    viewModel: {
      type: Object as () => ViewModel
    },
    commandStore: {
      type: Object as () => CommandStore
    },
    activeProjectName: String,
    backends: {
      type: Array as () => Array<string>
    },
    frontends: {
      type: Array as () => Array<string>
    },
  },
  data: function () {
    return {
      CommandType: CommandType,
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