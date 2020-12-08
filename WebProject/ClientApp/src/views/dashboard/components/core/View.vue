<template>
  <v-container>
    <router-view />

    <dashboard-core-footer />
  </v-container>
</template>

<script>
  export default {
    name: 'DashboardCoreView',

    components: {
      DashboardCoreFooter: () => import('./Footer'),
    },
  }
</script>
