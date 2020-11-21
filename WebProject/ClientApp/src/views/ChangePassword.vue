<template>
  <v-container fluid>
    <v-row>
      <v-col cols="8">
        <ValidationObserver v-slot="{ invalid }">
          <ValidationProvider
            v-slot="{ errors }"
            name="password"
            rules="required"
          >
            <v-text-field
              v-model="oldPassword"
              :error-messages="errors"
              placeholder="old password"
              type="password"
            ></v-text-field>
          </ValidationProvider>
          <ValidationProvider
            v-slot="{ errors }"
            name="password"
            rules="required|min:8|password:@confirmation"
          >
            <v-text-field
              v-model="newPassword"
              :error-messages="errors"
              placeholder="new password"
              type="password"
            ></v-text-field>
          </ValidationProvider>
          <ValidationProvider
            v-slot="{ errors }"
            name="confirmation"
            rules="required|min:8"
          >
            <v-text-field
              v-model="confirmPassword"
              :error-messages="errors"
              placeholder="confirm password"
              type="password"
            ></v-text-field>
          </ValidationProvider>
          <v-alert type="error" v-if="errorMsg">{{ errorMsg }}</v-alert>
          <v-alert type="success" v-if="successMsg">{{ successMsg }}</v-alert>
          <v-btn large @click="savePassword" :disabled="invalid">Save</v-btn>
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

extend("password", {
  params: ["target"],
  validate(value, { target }: any) {
    return value === target;
  },
  message: "Password confirmation does not match",
});

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
  },
  data: function () {
    return {
      dialog: false,
      oldPassword: "",
      newPassword: "",
      confirmPassword: "",
      errorMsg: "",
      successMsg: ""
    };
  },
  methods: {
    savePassword: async function () {
      this.successMsg = "";
      this.errorMsg = "";
      try {
            await this.$store.dispatch("changePassword", {
                user: this.$root.$data.user,
                oldPassword: this.oldPassword,
                newPassword: this.newPassword,
            });
            this.successMsg = "Your password is successfuly changed."
      } catch {
        this.errorMsg = "Wrong password."
      }
    }
  },
});
</script>