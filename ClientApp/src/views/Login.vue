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
            <v-container>
              <ValidationObserver v-slot="{ invalid }">
                <ValidationProvider v-slot="{ errors }" name="email" rules="required|email">
                <v-text-field
                  name="email"
                  label="E-mail"
                  v-model="email"
                  :error-messages="errors"
                  prepend-icon="mdi-email-outline"
                ></v-text-field>
                </ValidationProvider>
                <ValidationProvider v-slot="{ errors }" name="password" rules="required">
                  <v-text-field
                    name="password"
                    label="Password"
                    v-model="password"
                    :error-messages="errors"
                    type="password"
                    prepend-icon="mdi-form-textbox-password"
                  ></v-text-field>
                </ValidationProvider>
                <v-alert class="text-left" type="error" v-if="errorMsg">{{ errorMsg }}</v-alert>
                <div class="text-center">
                  <v-btn color="primary" large @click="tryLogin" :disabled="invalid">Sign in</v-btn>
                </div>
              </ValidationObserver>
            </v-container>
          </v-form>
        </base-material-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import { ValidationObserver, ValidationProvider } from 'vee-validate';
import {LoginError} from '../models/LoginError';

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
  },
  data: function () {
    return {
      email: '',
      password: '',
      passwordAgain: '',
      userName: '',
      errorMsg: ''
    };
  },
  methods: {
    tryLogin: async function () {
      this.$gtag?.event('login');
      try {
        await this.$store.dispatch('login', {
          email: this.email.trim(),
          password: this.password,
        });
        await this.$router.push('profile');
      } catch (reason) {
        const error: LoginError = reason.response.data;
        if(error.isInactive){
          this.errorMsg = 'You have not confirmed your e-mail address yet. Please check your e-mail account and click on the link in the message. If you do not find the confirmation e-mail, please check your spam folder.'
        }else{
          this.errorMsg = 'Incorrect email or password';
        }
      }
    }
  },
});
</script>
