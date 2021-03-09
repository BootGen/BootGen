import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import authModule, { AuthState } from '@/store/AuthModule'
import generateModule from '@/store/GenerateModule'
import userModule, { UsersState } from '@/store/UserModule'
import projectModule, { ProjectsState } from '@/store/ProjectModule'

Vue.use(Vuex)

axios.defaults.baseURL = "https://localhost:5001"

export interface State {
  auth: AuthState;
  users: UsersState;
  projects: ProjectsState;
}

export default new Vuex.Store<State>({
  modules: {
    auth: authModule,
    users: userModule,
    projects: projectModule,
    generate: generateModule,
  }
})
