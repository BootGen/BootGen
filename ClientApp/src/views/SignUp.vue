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
          <div v-if="registration" class="d-flex flex-column align-center">
            <v-icon color="green" size="70">mdi-check-circle-outline</v-icon>
            <p class="display-2 text-center">Registration was <span class="green--text">successful.</span></p>
            <p class="display-1 text-center">Thank you for choosing us!</p>
            <p class="caption text-center">Please check your e-mail account and click on the link in the message. If you do not find the confirmation e-mail, please check your spam folder.</p>
          </div>
          <v-form v-else>
            <v-container class="py-0">
              <v-row>
                <v-col cols="12">
                  <ValidationObserver v-slot="{ invalid }">
                    <v-col cols="12" class="text-center">
                      <ValidationProvider v-slot="{ errors }" name="user name" rules="required">
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
                      <ValidationProvider v-slot="{ errors }" name="password" rules="required|min:8|password:@confirmation">
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
                      <v-alert class="text-left" type="error" v-if="errorMsg">{{ errorMsg }}</v-alert>
                      <v-btn color="primary" large @click="trySignUp" :disabled="invalid">Sign up</v-btn>
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
import { required, email, min } from 'vee-validate/dist/rules';
import { extend, ValidationObserver, ValidationProvider } from 'vee-validate';
import api from "@/api"
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

extend('password', {
  params: ['target'],
  validate(value, {target}: any) {
    return value === target;
  },
  message: 'Password confirmation does not match'
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
      confirmPassword: "",
      userName: "",
      errorMsg: "",
      registration: false,
    };
  },
  methods: {
    trySignUp: async function () {
      this.$gtag.event('sign-up');
      const response = await api.register({
        userName: this.userName,
        email: this.email,
        password: this.password,
      });
      if(response.isUserNameInUse) {
        this.errorMsg = "This user name is already in use!";
      }else if(response.isEmailInUse) {
        this.errorMsg = "This email is already in use!";
      }else{
        this.registration = true;
        try{
          await this.$store.dispatch("login", {
            email: this.email,
            password: this.password,
          });
        }catch(e){
          this.errorMsg = "";
        }
      }
    },
  },
});
</script>
