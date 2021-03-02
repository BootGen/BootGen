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
  state: ()  => ({
    item: {} as GenerateRequest
  }),
  mutations: {
    setProjectSettings: function(state: ProjectSettingsState, settings: GenerateRequest) {
      state.item = settings;
    },
  },
}
