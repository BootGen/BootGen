import axios from 'axios'
import { ActionContext } from 'vuex';
import { config, findById, findObjectsById, patchArray, setArray, setItem } from './util';
import { State } from '.';
import { User } from '@/models/User'

export function userToDto(user: User): User {
  return {
    id: user.id,
    userName: user.userName,
    email: user.email,
  };
}

export interface UsersState {
  items: Array<User>;
}

type Context = ActionContext<UsersState, State>;


export default {
  namespaced: true,
  state: () => ({
    items: Array<User>()
  }),
  mutations: {
    setUsers: function(state: UsersState, users: Array<User>) {
      state.items = setArray(state.items,users);
    },
    patchUsers: function(state: UsersState, users: Array<User>) {
      patchArray(state.items,users);
    },
    setUser: function(state: UsersState, user: User) {
      setItem(state.items, user);
    }
  },
  actions: {
    getUsers: function(context: Context): Promise<Array<User>> {
      return new Promise((resolve, reject) => {
        axios.get(`users`, config(context.rootState.jwt)).then(response => {
          context.commit("setUsers", response.data);
          resolve(context.state.items);
        }).catch(reason => {
          reject(reason);
        })
      })
    },
    getUser: function(context: Context, id: number): Promise<User> {
      return new Promise((resolve, reject) => {
        axios.get(`users/${id}`, config(context.rootState.jwt)).then(response => {
          context.commit("setUser", response.data);
          const savedItem = findById<User>(context.state.items, response.data.id);
          if (savedItem)
            resolve(savedItem);
        }).catch(reason => {
          reject(reason);
        })
      })
    },
  }
}
