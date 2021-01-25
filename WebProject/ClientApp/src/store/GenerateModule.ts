
import axios from 'axios'
import { ActionContext } from 'vuex';
import { State } from '.';
import { GenerateRequest } from '@/models/GenerateRequest';
import { GenerateResponse } from '@/models/GenerateResponse';

type Context = ActionContext<{}, State>;
export default {
  actions: {
    generate: function(context: Context, request: GenerateRequest): Promise<GenerateResponse> {
      return new Promise((resolve, reject) => {
        axios.post("generate/generate", request).then(response => {
          resolve(response.data);
        }).catch(reason => {
          reject({
            status: reason.response.status,
            statusText: reason.response.statusText,
            message: reason.message
          });
        })
      });
    }
  },
}