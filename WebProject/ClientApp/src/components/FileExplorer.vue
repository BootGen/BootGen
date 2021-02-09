<template>
  <v-dialog v-model="dialog">
    <template v-slot:activator="{ on, attrs }">
			<v-btn class="ml-2 mr-2" color="white" elevation="1" fab small v-bind="attrs" v-on="on">
				<v-icon color="primary">mdi-file-multiple</v-icon>
			</v-btn>
    </template>
    <v-card>
      <v-card-title>
        <v-text-field v-model="search" append-icon="mdi-magnify" label="Search" single-line hide-details></v-text-field>
      </v-card-title>
      <v-data-table
        :headers="headers"
        :items="$store.state.projects.items"
        :search="search"
				sort-by="name"
				@click:row="openProject"
      >
			</v-data-table>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from "vue";
import { Project } from "../models/Project";

export default Vue.extend({
  data: function () {
    return {
			dialog: false,
			search: '',
      headers: [
        {
          text: 'Name',
          align: 'start',
          sortable: true,
          value: 'name',
        },
      ],
    };
  },
  methods: {
		openProject: async function(project: {id: number; name: string; owner: string}){
			this.$emit("select-project", await this.$store.dispatch("projects/getProject", project.id));
			this.dialog = false;
		}
  },
});
</script>
