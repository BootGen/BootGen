<template>
  <v-container fluid>
    <v-row>
      <v-col cols="8" v-if="$root.$data.user">
        <h2>Goodbye!</h2>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
export default Vue.extend({
  data: function () {
    return {};
  },
  created: function() {
    this.$store.commit("setJwt", null);
  }
});
</script>