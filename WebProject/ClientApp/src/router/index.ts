import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Login from '../views/Login.vue'
import Logout from '../views/Logout.vue'
import SignUp from '../views/SignUp.vue'
import Generator from '../views/Generator.vue'
import Profile from '../views/Profile.vue'
import ChangePassword from '../views/ChangePassword.vue'
import Index from '@/views/dashboard/Index.vue'
import Dashboard from '@/views/dashboard/Dashboard.vue'
import UserProfile from '@/views/dashboard/pages/UserProfile.vue'
import Notifications from '@/views/dashboard/component/Notifications.vue'
import Icons from '@/views/dashboard/component/Icons.vue'
import Typography from '@/views/dashboard/component/Typography.vue'
import RegularTables from '@/views/dashboard/tables/RegularTables.vue'
import GoogleMaps from '@/views/dashboard/maps/GoogleMaps.vue'
import Upgrade from '@/views/dashboard/Upgrade.vue'

import store from "../store/index"

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
    path: "/logout",
    name: "Logout",
    beforeEnter: loggedInGuard,
    component: Logout
  },
  {
    path: "/index",
    name: "Index",
    component: Index,
    children: [
      {
        path: "/dashboard",
        name: "Dashboard",
        component: Dashboard,
      },
      {
        path: "/user-profile",
        name: "User Profile",
        component: UserProfile,
      },
      {
        path: "/notifications",
        name: "Notifications",
        component: Notifications,
      },
      {
        path: "/icons",
        name: "Icons",
        component: Icons,
      },
      {
        path: "/typography",
        name: "Typography",
        component: Typography,
      },
      {
        path: "/regular-tables",
        name: "Regular Tables",
        component: RegularTables,
      },
      {
        path: "/google-maps",
        name: "Google Maps",
        component: GoogleMaps,
      },
      {
        path: "/upgrade",
        name: "Upgrade",
        component: Upgrade,
      },
    ]
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
