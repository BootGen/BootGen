import axios from 'axios'
import { ActionContext } from 'vuex';
import { config, findById, findObjectsById, patchArray, setArray, setItem } from './util';
import { State } from '.';
import { GenerateRequest } from '@/models/GenerateRequest';

export function projectSettingsToDto(project: GenerateRequest): GenerateRequest {
  return {
    data: project.data,
    generateClient: project.generateClient,
    nameSpace: project.nameSpace,
  };
}

export interface ProjectSettingsState {
  item: GenerateRequest;
}

type Context = ActionContext<ProjectSettingsState, State>;

export default {
  //namespaced: true,
  state: () => ({
    item: null as (GenerateRequest | null)
  }),
  mutations: {
    setProjectSettings: function(state: ProjectSettingsState, settings: GenerateRequest) {
      state.item = settings;
    },
  },
  actions: {
    getProjectSettings: function(context: Context): GenerateRequest {
        return context.state.item;
    },
    updateProjectSettings: function(context: Context, settings: GenerateRequest): GenerateRequest {
      context.commit("setProjectSettings", settings);
      return context.state.item;
    },
  }
}
