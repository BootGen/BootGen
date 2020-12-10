<template>
  <v-container fluid>
    <v-row class="d-flex justify-center">
      <v-col cols="12" sm="5">
        <base-material-card>
          <template v-slot:heading>
            <div class="d-flex display-2 font-weight-light flex-column justify-space-between align-center">
              <span>Sing Up</span>
              <v-icon class="pa-2">mdi-account-plus</v-icon>
            </div>
          </template>

          <v-form>
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
                        ></v-text-field>
                      </ValidationProvider>
                      <ValidationProvider v-slot="{ errors }" name="email" rules="required|email">
                        <v-text-field
                          v-model="email"
                          :error-messages="errors"
                          placeholder="E-mail"
                        ></v-text-field>
                      </ValidationProvider>
                      <ValidationProvider v-slot="{ errors }" name="password" rules="required|min:8|password:@confirmation">
                        <v-text-field
                          v-model="password"
                          :error-messages="errors"
                          placeholder="Password"
                          type="password" 
                        ></v-text-field>
                      </ValidationProvider>
                      <ValidationProvider v-slot="{ errors }" name="confirmation" rules='required|min:8'>
                        <v-text-field
                          v-model="confirmPassword"
                          :error-messages="errors"
                          placeholder="Confirm Password"
                          type="password" 
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
      errorMsg: ""
    };
  },
  created: function() {
    console.log("Hello?");
  },
  methods: {
    trySignUp: async function () {
      const response = await this.$store.dispatch("register", {
        userName: this.userName,
        email: this.email,
        password: this.password,
      });
      if(response.isUserNameInUse) {
        this.errorMsg = "This user name is already in use!";
      }else if(response.isEmailInUse) {
        this.errorMsg = "This email is already in use!";
      }else{
        this.errorMsg = "";
        const response = await this.$store.dispatch("login", {
          email: this.email,
          password: this.password,
        });
        this.$store.commit("setJwt", response.jwt);
        this.$root.$data.user = response.user;
        this.$router.push("profile");
      }
    },
  },
});
</script>