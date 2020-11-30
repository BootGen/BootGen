import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify'
import { User } from './models/User'
import hljs from 'highlight.js';
import 'highlight.js/styles/vs.css';

Vue.use(hljs.vuePlugin);
Vue.config.productionTip = false

store.commit('init');

new Vue({
  router,
  store,
  vuetify,
  mounted: async function() {
    if(store.state.jwt){
      this.$root.$data.user = await this.$store.dispatch("profile");
    }
  },
  data: function () {
    return {
      user: null as (User | null)
    };
  },
  render: h => h(App)
}).$mount('#app')
