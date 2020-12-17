<template>
  <v-dialog v-model="dialog">
    <template v-slot:activator="{ on, attrs }">
			<v-btn class="ml-2" color="white" elevation="1" fab small v-bind="attrs" v-on="on">
				<v-icon color="primary">mdi-help</v-icon>
			</v-btn>
    </template>
    <v-card>
      <v-toolbar flat color="primary" dark>
				<v-btn icon dark @click="dialog = false">
					<v-icon>mdi-close</v-icon>
				</v-btn>
        <v-toolbar-title>Help</v-toolbar-title>
      </v-toolbar>
      <v-tabs>
        <v-tab v-for="item in help" :key="item.title">
          <v-icon left>
            {{item.icon}}
          </v-icon>
          {{item.title}}
        </v-tab>
        <v-tab-item v-for="item in help" :key="item.title">
          <v-card flat>
            <v-card-text>
              <p>
               {{item.description}}
              </p>
            </v-card-text>
          </v-card>
        </v-tab-item>
      </v-tabs>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from "vue";

export default Vue.extend({
  components: {
  },
  data: function () {
    return {
      dialog: false,
      help: [
        {title: "Title 1", icon: "mdi-account", description: "Description 1"}, 
        {title: "Title 2", icon: "mdi-clock", description: "Description 2"}, 
        {title: "Title 3", icon: "mdi-cog", description: "Description 3"}, 
      ],
    };
  },
  methods: {
  },
});
</script>
