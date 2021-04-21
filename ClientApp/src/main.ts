import Vue from 'vue'
import App from './App.vue'
import router from './router/index'
import store from './store/index'
import vuetify from './plugins/vuetify'
import './plugins/vee-validate'
import VueGtag from 'vue-gtag';
import axios from 'axios'
import { AppError } from './models/AppError'
import MaterialCard from './components/base/MaterialCard.vue'
import MaterialGeneratorCard from './components/base/MaterialGeneratorCard.vue'

Vue.component('base-material-card', MaterialCard)
Vue.component('base-material-generator-card', MaterialGeneratorCard)

if (process.env.NODE_ENV === 'production') {
  Vue.use(VueGtag, {
    config: {
      id: 'G-X2LQJGC8QE'
    },
  }, router);
}

Vue.config.productionTip = false

store.dispatch('init');

new Vue({
  router,
  store,
  vuetify,
  mounted: async function () {
    if (store.state.auth.jwt) {
      try {
        await this.$store.dispatch('profile');
        await this.$store.dispatch('projects/getProjects');
      } catch {
        this.$store.commit('setJwt', null);
        await this.$router.push('/login');
      }
    }
  },
  data: function () {
    return {
      baseUrl: process.env.VUE_APP_BASE_URL
    }
  },
  render: h => h(App)
}).$mount('#app');


Vue.config.errorHandler = (error: Error, vm: Vue, info: string) => {
  console.error(error);
  const err: AppError = {
    kind: 'Vue',
    type: error?.name,
    stackTrace: error?.stack,
    message: error?.message,
    info: info
  }
  axios.post('errors/log', err);
};

window.onerror = function(event: Event | string, source?: string, lineno?: number, colno?: number, error?: Error) {
  console.error(error);
  const err: AppError = {
    kind: 'JavaScript',
    type: error?.name,
    lineNumber: lineno,
    columnNumber: colno,
    stackTrace: error?.stack,
    message: error?.message
  }
  axios.post('errors/log', err);
 };
