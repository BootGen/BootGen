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
      <div class="ma-4">
        <div class="container-fluid d-flex flex-column navCookie" v-if="!cookieConsentAnswered">
          <div class="container pl-0 align-center">
            <v-icon class="pr-2" dark large>{{ snackbar.icon }}</v-icon>
            <span>{{ snackbar.text }}</span>
          </div>
          <div v-if="snackbar.buttons" class="d-flex justify-space-around pb-5">
            <v-btn color="primary" @click="acceptCookies()" small>accept</v-btn>
            <v-btn color="primary" @click="customizeCookies()" small>customize</v-btn>
          </div>
        </div>
        <v-switch
          v-model="darkTheme"
          label="Dark Mode"
        ></v-switch>
        <a class="repositoryLink" href="https://github.com/BootGen/Editor" target="_blank" @click="openGithub()">
          <v-icon class="pr-2">mdi-github</v-icon>
          github.com/BootGen/Editor
        </a>
      </div>
    </v-navigation-drawer>

    <v-app-bar app color="transparent" flat height="75" class="navPosition" :data-value="$route.name !== 'Editor'">
      <v-btn class="mr-3" @click="setNavDraver()" elevation="1" fab small>
        <v-icon v-if="drawer">mdi-dots-vertical</v-icon>
        <v-icon v-else>mdi-view-quilt</v-icon>
      </v-btn>
      <v-toolbar-title class="hidden-sm-and-down font-weight-light" v-if="$route.name !== 'Editor'">{{ $route.name }}</v-toolbar-title>
    </v-app-bar>

    <v-main>
      <v-container fluid>
        <router-view></router-view>
        <footer><router-link to="/privacy-policy">Privacy Policy</router-link> | Created by <a href="https://codesharp.hu" target="_blank">Code Sharp Kft.</a></footer>
      </v-container>
    </v-main>
    <snackbar v-if="!cookieConsentAnswered && !drawer" :snackbar="snackbar" @accept="acceptCookies" @customize="customizeCookies"></snackbar>
  </v-app>
</template>

<script>
import { mapState } from 'vuex'
import Snackbar from './components/Snackbar.vue';

export default {
  data: () => ({
    drawer: null,
    snackbar: {
      dismissible: false,
      visible: true,
      type: 'third',
      icon: 'mdi-cookie-alert',
      text: 'We use cookies to improve your experience on our website.',
      timeout: -1,
      buttons: [
        {name: 'accept', color: 'secondary'},
        {name: 'customize', color: 'secondary'}
      ],
    },
    cookieConsentAnswered: false,
    cookiesAccepted: false,
    darkTheme: false
  }),
  components: {
    Snackbar,
  },
  mounted(){
    if(localStorage.cookieConsentAnswered) this.cookieConsentAnswered = localStorage.cookieConsentAnswered;
    if(localStorage.cookiesAccepted) this.cookiesAccepted = localStorage.cookiesAccepted;
    if(localStorage.darkTheme === 'true'){
      this.darkTheme = true;
    }else{
      this.darkTheme = false;
    }
  },
  watch:{
    cookieConsentAnswered(answered) {
      localStorage.cookieConsentAnswered = answered;
    },
    cookiesAccepted(accepted) {
      localStorage.cookiesAccepted = accepted;
    },
    darkTheme(dark){
      this.$gtag?.event('change-theme');
      localStorage.darkTheme = dark;
      this.$vuetify.theme.dark = dark;
    }
  },
  computed: {
    ...mapState(['barColor', 'barImage']),
    items: function () {
      const menuItems = [
          { title: 'Getting Started', link: '/', icon: 'mdi-rocket-launch' },
          { title: 'Editor', link: '/editor', icon: 'mdi-cog' }
      ];
      if (this.$store.state.auth.jwt) {
        return menuItems.concat([
          { title: 'Saved Projects', link: '/saved-projects', icon: 'mdi-file-multiple' },
          { title: 'Profile', link: '/profile', icon: 'mdi-account' },
          { title: 'Change Password', link: '/change-password', icon: 'mdi-form-textbox-password' },
          { title: 'Logout', link: '/logout', icon: 'mdi-account-arrow-right' }
        ]);
      } else {
        return menuItems.concat([
          { title: 'Login', link: '/login', icon: 'mdi-account-arrow-left' },
          { title: 'Sign Up', link: 'sign-up', icon: 'mdi-account-plus' }
        ]);
      }
    },
  },
  methods: {
    acceptCookies: function(){
      this.$gtag?.event('accept-cookies');
      this.cookieConsentAnswered = true;
      this.cookiesAccepted = true;
    },
    customizeCookies: function(){
      this.$gtag?.event('customize-cookies');
      this.cookieConsentAnswered = true;
      this.$router.push('/privacy-policy');
    },
    setNavDraver: function(){
      this.$gtag?.event('toggle-drawer');
      this.drawer = !this.drawer;
    },
    openGithub: function(){
      this.$gtag?.event('open-github');
    }
  }
};
</script>

<style lang="css">
  .theme--light .v-application--wrap{
    background-color: #eee;
  }
  .theme--dark .v-application--wrap{
    background-color: #363636;
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
    color: #eee;
  }
  a.repositoryLink{
    color: #eee!important;
    text-decoration: none;
  }
  a.repositoryLink:hover{
    color: #999 !important;
  }
  footer {
    text-align: center;
  }
</style>