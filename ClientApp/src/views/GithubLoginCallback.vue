<template>
 <div>
 </div>
</template>

<script lang="ts">
import Vue from 'vue';

export default Vue.extend({
  props: ['code'],

  async mounted() {
    await this.$store.dispatch('githubLogin', {code: this.$props.code});
    await this.$router.replace({name: 'Profile'});
  }
});
</script>
