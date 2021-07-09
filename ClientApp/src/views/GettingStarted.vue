<template>
  <v-container fluid>
    <v-card>
      <v-img
        max-width="600"
        src="@/assets/img/bootgen_full.png"
        alt="BootGen logo"
      ></v-img>
      <h1 class="slogan">
        Getting Started
      </h1>
      <h2 class="slogan">
        Online project generator for ASP.Net 5 with Vue.js
      </h2>
      <h3>
        BootGen generates an ASP.Net 5 project with Vue.js based on the sample JSON data you provide. From this sample data it infers the types you need, and creates entity classes, data services, controllers and Vuex state management that fits your use case.
      </h3>
      <div class="slogan">
        <v-btn class="ma-2" to="/editor" color="primary"> Try Online </v-btn>
      </div>
      <h2>An Example JSON Input</h2>
      <codemirror :value="example" :options="cmOptions" />
      <h2>Conventions</h2>
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
      <h2>Hinting</h2>

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
h2 {
  margin-top: 30px;
  margin-bottom: 30px;
}

@media screen and (max-width: 576px) {
  .v-image {
    margin-bottom: 50px;
  }
}
</style>
