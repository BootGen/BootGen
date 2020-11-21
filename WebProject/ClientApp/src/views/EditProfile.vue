<template>
  <v-container fluid>
    <v-row>
      <v-col cols="8">
        <ValidationObserver v-slot="{ invalid }">
          <h4>Name:</h4>
          <ValidationProvider
            v-slot="{ errors }"
            name="user name"
            rules="required"
          >
            <v-text-field
              v-model="userName"
              :error-messages="errors"
            ></v-text-field>
          </ValidationProvider>
          <h4>Email:</h4>
          <ValidationProvider
            v-slot="{ errors }"
            name="email"
            rules="required|email"
          >
            <v-text-field
              v-model="email"
              :error-messages="errors"
            ></v-text-field>
          </ValidationProvider>
          <v-alert type="error" v-if="errorMsg">{{ errorMsg }}</v-alert>
          <v-alert type="success" v-if="successMsg">{{ successMsg }}</v-alert>
          <v-btn large @click="saveUser" :disabled="invalid">Save</v-btn
          >
        </ValidationObserver>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { required, min } from "vee-validate/dist/rules";
import { extend, ValidationObserver, ValidationProvider } from "vee-validate";

extend("required", {
  ...required,
  message: "{_field_} can not be empty",
});

extend("min", {
  ...min,
  message: "{_field_} may not be less than {length} characters",
});

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
  },
  data: function () {
    return {
      dialog: false,
      userName: this.$root.$data.user.userName,
      email: this.$root.$data.user.email,
      errorMsg: "",
      successMsg: ""
    };
  },
  methods: {
    saveUser: async function () {
      this.successMsg = "";
      this.$root.$data.user.userName = this.userName;
      this.$root.$data.user.email = this.email;
      const response = await this.$store.dispatch(
        "updateProfile",
        this.$root.$data.user
      );
      if (response.isUserNameInUse) {
        this.errorMsg = "This user name already exists!";
      } else if (response.isEmailInUse) {
        this.errorMsg = "This email already exists!";
      } else {
        this.errorMsg = "";
      }
      if (response.success) {
        this.successMsg = "Your profile is successfuly updated."
      }
    }
  },
});
</script>