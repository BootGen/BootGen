<template>
  <v-dialog v-model="dialog">
    <v-card v-click-outside="close">
      <v-toolbar flat color="primary" dark>
				<v-btn icon dark @click="close">
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
      dialog: true,
      help: [
        {title: "Title 1", icon: "mdi-account", description: "Description 1"}, 
        {title: "Title 2", icon: "mdi-clock", description: "Description 2"}, 
        {title: "Title 3", icon: "mdi-cog", description: "Description 3"}, 
      ],
    };
  },
  methods: {
    close: function(){
      this.dialog = false;
      this.$emit("close-help");
    }
  },
});
</script>
