
import { ActionContext } from 'vuex';
import { AuthenticationData } from '@/models/AuthenticationData'
import { LoginSuccess } from '@/models/LoginSuccess'
import { User } from '@/models/User'
import { State } from '.';
import api from '@/api'

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
    login: async function (context: Context, data: AuthenticationData): Promise<LoginSuccess> {
      const response = await api.login(data);
      context.commit("setJwt", response.jwt);
      await context.dispatch("profile");
      return response;
    },
    profile: async function (context: Context): Promise<User> {
      const user = await api.profile(context.state.jwt);
      context.commit('setUser', user);
      return user;
    }
  },
}
