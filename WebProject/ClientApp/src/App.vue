<template>
  <v-app id="inspire">
    <v-navigation-drawer 
      v-model="drawer"
      :dark="barColor !== 'rgba(228, 226, 226, 1), rgba(255, 255, 255, 0.7)'"
      :src="barImage"
      app>
      <template v-slot:img="props">
        <v-img
          :gradient="`to bottom, ${barColor}`"
          v-bind="props"
        />
      </template>
      <v-divider class="mb-1" />
    
      <v-list dense nav>
        <div v-for="item in items" :key="item.title" link>
          <div v-if="item.children">
            <v-list-group :value="true" :prepend-icon="item.icon">
              <template v-slot:activator>
                <v-list-item-content>
                  <v-list-item-title>{{ item.title }}</v-list-item-title>
                </v-list-item-content>
              </template>

              <v-list-item v-for="(child, i) in item.children" :key="i" :to="child.link" link class="ml-14">
                <v-list-item-title>{{ child.title }}</v-list-item-title>
                <v-list-item-icon>
                  <v-icon>{{ child.icon }}</v-icon>
                </v-list-item-icon>
              </v-list-item>
            </v-list-group>
          </div>
          
          <v-list-item v-else :to="item.link" link>
            <v-list-item-icon>
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-icon>
            <v-list-item-title>{{ item.title }}</v-list-item-title>
          </v-list-item>
        </div>
      </v-list>
    </v-navigation-drawer>

    <v-app-bar app>
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>

      <v-toolbar-title>{{ $route.name }}</v-toolbar-title>
    </v-app-bar>

    <v-main>
      <!-- Provides the application the proper gutter -->
      <v-container fluid>
        <!-- If using vue-router -->
        <router-view></router-view>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
 import { mapState } from 'vuex'

export default {
  computed: {
    ...mapState(['barColor', 'barImage']),
    drawer: {
      get () {
        return this.$store.state.drawer
      },
      set (val) {
        this.$store.commit('SET_DRAWER', val)
      },
    },
    items: function () {
      if (this.$store.state.jwt) {
        return [
          { title: "Generator", link: "/generator", icon: "mdi-cog-outline" },
          { title: "Profile", link: "/profile", icon: "mdi-account-outline" },
          {
            title: "Edit Profile",
            link: "/edit-profile",
            icon: "mdi-account-edit-outline",
          },
          {
            title: "Change Password",
            link: "/change-password",
            icon: "mdi-form-textbox-password",
          },
          { title: "Logout", link: "/logout", icon: "mdi-account-arrow-right-outline" },
        ];
      } else {
        return [
          { title: "Login", link: "/", icon: "mdi-account-arrow-left-outline" },
          {
            title: "Sign Up",
            link: "sign-up",
            icon: "mdi-account-plus-outline",
          },
          { title: "Generator", link: "/generator", icon: "mdi-cog-outline" },
          { title: "Vuetify MD", link: "/index", icon: "mdi-vuetify", children: [
            { title: "Dashboard", link: "/dashboard", icon: "mdi-view-dashboard-outline" },
            { title: "User Profile", link: "/user-profile", icon: "mdi-account-outline" },
            { title: "Regular Tables", link: "/regular-tables", icon: "mdi-clipboard-outline" },
            { title: "Typography", link: "/typography", icon: "mdi-format-font" },
            { title: "Icons", link: "/icons", icon: "mdi-chart-bubble" },
            { title: "Google Maps", link: "/google-maps", icon: "mdi-map-marker" },
            { title: "Notifications", link: "/notifications", icon: "mdi-bell-outline" },
            { title: "Upgrade To PRO", link: "/upgrade", icon: "mdi-package-up" },
          ]},
        ];
      }
    },
  },
};
</script>