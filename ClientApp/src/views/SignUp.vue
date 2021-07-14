<template>
  <v-container fluid>
    <v-row class="d-flex justify-center">
      <v-col cols="12" md="6">
        <base-material-card>
          <template v-slot:heading>
            <div class="d-flex display-2 font-weight-light flex-column justify-space-between align-center">
              <span>Registration</span>
              <v-icon class="pa-2">mdi-account-plus</v-icon>
            </div>
          </template>
          <div v-if="registrationComplete" class="d-flex flex-column align-center">
            <v-icon color="primary" size="70">mdi-check-circle-outline</v-icon>
            <p class="display-2 text-center">Registration was <span class="primary--text">successful.</span></p>
            <p class="display-1 text-center">Thank you for choosing us!</p>
            <p class="caption text-center">Please check your e-mail account and click on the link in the message. If you do not find the confirmation e-mail, please check your spam folder.</p>
          </div>
          <v-form v-else>
            <v-container>
              <ValidationObserver v-slot="{ invalid }">
                <ValidationProvider v-slot="{ errors }" name="user name" rules="required|username">
                  <v-text-field
                    v-model="userName"
                    :error-messages="errors"
                    placeholder="User Name"
                  prepend-icon="mdi-face-outline"
                  ></v-text-field>
                </ValidationProvider>
                <ValidationProvider v-slot="{ errors }" name="email" rules="required|email">
                  <v-text-field
                    v-model="email"
                    :error-messages="errors"
                    placeholder="E-mail"
                  prepend-icon="mdi-email-outline"
                  ></v-text-field>
                </ValidationProvider>
                <ValidationProvider v-slot="{ errors }" name="password" rules="required|customPassword|password:@confirmation">
                  <v-text-field
                    v-model="password"
                    :error-messages="errors"
                    placeholder="Password"
                    type="password"
                    prepend-icon="mdi-form-textbox-password"
                  ></v-text-field>
                </ValidationProvider>
                <ValidationProvider v-slot="{ errors }" name="confirmation" rules='required|min:8'>
                  <v-text-field
                    v-model="confirmPassword"
                    :error-messages="errors"
                    placeholder="Confirm Password"
                    type="password"
                    prepend-icon="mdi-form-textbox-password"
                  ></v-text-field>
                </ValidationProvider>
                <v-checkbox color="primary" v-model="newsletter" label="Sign me up for the newsletter!"></v-checkbox>
                <v-alert class="text-left" type="error" v-if="errorMsg">{{ errorMsg }}</v-alert>
                <div class="text-center">
                  <v-btn color="primary" large @click="trySignUp" :disabled="invalid">Sign up</v-btn>
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
import api from '../api'

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
  },
  data: function () {
    return {
      email: '',
      password: '',
      confirmPassword: '',
      userName: '',
      newsletter: false,
      errorMsg: '',
      registrationComplete: false
    };
  },
  methods: {
    trySignUp: async function () {
      this.$gtag?.event('sign-up');
      const response = await api.register({
        userName: this.userName,
        email: this.email,
        newsletter: this.newsletter,
        password: this.password,
      });
      if(response.isUserNameInUse) {
        this.errorMsg = 'This user name is already in use!';
      }else if(response.isEmailInUse) {
        this.errorMsg = 'This email is already in use!';
      } else {
        this.registrationComplete = true;
      }
    },
  },
});
</script>
