import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Login from '../views/Login.vue'
import Logout from '../views/Logout.vue'
import SignUp from '../views/SignUp.vue'
import Activation from '../views/Activation.vue'
import PrivacyStatement from '../views/PrivacyStatement.vue'
import Editor from '../views/Editor.vue'
import SavedProjects from '../views/SavedProjects.vue'
import Profile from '../views/Profile.vue'
import ChangePassword from '../views/ChangePassword.vue'

import store from '../store/index'
import {NavigationGuardNext, Route} from 'vue-router/types/router';

Vue.use(VueRouter)

const loggedInGuard = (to: Route, from: Route, next: NavigationGuardNext) => {
  if(store.state.auth.jwt){
    next();
  }else{
    next('/login');
  }
};

const loggedOutGuard = (to: Route, from: Route, next: NavigationGuardNext) => {
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
    path: '/activation/:activationToken',
    name: 'Activation',
    beforeEnter: loggedOutGuard,
    component: Activation
  },
  {
    path: '/privacy-statement',
    name: 'Privacy Statement',
    component: PrivacyStatement
  },
  {
    path: '/',
    name: 'Editor',
    component: Editor
  },
  {
    path: '/saved-projects',
    name: 'Saved Projects',
    beforeEnter: loggedInGuard,
    component: SavedProjects
  },
  {
    path: '/profile',
    name: 'Profile',
    beforeEnter: loggedInGuard,
    component: Profile
  },
  {
    path: '/change-password',
    name: 'Change Password',
    beforeEnter: loggedInGuard,
    component: ChangePassword
  },
  {
    path: '/logout',
    name: 'Logout',
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
