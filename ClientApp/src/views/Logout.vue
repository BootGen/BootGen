<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12" class="d-flex justify-center">
        <div class="d-flex flex-column text-center align-center">
          <div class="d-flex flex-column align-center">
            <v-icon color="primary" size="70">mdi-check-circle-outline</v-icon>
            <p class="display-3">Logout was <span class="primary--text">successful.</span></p>
          </div>
          <h2 class="response">Goodbye!</h2>
          <div class="d-flex">
            <v-btn class="ma-2 mt-5" color="primary" rounded to="/login">Login</v-btn>
            <v-btn class="ma-2 mt-5" color="primary" rounded to="/sign-up">Sign up</v-btn>
          </div>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
export default Vue.extend({
  data: function () {
    return {};
  },
  created: function() {
    this.$store.commit('setJwt', null);
  }
});
</script>
