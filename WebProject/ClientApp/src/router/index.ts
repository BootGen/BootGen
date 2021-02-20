import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Login from '../views/Login.vue'
import Logout from '../views/Logout.vue'
import SignUp from '../views/SignUp.vue'
import Editor from '../views/Editor.vue'
import Profile from '../views/Profile.vue'
import ChangePassword from '../views/ChangePassword.vue'

import store from "../store/index"

Vue.use(VueRouter)

const loggedInGuard = (to: any, from: any, next: any) => {
  if(store.state.auth.jwt){
    next();
  }else{
    next('/login');
  }
};

const loggedOutGuard = (to: any, from: any, next: any) => {
  if(store.state.auth.jwt){
    next('/profile');
  }else{
    next();
  }
};
  const routes: Array<RouteConfig> = [
  {
    path: '/login',
    name: 'Login',
    beforeEnter: loggedOutGuard,
    component: Login
  },
  {
    path: '/sign-up',
    name: 'Sign Up',
    beforeEnter: loggedOutGuard,
    component: SignUp
  },
  {
    path: "/",
    name: "Editor",
    component: Editor
  },
  {
    path: "/profile",
    name: "Profile",
    beforeEnter: loggedInGuard,
    component: Profile
  },
  {
    path: "/change-password",
    name: "Change Password",
    beforeEnter: loggedInGuard,
    component: ChangePassword
  },
  {
    path: "/logout",
    name: "Logout",
    beforeEnter: loggedInGuard,
    component: Logout
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
