<template>
 <v-container fluid class="pa-0 ma-0">
    <v-tooltip bottom v-for="(button, idx) in buttons" :key="idx">
      <template v-slot:activator="{ on, attrs }">
      <v-btn class="mr-1" color="white" elevation="1" fab small @click="$emit(button.onClick)" v-bind="attrs" v-on="on" :disabled="!button.visible">
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
      <span>{{ button.hoverText }}</span>
    </v-tooltip>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';

interface Button {
	icon: string;
	hoverText: string;
	visible: boolean;
  onClick: string;
  progress: boolean;
}

export default Vue.extend({
	props: {
    buttons: {
      type: Array as () => Button[]
    }        
	},
  methods: {
  },
});
</script>
