<template>
  <v-container fluid class="d-flex pa-0 ma-0">
    <v-tooltip v-for="(buttonType, idx) in buttonTypes" :key="idx" bottom>
      {{buttonType}}-{{idx}}
      <template v-slot:activator="{ on, attrs }">
        <v-btn class="mr-1" color="white" elevation="1" fab small @click="commandStore.do(buttonType)" v-bind="attrs" v-on="on" :disabled="commandStore.command(buttonType).disabled">
          <v-icon v-if="!commandStore.command(buttonType).progress" color="primary">{{ commandStore.command(buttonType).icon }}</v-icon>
          <div v-if="commandStore.command(buttonType).progress">
            <v-progress-circular
              indeterminate
              :size="25"
              color="primary"
            ></v-progress-circular>
          </div>
        </v-btn>
      </template>
      <span>{{ commandStore.command(buttonType).text }}</span>
    </v-tooltip>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import { CommandStore, CommandType } from '../commands/CommandStore';

export default Vue.extend({
	props: {
    buttonTypes: {
      type: Array as () => CommandType[]
    },
    commandStore: CommandStore
	},
  methods: {
  },
});
</script>
