import Vue from 'vue'
import Vuetify from 'vuetify'
import '@/sass/overrides.sass'

Vue.use(Vuetify)

const theme = {
  primary: '#22195B',
  secondary: '#412fb3',
  third: '#22195b',
  accent: '#412fb3',
  info: '#00CAE3',
  orange: '#F78C6C',
  green: '#C3E88D',
}

export default new Vuetify({
  theme: {
    themes: {
      dark: theme,
      light: theme,
    },
  },
})
