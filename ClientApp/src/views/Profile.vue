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
            <v-container class="py-0">
              <v-row>
                <v-col cols="12">
                  <ValidationObserver v-slot="{ invalid }">
                    <ValidationProvider v-slot="{ errors }" name="user name" rules="required">
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
                    <v-checkbox v-model="$store.state.auth.user.newsletter" label="Sign me up for the newsletter!"></v-checkbox>
                    <v-alert type="error" v-if="errorMsg">{{ errorMsg }}</v-alert>
                    <v-alert type="success" v-if="successMsg">{{ successMsg }}</v-alert>
                    <v-col cols="12" class="text-right">
                      <v-btn color="primary" class="mr-0"  @click="saveUser" :disabled="invalid">
                        Update Profile
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
import { required, min } from 'vee-validate/dist/rules';
import { extend, ValidationObserver, ValidationProvider } from 'vee-validate';
import api from '../api'

extend('required', {
  ...required,
  message: '{_field_} can not be empty',
});

extend('min', {
  ...min,
  message: '{_field_} may not be less than {length} characters',
});

export default Vue.extend({
  components: {
    ValidationProvider,
    ValidationObserver,
  },
  mounted: async function() {
    if(this.$store.state.auth.jwt){
      this.$store.state.auth.user = await this.$store.dispatch('profile');
    }
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
      this.$gtag.event('update-profile');
      this.successMsg = '';
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