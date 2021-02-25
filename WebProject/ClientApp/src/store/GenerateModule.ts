
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
    },
    download: function(context: Context, request: GenerateRequest): Promise<void> {
      return new Promise((resolve, reject) => {

        axios.post("generate/download", request, {responseType: 'blob'}).then(response => {
          const fileURL = window.URL.createObjectURL(new Blob([response.data]));
          const fileLink = document.createElement('a');
     
          fileLink.href = fileURL;
          fileLink.setAttribute('download', 'project.zip');
          document.body.appendChild(fileLink);
     
          fileLink.click();
          resolve();
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