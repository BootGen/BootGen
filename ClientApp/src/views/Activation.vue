<template>
  <v-container id="privacy-statement" fluid tag="section">
    <v-row justify="center">
      <v-col cols="12" md="8">
        <base-material-card>
          <template v-slot:heading>
            <div class="display-2 font-weight-light">
              Activation your account
            </div>
          </template>
          <div class="d-flex flex-column align-center">
            <h1>You're almost there!</h1>
            <h3>Just one more step to get started.</h3>
            <p>After activating your account, click the button below to login to your account.</p>
          </div>
          <v-row class="d-flex justify-center" >
            <v-col cols="12" md="10">
              <v-alert class="text-left" type="error" v-if="errorMsg">{{ errorMsg }}</v-alert>
            </v-col>
          </v-row>
          <div class="d-flex justify-center">
            <v-btn color="primary" @click="tryLogin">Login</v-btn>
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
      errorMsg: ""
    };
  },
  methods: {
    tryLogin: async function(){
      try{
        const response = await this.$store.dispatch("login", {
          email: this.$route.params.email,
          password: this.$route.params.password,
        });
        this.$store.commit("setJwt", response.jwt);
        this.$store.state.auth.user = response.user;
        this.$router.push("profile");
      }catch(e){
        if(e.statusText === "Unauthorized"){
          this.errorMsg = "You have not confirmed your e-mail address yet. Please check your e-mail account and click on the link in the message. If you do not find the confirmation e-mail, please check your spam folder."
        }
      }
    }
  },
});
</script>