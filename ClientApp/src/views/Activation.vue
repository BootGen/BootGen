<template>
  <v-container id="privacy-statement" fluid tag="section">
    <v-row justify="center">
      <v-col cols="12" md="8">
        <base-material-card>
          <template v-slot:heading>
            <div class="display-2 font-weight-light">
              Activation
            </div>
          </template>
          <div v-if="waitForResponse" class="d-flex flex-column align-center">
            <v-progress-circular
              indeterminate  
              :size="70" 
              color="primary"
            ></v-progress-circular>
            <p class="display-3">Please wait for the result!</p>
          </div>
          <div v-else-if="activeted" class="d-flex flex-column align-center">
            <v-icon color="green" size="70">mdi-check-circle-outline</v-icon>
            <p class="display-3">Activation was <span class="green--text">successful.</span></p>
          </div>
          <div v-else class="d-flex flex-column align-center">
            <v-icon color="red" size="70">mdi-close-circle-outline</v-icon>
            <p class="display-3">Activation <span class="red--text">failed.</span></p>
          </div>
        </base-material-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";

export default Vue.extend({
  data: function () {
    return {
      waitForResponse: true,
      activeted: false,
    };
  },
  created: async function() {
    
      try{
        await this.$store.dispatch("activate", this.$route.params.activationToken);
        this.activeted = true;
        this.waitForResponse = false;
      }catch(reason){
        this.activeted = false;
        this.waitForResponse = false;
      }
  },
  methods: {},
});
</script>