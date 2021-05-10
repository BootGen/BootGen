<template>
  <v-container fluid>
    <v-card>
      <v-img
        width="600"
        height="90"
        src="@/assets/img/bootgen_full.png"
        alt="BootGen logo"
      ></v-img>
      <h1 class="slogan">
        Rapidly prototype yor next ASP.Net 5 - Vue.js application!
      </h1>
      <h2 class="slogan">
        Turns JSON sample data into working web application with best practices
        applied.
      </h2>
      <div class="slogan">
        <v-btn to="/editor" color="primary"> Try Online </v-btn>
      </div>
      <h2>Getting Started</h2>
      <p>To start using BootGen all you need to do, is to create a sample dataset for your next application in JSON. You do not need to install anything, and you do not need to register. Just go to the editor, and enjoy the head start for your project!</p>

      <h3>An Example JSON Input</h3>
      <codemirror :value="example" :options="cmOptions" />
      <h3>Conventions</h3>
      <ul>
        <li>
          Property and class names should be camelCase. Casing will be set in
          the generated code according to the type of file generated. In C#
          PascalCase will be used, in TypeScript property names will remain
          camelCase.
        </li>
        <li>
          Array names should be plural nouns, everything else should be in
          singular form. In the generated code the fitting plural or singular
          form of names will be used.
        </li>
      </ul>
      <h3>Hinting</h3>

      You can give hints in the form of comments for the generator. Hints can be
      placed at the beginning of arrays. Possible hints:
      <ul>
        <li>
          <span class="code">timestamps</span>: Adds a <span class="code">Created</span> and an <span class="code">Updated</span> timestamp property to
          the class.
        </li>
        <li>
          <span class="code">manyToMany</span>: Declears that the given relation is a Many-To-Many
          relation, as opposed to the default One-To-Many relation.
        </li>
        <li>
          <span class="code">class:[name]</span>: Substitute <span class="code">[name]</span> with the intended name of the
          class. Example:
        </li>
      </ul>
      <codemirror :value="friends" :options="cmOptions" />
    </v-card>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';

import { codemirror } from 'vue-codemirror';
import 'codemirror/theme/material.css';
import 'codemirror/lib/codemirror.css';
import 'codemirror/mode/javascript/javascript.js';
import axios from 'axios';
export default Vue.extend({
  components: {
    codemirror,
  },
  data: function () {
    return {
      cmOptions: {
        theme: 'material',
        tabSize: 2,
        mode: 'text/javascript',
        lineNumbers: false,
        line: true,
        readOnly: true,
        scrollbarStyle: null
      },
      example: '',
      friends: ''
    };
  },
  created: async function () {
    this.example = (
      await axios.get(`${this.$root.$data.baseUrl}/example_input.json`, {
        responseType: 'text',
      })
    ).data;
    this.friends = (
      await axios.get(`${this.$root.$data.baseUrl}/friends.json`, {
        responseType: 'text',
      })
    ).data;
  },
});
</script>

<style scoped>
.v-image {
  margin-left: auto;
  margin-right: auto;
  margin-bottom: 100px;
}
.v-card {
  padding-top: 50px;
  padding-bottom: 50px;
  padding-left: 12%;
  padding-right: 12%;
}
.slogan {
  text-align: center;
  margin: 10px;
}
.vue-codemirror {
  margin-top: 30px;
  margin-bottom: 30px;
  padding-bottom: 0 !important;
}
.CodeMirror {
  height: auto;
}
.theme--light .code{
  font-family: 'Inconsolata';
  background-color: #eee;
}
.theme--dark .code{
  font-family: 'Inconsolata';
  background-color: #666;
}
h3, h2 {
  margin-top: 30px;
  margin-bottom: 30px;
}
</style>