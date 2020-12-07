import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store/index'
import vuetify from './plugins/vuetify'
import { User } from './models/User'
import hljs from 'highlight.js'
import 'highlight.js/styles/vs.css'
import './plugins/base'
import './plugins/chartist'
import './plugins/vee-validate'
import i18n from './i18n'

Vue.use(hljs.vuePlugin);
Vue.config.productionTip = false

store.commit('init');

new Vue({
  router,
  store,
  vuetify,
  i18n,
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
