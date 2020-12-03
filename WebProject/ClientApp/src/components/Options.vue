<template>
  <v-container fluid>
		<v-row v-if="$store.state.projects.length < 1" class="d-flex justify-end">
			<p class="mr-4">for save <a href="/">sign in</a></p>
			<v-btn class="mr-4" color="primary" small @click="download">Download</v-btn>
    </v-row>
		<v-row v-else class="d-flex align-center">
			<v-select class="mr-4" label="My Projects" @change="selectProject" :items="$store.state.projects" item-text="name" return-object></v-select>
			<save-project-dialog :project="project"></save-project-dialog>
			<v-btn class="mr-4" color="primary" small @click="download">Download</v-btn>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { Project } from "../models/Project";
import { User } from "../models/User";
import SaveProjectDialog from "@/components/SaveProjectDialog.vue";

export default Vue.extend({
	props:[
		"project"
	],
  components: {
    SaveProjectDialog,
  },
	created: async function() {
		if(this.$store.state.jwt){
			const user = await this.$store.dispatch("profile");
			await this.$store.dispatch("getProjectsOfUser", user);
		}
	},
	data: function () {
    return {
    };
  },
  methods: {
    selectProject: function(project: Project) {
			this.$emit('select-project', project);
    },
    download: function() {
      console.log("download:", this.project);
    },
	},
});
</script>
