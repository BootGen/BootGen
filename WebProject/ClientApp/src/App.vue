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
    
      <v-list nav>
        <div v-for="item in items" :key="item.title" link>
          <div v-if="item.children">
            <v-list-group active-class="primary white--text" :prepend-icon="item.icon">
              <template v-slot:activator>
                <v-list-item-content>
                  <v-list-item-title>{{ item.title }}</v-list-item-title>
                </v-list-item-content>
              </template>

              <v-list-item active-class="primary white--text" v-for="child in item.children" :key="child.title" :to="child.link" link class="ml-14">
                <v-list-item-title>{{ child.title }}</v-list-item-title>
                <v-list-item-icon>
                  <v-icon>{{ child.icon }}</v-icon>
                </v-list-item-icon>
              </v-list-item>
            </v-list-group>
          </div>
          
          <v-list-item v-else active-class="primary white--text" class="mb-1" :to="item.link" link>
            <v-list-item-icon>
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-icon>
            <v-list-item-title>{{ item.title }}</v-list-item-title>
          </v-list-item>
        </div>
      </v-list>
    </v-navigation-drawer>

    <v-app-bar app color="transparent" flat height="75">
      <v-btn class="mr-3" @click="drawer = !drawer" elevation="1" fab small>
        <v-icon v-if="drawer">mdi-dots-vertical</v-icon>
        <v-icon v-else>mdi-view-quilt</v-icon>
      </v-btn>
      <v-toolbar-title class="hidden-sm-and-down font-weight-light">{{ $route.name }}</v-toolbar-title>
      
      <v-spacer></v-spacer>
      
      <v-btn class="ml-2" v-if="this.$store.state.jwt" min-width="0" text to="/profile">
        <v-icon>mdi-account</v-icon>
      </v-btn>
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
    items: function () {
      if (this.$store.state.jwt) {
        return [
          { title: "Profile", link: "/profile", icon: "mdi-account-outline" },
          {
            title: "Change Password",
            link: "/change-password",
            icon: "mdi-form-textbox-password",
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
  data: () => ({
    drawer: null,
  }),
};
</script>