<template>
  <v-container id="user-profile" fluid tag="section">
    <v-row justify="center" v-if="$store.state.auth.user">
      <v-col cols="12" md="8">
        <base-material-card>
          <template v-slot:heading>
            <div class="display-2 font-weight-light">
              Change Password
            </div>

            <div class="subtitle-1 font-weight-light">
              Complete your password
            </div>
          </template>

          <v-form>
            <v-container class="py-0">
              <v-row>
                <v-col cols="12">
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
                    <v-col cols="12" class="text-right">
                      <v-btn color="primary" class="mr-0"  @click="savePassword" :disabled="invalid">
                        Change Password
                      </v-btn>
                    </v-col>
                  </ValidationObserver>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </base-material-card>
      </v-col>

      <v-col cols="12" md="4">
        <base-material-card
          class="v-card-profile"
          avatar="https://demos.creative-tim.com/vue-material-dashboard/img/marc.aba54d65.jpg"
        >
          <v-card-text class="text-center">
            <h4 class="display-2 font-weight-light mb-3 black--text">
              {{ $store.state.auth.user.userName }}
            </h4>

            <p class="font-weight-light grey--text">
              {{ $store.state.auth.user.email }}
            </p>

            <v-btn color="primary" rounded class="mr-0" to="/profile">
              Edit Profile
            </v-btn>
          </v-card-text>
        </base-material-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { required, min } from "vee-validate/dist/rules";
import { extend, ValidationObserver, ValidationProvider } from "vee-validate";
import api from "@/api";

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
      this.$gtag.event('change-password');
      this.successMsg = "";
      this.errorMsg = "";
      try {
            await api.changePassword({
              oldPassword: this.oldPassword,
              newPassword: this.newPassword,
            }, this.$store.state.auth.jwt);
            this.successMsg = "Your password is successfuly changed."
      } catch {
        this.errorMsg = "Wrong password."
      }
    }
  },
});
</script>
