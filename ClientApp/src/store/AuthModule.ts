
import { ActionContext } from 'vuex';
import { AuthenticationData } from '@/models/AuthenticationData'
import { LoginSuccess } from '@/models/LoginSuccess'
import { User } from '@/models/User'
import { State } from '.';
import api from '@/api';
import githubApi from '@/github-api'
import { ProfileResponse } from '@/models/ProfileResponse';
import { OAuthLoginSuccess } from '@/models/OAuthLoginSuccess';
import { OAuthLoginData } from '@/models/OAuthLoginData';

export interface AuthState {
  jwt: string;
  githubToken: string;
  user: User | null;
}
type Context = ActionContext<AuthState, State>;

export default {
  state: {
    jwt: '',
    githubToken: '',
    user: null
  },
  mutations: {
    setJwt: function(state: AuthState, jwt: string) {
      state.jwt = jwt
      try {
        if (jwt) {
          localStorage.setItem('jwt', jwt);
        } else {
          localStorage.removeItem('jwt');
        }
      } catch {
        console.log('Local storage is not available.')
      }
    },
    setGithubToken: function(state: AuthState, githubToken: string) {
      state.githubToken = githubToken;
      try {
        if (githubToken) {
          localStorage.setItem('githubToken', githubToken);
        } else {
          localStorage.removeItem('githubToken');
        }
      } catch {
        console.log('Local storage is not available.')
      }
    },
    setUser: function(state: AuthState, user: User) {
      state.user = user
    }
  },
  actions: {
    init: function(context: Context) {
      const jwt = localStorage.getItem('jwt')
      if (jwt) {
        context.commit('setJwt', jwt);
      }
    },
    login: async function (context: Context, data: AuthenticationData): Promise<LoginSuccess> {
      const response = await api.login(data);
      context.commit('setJwt', response.jwt);
      await context.dispatch('profile');
      return response;
    },
    githubAuthorize: async function (context: Context): Promise<void> {
      await githubApi.authorize();
    },
    githubLogin: async function (context: Context, data: OAuthLoginData): Promise<OAuthLoginSuccess> {
      const response = await api.githubLogin(data);
      context.commit('setGithubToken', response.accessToken);
      context.commit('setJwt', response.jwt);
      await context.dispatch('profile');
      return response;
    },
    profile: async function (context: Context): Promise<User> {
      const user = await api.profile(context.state.jwt);
      context.commit('setUser', user);
      return user;
    },
    updateProfile: async function (context: Context, user: User): Promise<ProfileResponse> {
      const response  = await api.updateProfile(user, context.state.jwt);
      if (response.success)
        context.commit('setUser', user);
      return response;
    }
  },
}
