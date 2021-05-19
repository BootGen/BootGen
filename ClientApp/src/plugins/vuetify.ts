import Vue from 'vue'
import Vuetify from 'vuetify'
import '@/sass/overrides.sass'

Vue.use(Vuetify)

const lightTheme = {
  primary: '#00A170',
  secondary: '#656867',
  third: '#00A170',
  accent: '#656867',
  info: '#00CAE3',
  orange: '#F78C6C',
  green: '#C3E88D',
  purple: '#C792EA',
}

const darkTheme = {
  primary: '#00A170',
  secondary: '#656867',
  third: '#00A170',
  accent: '#656867',
  info: '#00CAE3',
  white: '#eee',
  orange: '#F78C6C',
  green: '#C3E88D',
  purple: '#C792EA',
}

export default new Vuetify({
  theme: {
    themes: {
      dark: darkTheme,
      light: lightTheme,
    },
  },
})
