<template>
  <v-container class="pt-0 pb-0" fluid style="height:100vh">
    <v-row>
      <v-col cols="12" class="pt-0 pb-0">
        <v-card>
          <v-toolbar flat color="primary" dark>
            <v-toolbar-title>Getting Started</v-toolbar-title>
          </v-toolbar>
          <v-tabs>
            <v-tab v-for="item in help" :key="item.title" @click="changeHelpMenu">
              <v-icon left>
                {{ item.icon }}
              </v-icon>
              {{ item.title }}
            </v-tab>
            <v-tab-item v-for="item in help" :key="item.title">
              <v-card flat>
                <v-card-text>
                  <p>
                  {{ item.description }}
                  </p>
                </v-card-text>
              </v-card>
            </v-tab-item>
          </v-tabs>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';

export default Vue.extend({
  data: function () {
    return {
      help: [
        {title: 'Title 1', icon: 'mdi-account', description: 'Description 1'}, 
        {title: 'Title 2', icon: 'mdi-clock', description: 'Description 2'}, 
        {title: 'Title 3', icon: 'mdi-cog', description: 'Description 3'}, 
      ],
    };
  },
  methods: {
    changeHelpMenu: function(){
      this.$gtag.event('change-help-menu');
    }
  }
});
</script>
