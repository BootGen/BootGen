<template>
  <v-container>
    <dashboard-core-view />

    <dashboard-core-settings />
  </v-container>
</template>

<script>
  export default {
    name: 'DashboardIndex',
    components: {
      DashboardCoreSettings: () => import('./components/core/Settings'),
      DashboardCoreView: () => import('./components/core/View'),
    },

    data: () => ({
      expandOnHover: false,
    }),
  }
</script>
