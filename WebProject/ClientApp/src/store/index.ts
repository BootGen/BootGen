import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import projectModule, { ProjectsState } from '@/store/ProjectModule'
import userModule, { UsersState } from '@/store/UserModule'
import ProjectSettingsModule, { ProjectSettingsState} from '@/store/ProjectSettingsModule'
import authModule from '@/store/AuthModule'
import generateModule from '@/store/GenerateModule'
import { GenerateRequest } from '@/models/GenerateRequest';

Vue.use(Vuex)

axios.defaults.baseURL = "https://localhost:5001"

export interface State {
  jwt: string;
  users: UsersState;
  projects: ProjectsState;
  projectSettings: ProjectSettingsState;
}

export default new Vuex.Store<State>({
  state: {
    jwt: "",
    users : {
      items: []
    },
    projects : {
      items: []
    },
    projectSettings : {
      item: {data: "", generateClient: false, nameSpace: ""}
    }
  },
  mutations: {
    setJwt: function(state, jwt) {
      state.jwt = jwt
    }
  },
  actions: {

    init: function(context) {
      const jwt = localStorage.getItem("jwt")
      if (jwt) {
        context.dispatch('setJwt', jwt);
      }
    },
    setJwt(context, jwt) {
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
    }
  },
  modules: {
    auth: authModule,
    generate: generateModule,
    projects: projectModule,
    users: userModule,
    projectSettings: ProjectSettingsModule,
  }
})
