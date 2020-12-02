<template>
  <v-container fluid>
    <v-row class="d-flex justify-end">
      <v-btn class="mr-4" color="primary" small @click="download">download</v-btn>
    </v-row>
    <v-row>
      <v-col cols="5">
        <v-textarea 
          v-model="jsonText"
          placeholder="json"
          rows="10"
          required
        ></v-textarea>
        <v-btn class="mr-4" large :disabled="jsonText == ''" @click="setJson(jsonText)">Generation</v-btn>
      </v-col>
      <v-col cols="7">
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
    this.setJson(JSON.stringify(this.json));
  },
  data: function () {
    return {
      generatedFiles: [],
      json: {
        users: [{'userName': 'Test User', 'email': 'aa@bb@cc'}],
        tasks: [{'title': 'Task Title', 'description': 'Task des'}]
      },
      jsonText: "",
    };
  },
  methods: {
    setJson: async function(json: string) {
      const generate = await this.$store.dispatch("generate", {data: json});
      this.generatedFiles = generate.generatedFiles;
      this.jsonText = json;
    },
    download: function() {
      console.log("download");
    }
  },
});
</script>
