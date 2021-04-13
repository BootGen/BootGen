import { ActionContext } from 'vuex';
import { findById, patchArray, setArray, setItem } from './util';
import { State } from '.';
import { Project } from '@/models/Project'
import { GeneratedFile } from '@/models/GeneratedFile';
import api from '@/api'

export interface ProjectsState {
  items: Array<Project>;
  lastProject: Project;
  lastGeneratedFiles: Array<GeneratedFile>;
}

type Context = ActionContext<ProjectsState, State>;

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
    getProjects: async function(context: Context): Promise<Array<Project>> {
      const projects = await api.getProjects(context.rootState.auth.jwt);
      context.commit('setProjects', projects);
      return context.state.items;
    },
    getProject: async function(context: Context, id: number): Promise<Project | undefined> {
      const project = await api.getProject(id, context.rootState.auth.jwt);
      context.commit('setProject', project);
      return findById<Project>(context.state.items, project.id);
    },
    addProject: async function(context: Context, project: Project): Promise<Project> {
      project = await api.addProject(project, context.rootState.auth.jwt);
      context.commit('setProject', project);
      return findById<Project>(context.state.items, project.id) as Project;
    },
    updateProject: async function(context: Context, project: Project): Promise<Project> {
      project = await api.updateProject(project, context.rootState.auth.jwt);
      context.commit('setProject', project);
      return findById<Project>(context.state.items, project.id) as Project;
    },
    deleteProject: async function(context: Context, project: Project): Promise<void> {
      context.commit('setProjects', context.state.items.filter((i: Project) => i !== project));
      await api.deleteProject(project, context.rootState.auth.jwt);
    }
  }
}
