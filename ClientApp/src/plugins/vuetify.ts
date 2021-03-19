import Vue from 'vue'
import en from 'vuetify'
import Vuetify from 'vuetify/lib'
import '@/sass/overrides.sass'

Vue.use(Vuetify)

const theme = {
  primary: '#22195B',
  secondary: '#412fb3',
  accent: '#412fb3',
  info: '#00CAE3',
}

export default new Vuetify({
  theme: {
    themes: {
      dark: theme,
      light: theme,
    },
  },
})
