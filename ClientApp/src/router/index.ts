import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Login from '../views/Login.vue'
import GithubLoginCallback from '../views/GithubLoginCallback.vue'
import Logout from '../views/Logout.vue'
import SignUp from '../views/SignUp.vue'
import Activation from '../views/Activation.vue'
import PrivacyPolicy from '../views/PrivacyPolicy.vue'
import Editor from '../views/Editor.vue'
import SavedProjects from '../views/SavedProjects.vue'
import GettingStarted from '../views/GettingStarted.vue'
import Home from '../views/Home.vue'
import Profile from '../views/Profile.vue'
import ChangePassword from '../views/ChangePassword.vue'
import NotFound from '../views/NotFound.vue'

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
    path: '/privacy-policy',
    name: 'Privacy Policy',
    component: PrivacyPolicy
  },
  {
    path: '/editor',
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
    path: '/getting-started',
    name: 'Getting Started',
    component: GettingStarted
  },
  {
    path: '/',
    name: 'Home',
    component: Home
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
  },
  {
    path: '/oauth',
    component: {
      render: (c) => c('router-view')
    },
    children: [
      {
        path: 'github/callback',
        name: 'Redirecting ...',
        component: GithubLoginCallback,
        props: (route) => ({code: route.query.code})
      }
    ]
  },
  {
    path: '*',
    name: 'Not Found',
    component: NotFound
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

router.beforeEach((to: Route, from: Route, next: NavigationGuardNext) => {
  document.title = `BootGen - ${to.name}`;
  next();
});

export default router
