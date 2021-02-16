<template>
 <v-container fluid>
    <v-snackbar v-model="snackbar.visible" :color="snackbar.type" timeout="5000" bottom>
    <v-layout align-center justify-space-between>
      <div class="d-flex align-center">
        <v-icon class="pr-3" dark large>{{ snackbar.icon }}</v-icon>
      <div>{{ snackbar.text }}</div>
      </div>
      <v-btn v-if="snackbar.dismissable" icon @click="snackbar.visible = false">
        <v-icon>mdi-close-thick</v-icon>
      </v-btn>
    </v-layout>
  </v-snackbar>
  </v-container>
</template>
<script lang="ts">
interface Snackbar {
	dismissable: boolean;
	visible: boolean;
	type: string;
	icon: string;
	text: string;
}
export default {
	props: {
		snackbar: {
			type: Object as () => Snackbar
		}
	},
}
</script>
