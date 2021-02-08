<template>
  <v-dialog v-model="dialog">
    <template v-slot:activator="{ on, attrs }">
			<v-btn class="ml-2 mr-2" color="white" elevation="1" @click="init" fab small v-bind="attrs" v-on="on">
				<v-icon color="primary">mdi-file-multiple</v-icon>
			</v-btn>
    </template>
    <v-card>
      <v-card-title>
        <v-text-field v-model="search" append-icon="mdi-magnify" label="Search" single-line hide-details></v-text-field>
      </v-card-title>
      <v-data-table
        :headers="headers"
        :items="projects"
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
        { text: 'Owner', value: 'owner' },
      ],
      projects: Array<{id: number; name: string; owner: string}>(),
    };
  },
  methods: {
    init: async function(){
      this.projects = [];
      for(let i = 0; i < this.$store.state.projects.items.length; i++){
        const owner = await this.$store.dispatch("users/getUser", this.$store.state.projects.items[i].ownerId);
        this.projects.push({
          id: this.$store.state.projects.items[i].id,
          name: this.$store.state.projects.items[i].name,
          owner: owner.userName,
        });
      }
    },
		openProject: async function(project: {id: number; name: string; owner: string}){
			this.$emit("select-project", await this.$store.dispatch("projects/getProject", project.id));
			this.dialog = false;
		}
  },
});
</script>
