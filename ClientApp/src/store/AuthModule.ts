
import axios from 'axios'
import { ActionContext } from 'vuex';
import { AuthenticationData } from '@/models/AuthenticationData'
import { LoginResponse } from '@/models/LoginResponse'
import { RegistrationData } from '@/models/RegistrationData'
import { ProfileResponse } from '@/models/ProfileResponse'
import { ChangePasswordData } from '@/models/ChangePasswordData'
import { ChangePasswordResponse } from '@/models/ChangePasswordResponse'
import { User } from '@/models/User'
import { userToDto } from '@/store/UserModule'
import { config, findById } from './util';
import { State } from '.';

export interface AuthState {
  jwt: string;
  user: User | null;
}
type Context = ActionContext<AuthState, State>;
export default {
  state: {
    jwt: "",
    user: null
  },
  mutations: {
    setJwt: function(state: AuthState, jwt: string) {
      state.jwt = jwt
    },
    setUser: function(state: AuthState, user: User) {
      state.user = user
    }
  },
  actions: {
    init: function(context: Context) {
      const jwt = localStorage.getItem("jwt")
      if (jwt) {
        context.dispatch('setJwt', jwt);
      }
    },
    setJwt(context: Context, jwt: string) {
      context.commit('setJwt', jwt);
      try {
        if (jwt) {
          localStorage.setItem("jwt", jwt);
        } else {
          localStorage.removeItem("jwt");
        } 
      } catch {
        console.log("Local storage is not available.")
      }
    },
    login: function (context: Context, data: AuthenticationData): Promise<LoginResponse> {
      return new Promise((resolve, reject) => {
        axios.post("authentication/login", data).then(response => {
          context.commit("setJwt", response.data.jwt);
          context.dispatch("profile").then(() => {
            resolve(response.data);
          });
        }).catch(reason => {
          reject({
            status: reason.response.status,
            statusText: reason.response.statusText,
            message: reason.message
          });
        })
      });
    },
    register: function (context: Context, data: RegistrationData): Promise<ProfileResponse> {
      return new Promise((resolve, reject) => {
        axios.post("registration/register", data).then(response => {
          resolve(response.data);
        }).catch(reason => {
          reject({
            status: reason.response.status,
            statusText: reason.response.statusText,
            message: reason.message
          });
        })
      });
    },
    profile: function (context: Context): Promise<User> {
      return new Promise((resolve, reject) => {
        axios.get("profile/profile", config(context.state.jwt)).then(response => {
          context.commit("users/setUser", response.data);
          const savedItem = findById<User>(context.rootState.users.items, response.data.id);
          if (savedItem) {
            context.commit('setUser', savedItem);
            resolve(savedItem);
          }
        }).catch(reason => {
          reject({
            status: reason.response.status,
            statusText: reason.response.statusText,
            message: reason.message
          });
        })
      });
    },
    updateProfile: function (context: Context, user: User): Promise<ProfileResponse> {
      return new Promise((resolve, reject) => {
        axios.post("profile/update-profile", userToDto(user), config(context.state.jwt)).then(response => {
          resolve(response.data);
        }).catch(reason => {
          reject({
            status: reason.response.status,
            statusText: reason.response.statusText,
            message: reason.message
          });
        })
      });
    },
    changePassword: function (context: Context, data: ChangePasswordData): Promise<ChangePasswordResponse> {
      return new Promise((resolve, reject) => {
        axios.post("profile/change-password", data, config(context.state.jwt)).then(response => {
          resolve(response.data);
        }).catch(reason => {
          reject({
            status: reason.response.status,
            statusText: reason.response.statusText,
            message: reason.message
          });
        })
      });
    },
  },
}