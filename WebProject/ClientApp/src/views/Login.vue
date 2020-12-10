<template>
  <v-container fluid>
    <v-row>
      <v-col cols="8">
        <ValidationObserver v-slot="{ invalid }">
            <ValidationProvider v-slot="{ errors }" name="email" rules="required|email">
            <v-text-field
                v-model="email"
                :error-messages="errors"
                placeholder="E-mail"
            ></v-text-field>
            </ValidationProvider>
            <ValidationProvider v-slot="{ errors }" name="password" rules="required">
            <v-text-field
                v-model="password"
                :error-messages="errors"
                placeholder="Password"
                type="password" 
            ></v-text-field>
            </ValidationProvider>
            <p>email: example@email.com | pass: password123</p>
            <v-alert type="error" v-if="errorMsg">{{ errorMsg }}</v-alert>
            <v-btn color="primary" large @click="tryLogin" :disabled="invalid">Sign in</v-btn>
        </ValidationObserver>
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
      try {
        const response = await this.$store.dispatch("login", {
          email: this.email,
          password: this.password,
        });
        this.$store.commit("setJwt", response.jwt);
        this.$root.$data.user = response.user;
        this.$router.push("profile");
      } catch (reason) {
        this.errorMsg = "Incorrect email or password";
      }
    }
  },
});
</script>