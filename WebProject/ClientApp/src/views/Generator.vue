<template>
  <v-container fluid>
    <v-row>
      <v-col cols="6">
      </v-col>
      <v-col cols="6">
        <file-reader :files="generatedFiles"></file-reader>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import FileReader from "@/components/FileReader.vue";

export default Vue.extend({
  components: {
    FileReader
  },
  created: async function(){
    const generate = await this.$store.dispatch("generate", {data: JSON.stringify(this.json)});
    this.generatedFiles = generate.generatedFiles;
  },
  data: function () {
    return {
      generatedFiles: [],
      json: {
        users: [{'userName': 'Test User', 'email': 'aa@bb@cc'}],
        tasks: [{'title': 'Task Title', 'description': 'Task des'}]
      }
    };
  },
  methods: {
    onError() {
      console.log('error');
    }
  },
});
</script>