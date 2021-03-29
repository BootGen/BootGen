import axios from 'axios'
import { ActionContext } from 'vuex';
import { config, findById, patchArray, setArray, setItem } from './util';
import { State } from '.';
import { Project } from '@/models/Project'
import { User } from '@/models/User'
import { GeneratedFile } from '@/models/GeneratedFile';

export function projectToDto(project: Project): Project {
  return {
    id: project.id,
    name: project.name,
    json: project.json,
    ownerId: project.ownerId,
  };
}

export interface ProjectsState {
  items: Array<Project>;
  lastProject: Project;
  lastGeneratedFiles: Array<GeneratedFile>;
}

type Context = ActionContext<ProjectsState, State>;

interface ProjectUserPair {
  project: Project;
  user: User;
}

export default {
  namespaced: true,
  state: () => ({
    items: Array<Project>(),
    lastProject: {} as Project,
    lastGeneratedFiles: Array<GeneratedFile>()
  }),
  mutations: {
    setProjects: function(state: ProjectsState, projects: Array<Project>) {
      state.items = setArray(state.items,projects);
    },
    patchProjects: function(state: ProjectsState, projects: Array<Project>) {
      patchArray(state.items,projects);
    },
    setProject: function(state: ProjectsState, project: Project) {
      setItem(state.items, project);
    },
    setLastProject: function(state: ProjectsState, project: Project){
      state.lastProject = project;
    },
    setLastGeneratedFiles: function(state: ProjectsState, files: Array<GeneratedFile>){
      state.lastGeneratedFiles = files;
    }
  },
  actions: {
    getProjects: function(context: Context): Promise<Array<Project>> {
      return new Promise((resolve, reject) => {
        axios.get(`projects`, config(context.rootState.auth.jwt)).then(response => {
          context.commit("setProjects", response.data);
          resolve(context.state.items);
        }).catch(reason => {
          reject(reason);
        })
      })
    },
    getProject: function(context: Context, id: number): Promise<Project> {
      return new Promise((resolve, reject) => {
        axios.get(`projects/${id}`, config(context.rootState.auth.jwt)).then(response => {
          context.commit("setProject", response.data);
          const savedItem = findById<Project>(context.state.items, response.data.id);
          if (savedItem)
            resolve(savedItem);
        }).catch(reason => {
          reject(reason);
        })
      })
    },
    addProject: function(context: Context, project: Project): Promise<Project> {
      return new Promise((resolve, reject) => {
        axios.post(`projects`, projectToDto(project), config(context.rootState.auth.jwt)).then(response => {
          context.commit("setProject", response.data);
          const savedItem = findById<Project>(context.state.items, response.data.id);
          if (savedItem)
            resolve(savedItem);
        }).catch(reason => {
          reject(reason);
        })
      })
    },
    updateProject: function(context: Context, project: Project): Promise<Project> {
      return new Promise((resolve, reject) => {
        axios.put(`projects/${project.id}`, projectToDto(project), config(context.rootState.auth.jwt)).then(response => {
          context.commit("setProject", response.data);
          const savedItem = findById<Project>(context.state.items, response.data.id);
          if (savedItem)
            resolve(savedItem);
        }).catch(reason => {
          reject(reason);
        })
      })
    },
    deleteProject: function(context: Context, project: Project): Promise<void> {
      context.commit("setProjects", context.state.items.filter((i: Project) => i !== project));

      return new Promise((resolve, reject) => {
        axios.delete(`projects/${project.id}`, config(context.rootState.auth.jwt)).then(() => {
          resolve()
        }).catch(reason => {
          reject(reason);
        })
      })
    }
  }
}
