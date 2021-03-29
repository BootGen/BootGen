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
      <div class="container-fluid d-flex flex-column navCookie" v-if="!cookiesAccepted">
        <div class="container">
          <v-icon class="pr-2" dark large>{{ snackbar.icon }}</v-icon>
          <span>{{ snackbar.text }}</span>
        </div>
        <div v-if="snackbar.buttons" class="d-flex justify-space-between pa-2">
          <v-btn color="secondary" @click="acceptCookies()">accept</v-btn>
          <v-btn color="secondary" @click="customizeCookies()">customize</v-btn>
        </div>
      </div>
    </v-navigation-drawer>

    <v-app-bar app color="transparent" flat height="75" class="navPosition" :data-value="$route.name !== 'Editor'">
      <v-btn class="mr-3" @click="drawer = !drawer" elevation="1" fab small>
        <v-icon v-if="drawer">mdi-dots-vertical</v-icon>
        <v-icon v-else>mdi-view-quilt</v-icon>
      </v-btn>
      <v-toolbar-title class="hidden-sm-and-down font-weight-light" v-if="$route.name !== 'Editor'">{{ $route.name }}</v-toolbar-title>
    </v-app-bar>

    <v-main>
      <!-- Provides the application the proper gutter -->
      <v-container fluid>
        <!-- If using vue-router -->
        <router-view></router-view>
      </v-container>
    </v-main>
    <snackbar v-if="!cookiesAccepted && !drawer" :snackbar="snackbar" @accept="acceptCookies" @customize="customizeCookies"></snackbar>
  </v-app>
</template>

<script>
import { mapState } from 'vuex'
import Snackbar from "./components/Snackbar.vue";

export default {
  components: {
    Snackbar,
  },
  mounted() {
    if(localStorage.cookieConsentAnswered) this.cookieConsentAnswered = localStorage.cookieConsentAnswered;
    if(localStorage.cookiesAccepted) this.cookiesAccepted = localStorage.cookiesAccepted;
  },
  watch:{
    cookieConsentAnswered(answered) {
      localStorage.cookieConsentAnswered = answered;
    },
    cookiesAccepted(accepted) {
      localStorage.cookiesAccepted = accepted;
    }
  },
  computed: {
    ...mapState(['barColor', 'barImage']),
    items: function () {
      if (this.$store.state.auth.jwt) {
        return [
          { title: "Editor", link: "/", icon: "mdi-cog" },
          { title: "Profile", link: "/profile", icon: "mdi-account" },
          {
            title: "Change Password",
            link: "/change-password",
            icon: "mdi-form-textbox-password",
          },
          { title: "Logout", link: "/logout", icon: "mdi-account-arrow-right" }
        ];
      } else {
        return [
          { title: "Editor", link: "/", icon: "mdi-cog" },
          { title: "Login", link: "/login", icon: "mdi-account-arrow-left" },
          {
            title: "Sign Up",
            link: "sign-up",
            icon: "mdi-account-plus",
          }
        ];
      }
    },
  },
  data: () => ({
    drawer: null,
    snackbar: {
      dismissible: true,
      visible: true,
      type: "third",
      icon: "mdi-cookie-alert",
      text: "We use cookies to improve your experience on our website.",
      timeout: -1,
      buttons: [
        {name: "accept", color: "secondary"},
        {name: "customize", color: "secondary"}
      ]
    },
    cookieConsentAnswered: false,
    cookiesAccepted: false,
  }),
  methods: {
    acceptCookies: function(){
      this.cookieConsentAnswered = true;
      this.cookiesAccepted = true
    },
    customizeCookies: function(){
      this.cookieConsentAnswered = true;
      this.$router.push('/privacy-statement');
    }
  }
};
</script>

<style lang="css">
  .v-application--wrap{
    background-color: #ededed;
  }
  .v-application--wrap nav{
    z-index: 999;
  }
  .v-navigation-drawer__content{
    display: flex;
    flex-direction: column;
    justify-content: space-between;
  }
  main{
    padding-top: unset!important;
  }
  .navPosition[data-value=true]{
    position: inherit!important;
  }
  .navPosition[data-value=false]{
    position: absolute!important;
  }
  .navCookie{
    color: #fff;
  }
</style>