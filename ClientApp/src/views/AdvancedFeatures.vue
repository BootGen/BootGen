<template>
  <v-container fluid>
    <v-card>
      <v-img
        max-width="600"
        src="@/assets/img/bootgen_full.png"
        alt="BootGen logo"
      ></v-img>
      <h1 class="slogan">
        Advanced Features
      </h1>
      <p>Although comments are non-standard feature in JSON, many JSON processing libraries support it. We use them as annotations. Annotations can be placed at the beginning of arrays. Possible annotations:</p>
      <ul>
        <li><span class="code">timestamps</span> Adds a <span class="code">Created</span> and an <span class="code">Updated</span> timestamp property to the class.</li>
        <li><span class="code">manyToMany</span>: Declears that the given relation is a Many-To-Many relation, as opposed to the default One-To-Many relation.</li>
      </ul>
      <h2>Advanced Example: Task Management System</h2>
      <codemirror :value="example" :options="cmOptions" />
      <h2>Known Limitations</h2>
      <p>There are many limitations of this project, simply because for many things there is no single accepted way to code it. We intend to generate those parts of your project that are fairly standard. If there is multiple accepted ways to implement a certain feature, or it is too complex to generate, then we won't generate it. Please treat the generated code as a basis of your work, and not something that you can hand over to a client out of the box.</p>
      <p>There are however limitations that we do plan to overcome in the future. These are:</p>
      <ul>
        <li> It is currently not possible to create self referencing Many-To-Many connections. </li>
      </ul>
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
      example: ''
    };
  },
  created: async function () {
    this.example = (
      await axios.get(`${this.$root.$data.baseUrl}/tasks.json`, {
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
h2 {
  margin-top: 30px;
  margin-bottom: 30px;
}
.theme--dark .code {
  font-family: "Inconsolata";
  color: #9CDCFE;
}
.theme--light .code {
  font-family: "Inconsolata";
  color: #0451A5;
}

@media screen and (max-width: 576px) {
  .v-image {
    margin-bottom: 50px;
  }
}
</style>
