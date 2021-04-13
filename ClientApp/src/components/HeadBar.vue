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
        <v-btn class="mr-0 ml-3" color="primary" small @click="newProject" v-if="$store.state.auth.jwt"><v-icon>mdi-plus</v-icon></v-btn>
        <v-spacer v-if="!$store.state.auth.jwt"></v-spacer>
        <div class="d-flex align-center" @click="toLogin()" v-if="!$store.state.auth.jwt"><span class="pr-1">for save</span><router-link to="/login">sign in</router-link></div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { Project } from "../models/Project";
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
    };
  },
  methods: {
    newProject: function(){
      this.$emit('new-project');
    },
    changeName: function(name: string){
      this.$emit('change-project-name', name);
    },
    toLogin: function(){
      this.$gtag.event('editor-to-login');
    }
  }
});
</script>