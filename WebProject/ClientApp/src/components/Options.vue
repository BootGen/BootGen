<template>
  <v-container fluid>
		<v-row v-if="projects.length < 1" class="d-flex justify-end">
			<v-btn v-if="this.$store.state.jwt" class="mr-4" color="primary" small @click="save">Save</v-btn>
			<p v-else class="mr-4">for save <a href="/">sign in</a></p>
			<v-btn class="mr-4" color="primary" small @click="download">Download</v-btn>
    </v-row>
		<v-row v-else class="d-flex align-center">
			<v-select class="mr-4" label="My Projects" @change="selectProject" :items="projects" item-text="name" return-object></v-select>
			<v-btn v-if="this.$store.state.jwt" class="mr-4" color="primary" small @click="save">Save</v-btn>
			<p v-else class="mr-4">for save <a href="/">sign in</a></p>
			<v-btn class="mr-4" color="primary" small @click="download">Download</v-btn>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { Project } from "../models/Project";
import { User } from "../models/User";

export default Vue.extend({
	props:[
		"project"
	],
	created: async function() {
		if(this.$store.state.jwt){
			const user = await this.$store.dispatch("profile");
			this.projects = await this.$store.dispatch("getProjectsOfUser", user);
		}
	},
	data: function () {
    return {
      projects: new Array<Project>(),
    };
  },
  methods: {
    selectProject: function(project: Project) {
			this.$emit('select-project', project);
    },
    download: function() {
      console.log("download:", this.project);
    },
    save: function() {
      console.log("save:", this.project);
    }
	},
});
</script>
