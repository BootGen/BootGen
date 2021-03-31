<template>
  <v-container fluid>
    <v-row class="d-flex justify-center">
      <v-col cols="12" md="6">
        <base-material-card>
          <template v-slot:heading>
            <div class="d-flex display-2 font-weight-light flex-column justify-space-between align-center">
              <span>Login</span>
              <v-icon class="pa-2">mdi-account-arrow-left</v-icon>
            </div>
          </template>

          <v-form>
            <v-container class="py-0">
              <v-row>
                <v-col cols="12">
                  <ValidationObserver v-slot="{ invalid }">
                    <v-col cols="12" class="text-center"> 
                      <ValidationProvider v-slot="{ errors }" name="email" rules="required|email">
                      <v-text-field
                        label="E-mail"
                        v-model="email"
                        :error-messages="errors"
                        prepend-icon="mdi-email-outline"
                      ></v-text-field>
                      </ValidationProvider>
                      <ValidationProvider v-slot="{ errors }" name="password" rules="required">
                        <v-text-field
                          label="Password"
                          v-model="password"
                          :error-messages="errors"
                          type="password" 
                          prepend-icon="mdi-form-textbox-password"
                        ></v-text-field>
                      </ValidationProvider> 
                      <p>email: example@email.com | pass: password123</p>
                      <v-alert class="text-left" type="error" v-if="errorMsg">{{ errorMsg }}</v-alert>
                      <v-btn color="primary" large @click="tryLogin" :disabled="invalid">Sign in</v-btn>
                    </v-col>
                </ValidationObserver>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </base-material-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { required, email, min, is } from 'vee-validate/dist/rules';
import { extend, ValidationObserver, ValidationProvider } from 'vee-validate';

extend('required', {
  ...required,
  message: '{_field_} can not be empty',
});

extend('min', {
  ...min,
  message: '{_field_} may not be less than {length} characters',
});

extend('email', {
  ...email,
  message: 'Email must be valid',
});

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
  },
  data: function () {
    return {
      email: "",
      password: "",
      passwordAgain: "",
      userName: "",
      errorMsg: ""
    };
  },
  methods: {
    tryLogin: async function () {
      this.$gtag.event('login');
      try {
        const response = await this.$store.dispatch("login", {
          email: this.email,
          password: this.password,
        });
        this.$store.commit("setJwt", response.jwt);
        this.$store.state.auth.user = response.user;
        this.$router.push("profile");
      } catch (reason) {
        console.log("reas", reason);
        if(reason.statusText === "Unauthorized"){
          this.errorMsg = "You have not confirmed your e-mail address yet. Please check your e-mail account and click on the link in the message. If you do not find the confirmation e-mail, please check your spam folder."
        }else{
          this.errorMsg = "Incorrect email or password";
        }
      }
    }
  },
});
</script>