import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Login from '../views/Login.vue'
import Logout from '../views/Logout.vue'
import SignUp from '../views/SignUp.vue'
import Generator from '../views/Generator.vue'
import Profile from '../views/Profile.vue'
import ChangePassword from '../views/ChangePassword.vue'
import EditProfile from '../views/EditProfile.vue'
import store from "../store"

Vue.use(VueRouter)

const loggedInGuard = (to: any, from: any, next: any) => {
  if(store.state.jwt){
    next();
  }else{
    next('/');
  }
};

const loggedOutGuard = (to: any, from: any, next: any) => {
  if(store.state.jwt){
    next('/profile');
  }else{
    next();
  }
};
  const routes: Array<RouteConfig> = [
  {
    path: '/',
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
    path: "/generator",
    name: "Generator",
    component: Generator
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
    path: "/edit-profile",
    name: "Edit Profile",
    beforeEnter: loggedInGuard,
    component: EditProfile
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
