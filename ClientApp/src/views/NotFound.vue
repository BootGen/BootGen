<template>
  <v-container fluid>
    <v-row>
      <v-col cols="12" class="d-flex justify-center">
        <div class="d-flex flex-column text-center align-center">
          <div class="d-flex">
            <h1 class="errorCode">4</h1>
            <v-icon color="#00A170" size="9rem">mdi-emoticon-dead-outline </v-icon>
            <h1 class="errorCode">4</h1>
          </div>
          <h2 class="response">Page not found</h2>
          <h3>The page you are looking for was moved, removed, renamed or might never existed.</h3>
          <v-btn class="ma-0 mt-5" color="primary" rounded to="/">Go to home</v-btn>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';

export default Vue.extend({
  data: function () {
    return {
    };
  },
  methods: {
  },
});
</script>

<style scoped>
  h1.errorCode{
    font-size: 10rem;
    color: #00A170;
    line-height: 1;
    text-shadow: 3px 3px 0px #eee, 6px 6px 0px #656867;
  }
  .response{
    font-size: 3.75rem;
    color: #656867;
  }
</style>