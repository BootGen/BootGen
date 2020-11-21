import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import { User } from '@/models/User'
import { AuthenticationData } from '@/models/AuthenticationData'
import { LoginResponse } from '@/models/LoginResponse'
import { RegistrationData } from '@/models/RegistrationData'
import { ProfileResponse } from '@/models/ProfileResponse'
import { ChangePasswordData } from '@/models/ChangePasswordData'
import { ChangePasswordResponse } from '@/models/ChangePasswordResponse'
import { GenerateRequest } from '@/models/GenerateRequest'
import { GenerateResponse } from '@/models/GenerateResponse'
import { GeneratedFile } from '@/models/GeneratedFile'

Vue.use(Vuex)

axios.defaults.baseURL = "https://localhost:5001"

const dateFormat = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/;

function reviver(key: string, value: string) {
  if (typeof value === "string" && dateFormat.test(value)) {
    return new Date(value);
  }
  return value;
}

function transformResponse(response: string) {
  if (response && response.trim()) {
    return JSON.parse(response, reviver);
  }
  return null;
}

function config(context: any) {
  return {headers: {'Authorization': 'Bearer ' + context.state.jwt}, transformResponse: transformResponse };
}

interface EntityWithId {
  id: number;
}

function setItem<T extends EntityWithId>(array: Array<T>, item: T): T {
  const oldItem = array.find(i => i.id == item.id);
  if (oldItem) {
    Object.assign(oldItem, item);
    return oldItem;
  } else {
    array.push(item);
    return item;
  }
}

function setArray<T extends EntityWithId>(target: Array<T>, source: Array<T>): Array<T> {
  const result = Array<T>();
  source.forEach(item =>{
    const oldItem = target.find(i => i.id == item.id);
    if (oldItem) {
      Object.assign(oldItem, item);
      result.push(oldItem);
    } else {
      result.push(item);
    }
  });
  return result;
}

function patchArray<T extends EntityWithId>(target: Array<T>, source: Array<T>) {
  source.forEach(item =>{
    setItem(target, item);
  });
}

function findById<T extends EntityWithId>(array: Array<T>, id: number): T | undefined {
  return array.find(i => i.id === id);
}

function userToDto(user: User): any {
  return {
    id: user.id,
    userName: user.userName,
    email: user.email,
  };
}
export default new Vuex.Store({
  state: {
    users: Array<User>(),
    jwt: ""
  },
  mutations: {
    setUsers: function(state, users) {
      state.users = setArray(state.users,users);
    },
    patchUsers: function(state, users) {
      patchArray(state.users,users);
    },
    setUser: function(state, user) {
      setItem(state.users, user);
    },
    init: function(state) {
      const jwt = localStorage.getItem("jwt")
      if (jwt) {
        state.jwt = jwt;
      }
    },
    setJwt: function(state, jwt) {
      state.jwt = jwt
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
  actions: {
    getUsers: function(context): Promise<Array<User>> {
      return new Promise((resolve, reject) => {
        axios.get(`users`, config(context)).then(response => {
          context.commit("setUsers", response.data);
          resolve(context.state.users);
        }).catch(reason => {
          reject(reason);
        })
      })
    },
    getUser: function(context, id): Promise<User> {
      return new Promise((resolve, reject) => {
        axios.get(`users/${id}`, config(context)).then(response => {
          context.commit("setUser", response.data);
          resolve(findById(context.state.users, response.data.id));
        }).catch(reason => {
          reject(reason);
        })
      })
    },
    login: function(context, data): Promise<LoginResponse> {
      return new Promise((resolve, reject) => {
        axios.post("authentication/login", data).then(response => {
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
    register: function(context, data): Promise<ProfileResponse> {
      return new Promise((resolve, reject) => {
        axios.post("registration/register", data).then(response => {
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
    profile: function(context): Promise<User> {
      return new Promise((resolve, reject) => {
        axios.get("profile/profile", config(context)).then(response => {
          context.commit("setUser", response.data);
          resolve(findById(context.state.users, response.data.id));
        }).catch(reason => {
          reject({
            status: reason.response.status,
            statusText: reason.response.statusText,
            message: reason.message
          });
        })
      });
    },
    updateProfile: function(context, user): Promise<ProfileResponse> {
      return new Promise((resolve, reject) => {
        axios.post("profile/update-profile", userToDto(user), config(context)).then(response => {
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
    changePassword: function(context, data): Promise<ChangePasswordResponse> {
      return new Promise((resolve, reject) => {
        axios.post("profile/change-password", data, config(context)).then(response => {
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
    generate: function(context, request): Promise<GenerateResponse> {
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
  },
  modules: {
  }
})
