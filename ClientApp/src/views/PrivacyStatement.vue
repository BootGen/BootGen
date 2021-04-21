<template>
  <v-container id="privacy-statement" fluid tag="section">
    <v-row justify="center">
      <v-col cols="12">
        <base-material-card>
          <template v-slot:heading>
            <div class="display-2 font-weight-light">
              Privacy statement
            </div>
          </template>
          <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Quidem nam architecto, molestiae dolorem eaque facere. Inventore quibusdam quidem eveniet! Fugit quibusdam voluptate temporibus earum fugiat ex quaerat! Sequi, fugiat nesciunt.</p>
          <v-checkbox v-model="cookiesAccepted" label="Accept Google Analytics cookies"></v-checkbox>
          <div class="d-flex justify-end">
            <v-btn color="primary" @click="consent()">consent</v-btn>
          </div>
        </base-material-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';

export default Vue.extend({
  data: function () {
    return {
      cookiesAccepted: true,
    };
  },
  watch:{
    cookiesAccepted(accepted) {
      this.$gtag?.event('set-opt');
      if(accepted){
        localStorage.cookiesAccepted = true;
        this.$gtag?.optIn();
      }else{
        localStorage.cookiesAccepted = false;
        this.$gtag?.optOut();
      }
    }
  },
  created: function(){
    if(localStorage.cookiesAccepted === 'false'){
      this.cookiesAccepted = false;
    }else{
      localStorage.cookiesAccepted = true;
    }
  },
  methods: {
    consent: function(){
      this.$gtag?.event('consent-privacy-statement');
      localStorage.cookieConsentAnswered = true;
      this.$router.go(-1);
    }
  },
});
</script>

<style>
label.v-label.theme--light{
  color: #412fb3;
}
</style>