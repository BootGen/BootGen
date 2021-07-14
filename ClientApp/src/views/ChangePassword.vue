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
            <v-container>
              <ValidationObserver v-slot="{ invalid }">
                <ValidationProvider
                  v-slot="{ errors }"
                  name="oldPassword"
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
                  rules="required|min:8|oldPassword:@oldPassword|password:@confirmation"
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
                  rules="required|customPassword"
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
                <div class="text-right">
                  <v-btn color="primary" class="mr-0"  @click="savePassword" :disabled="invalid">
                    Change Password
                  </v-btn>
                </div>
              </ValidationObserver>
            </v-container>
          </v-form>
        </base-material-card>
      </v-col>

      <v-col cols="12" md="4">
        <base-material-card
          class="v-card-profile"
          :avatar="true"
        >
          <v-card-text class="text-center">
            <h4 class="display-2 mb-3">
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
import Vue from 'vue';
import { ValidationObserver, ValidationProvider } from 'vee-validate';
import api from '../api';

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
  },
  data: function () {
    return {
      dialog: false,
      oldPassword: '',
      newPassword: '',
      confirmPassword: '',
      errorMsg: '',
      successMsg: ''
    };
  },
  methods: {
    savePassword: async function () {
      this.$gtag?.event('change-password');
      this.successMsg = '';
      this.errorMsg = '';
      try {
            await api.changePassword({
              oldPassword: this.oldPassword,
              newPassword: this.newPassword,
            }, this.$store.state.auth.jwt);
            this.successMsg = 'Your password is successfuly changed.'
      } catch {
        this.errorMsg = 'Wrong password.'
      }
    }
  },
});
</script>
