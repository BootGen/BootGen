<template>
 <v-container fluid>
    <v-snackbar v-model="snackbar.visible" :color="snackbar.type" :timeout="snackbar.timeout" bottom>
    <v-layout align-center justify-space-between>
      <div>
        <v-icon class="pr-3" dark large>{{ snackbar.icon }}</v-icon>
        <span>{{ snackbar.text }}</span>
        <div class="d-flex justify-space-around">
          <v-btn class="ml-3" v-for="(btn, idx) in snackbar.buttons" :key="idx" :color="btn.color" @click="$emit(btn.name)">{{ btn.name }}</v-btn>
        </div>
      </div>
      <v-btn v-if="snackbar.dismissible" icon @click="closeSnackbar()">
        <v-icon>mdi-close-thick</v-icon>
      </v-btn>
    </v-layout>
  </v-snackbar>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";

interface Snackbar {
	dismissible: boolean;
	visible: boolean;
	type: string;
	icon: string;
	text: string;
  buttons: Array<{name: string; color: string}>;
}

export default Vue.extend({
	props: {
		snackbar: {
			type: Object as () => Snackbar
		}
	},
  methods: {
    closeSnackbar: function(){
      this.$gtag.event('close-snackbar');
      this.snackbar.visible = false;
    }
  },
});
</script>
