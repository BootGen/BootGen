import Vue from 'vue'
import Vuetify from 'vuetify'
import '@/sass/overrides.sass'

Vue.use(Vuetify)

const lightTheme = {
  primary: '#006FC5',
  secondary: '#209EF0',
  accent: '#209EF0',
  info: '#00CAE3',
  textOrange: '#CE9178',
  textGreen: '#008000',
}

const darkTheme = {
  primary: '#006FC5',
  secondary: '#209EF0',
  accent: '#209EF0',
  info: '#00CAE3',
  white: '#eee',
  textOrange: '#CE9178',
  textGreen: '#6A9955',
}

export default new Vuetify({
  theme: {
    themes: {
      dark: darkTheme,
      light: lightTheme,
    },
  },
})
