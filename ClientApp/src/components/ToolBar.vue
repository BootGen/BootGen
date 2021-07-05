<template>
 <v-container fluid class="d-flex pa-0 ma-0">
   <div v-for="(button, idx) in buttons" :key="idx">
      <v-tooltip bottom v-if="button.isCircleShape">
        <template v-slot:activator="{ on, attrs }">
        <v-btn class="mr-1" :color="button.color ? button.color : 'white'" elevation="1" fab small @click="button.action()" v-bind="attrs" v-on="on" :disabled="button.disabled">
          <v-icon v-if="!button.progress" color="primary">{{ button.icon }}</v-icon>
          <div v-if="button.progress">
            <v-progress-circular
              indeterminate
              :size="25"
              color="primary"
            ></v-progress-circular>
          </div>
        </v-btn>
        </template>
        <span>{{ button.text }}</span>
      </v-tooltip>
      <v-btn v-else class="mr-0 ml-3" :color="button.color ? button.color : 'primary'" small @click="button.action()" :disabled="button.disabled">
        <v-icon>{{ button.icon }}</v-icon>
        {{ button.text }}
      </v-btn>
    </div>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import { Command } from '../commands/Command';

export default Vue.extend({
	props: {
    buttons: {
      type: Array as () => Command[]
    }        
	},
  methods: {
  },
});
</script>
