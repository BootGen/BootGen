import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import authModule, { AuthState } from '@/store/AuthModule'
import projectModule, { ProjectsState } from '@/store/ProjectModule'

Vue.use(Vuex)

axios.defaults.baseURL = process.env.VUE_APP_API_URL

export interface State {
  auth: AuthState;
  projects: ProjectsState;
}

export default new Vuex.Store<State>({
  modules: {
    auth: authModule,
    projects: projectModule,
  }
})
