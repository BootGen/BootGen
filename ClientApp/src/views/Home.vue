<template>
  <v-container fluid>
    <v-card>
      <v-img
        max-width="600"
        src="@/assets/img/bootgen_full.png"
        alt="BootGen logo"
      ></v-img>
      <h1 class="slogan">
        Develop ASP.NET 5 + Vue applications much faster!
      </h1>
      <h2 class="slogan">
        Online project generator for ASP.NET 5 with Vue.js
      </h2>
      <p>
        BootGen generates an ASP.NET 5 project with Vue.js based on the sample JSON data you provide. From this sample data it infers the types you need, and creates entity classes, data services, controllers and Vuex state management that fits your use case.
      </p>
      <div class="d-flex justify-center">
        <iframe width="560" height="315" src="https://www.youtube.com/embed/bdgWl8Ia7u8" title="BootGen tutorial" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
      </div>
      <div class="slogan">
        <v-btn class="ma-2" to="/editor" color="primary"> Try Online </v-btn>
        <v-btn class="ma-2" to="/getting-started" color="primary"> Getting Started </v-btn>
      </div>
    </v-card>
  </v-container>
</template>

<style scoped>
.v-image {
  margin-left: auto;
  margin-right: auto;
  margin-bottom: 100px;
}
.v-card {
  padding-top: 50px;
  padding-bottom: 50px;
  padding-left: 12%;
  padding-right: 12%;
}
.slogan {
  text-align: center;
  margin: 10px;
}
h2 {
  margin-top: 30px;
  margin-bottom: 30px;
}

@media screen and (max-width: 576px) {
  .v-image {
    margin-bottom: 50px;
  }
}
</style>
