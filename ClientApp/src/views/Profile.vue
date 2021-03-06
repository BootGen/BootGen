<template>
  <v-container id="user-profile" fluid tag="section">
    <v-row justify="center" v-if="$store.state.auth.user">
      <v-col cols="12" md="8">
        <base-material-card>
          <template v-slot:heading>
            <div class="display-2 font-weight-light">
              Edit Profile
            </div>

            <div class="subtitle-1 font-weight-light">
              Complete your profile
            </div>
          </template>

          <v-form>
            <v-container>
              <ValidationObserver v-slot="{ invalid }">
                <ValidationProvider v-slot="{ errors }" name="user name" rules="required|username">
                  <v-text-field
                    label="User Name"
                    v-model="$store.state.auth.user.userName"
                    :error-messages="errors"
                  ></v-text-field>
                </ValidationProvider>
                <ValidationProvider v-slot="{ errors }" name="email" rules="required|email">
                  <v-text-field
                    label="Email Address"
                    v-model="$store.state.auth.user.email"
                    :error-messages="errors"
                  ></v-text-field>
                </ValidationProvider>
                <v-checkbox color="primary" v-model="$store.state.auth.user.newsletter" label="Sign me up for the newsletter!"></v-checkbox>
                <v-alert type="error" v-if="errorMsg">{{ errorMsg }}</v-alert>
                <v-alert type="success" v-if="successMsg">{{ successMsg }}</v-alert>
                <div class="text-right">
                  <v-btn color="primary" class="mr-0"  @click="saveUser" :disabled="invalid">
                    Update Profile
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
            <h4 class="display-2 font-weight-light mb-3">
              {{ $store.state.auth.user.userName }}
            </h4>

            <p class="font-weight-light grey--text">
              {{ $store.state.auth.user.email }}
            </p>

            <v-btn color="primary" rounded class="mr-0" to="/change-password">
              Change Password
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
import api from '../api'

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
  },
  data: function () {
    return {
      dialog: false,
      errorMsg: '',
      successMsg: ''
    };
  },
  methods: {
    saveUser: async function () {
      this.$gtag?.event('update-profile');
      this.successMsg = '';
      this.$store.state.auth.user.userName = this.$store.state.auth.user.userName.trim();
      const response = await api.updateProfile(this.$store.state.auth.user, this.$store.state.auth.jwt);
      if (response.isUserNameInUse) {
        this.errorMsg = 'This user name already exists!';
      } else if (response.isEmailInUse) {
        this.errorMsg = 'This email already exists!';
      } else {
        this.errorMsg = '';
      }
      if (response.success) {
        this.successMsg = 'Your profile is successfuly updated.'
      }
    }
  },
});
</script>
