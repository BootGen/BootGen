<template>
  <v-container id="privacy-statement" fluid tag="section">
    <v-row justify="center">
      <v-col cols="12">
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Quidem nam architecto, molestiae dolorem eaque facere. Inventore quibusdam quidem eveniet! Fugit quibusdam voluptate temporibus earum fugiat ex quaerat! Sequi, fugiat nesciunt.</p>
        <div class="d-flex align-center">
          <v-checkbox v-model="cookiesAccepted" @click="setOpt"></v-checkbox>
          <span>Accept Google Analytics cookies</span>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";

export default Vue.extend({
  data: function () {
    return {
      cookiesAccepted: false,
    };
  },
  methods: {
    setOpt: function(){
      if(this.cookiesAccepted){
        this.$gtag.optIn();
        console.log(this.$gtag);
      }else{
        this.$gtag.optOut();
        console.log(this.$gtag);
      }
    }
  },
});
</script>