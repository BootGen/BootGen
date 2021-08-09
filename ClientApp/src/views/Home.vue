<template>
  <v-container fluid>
    <v-card>
      <v-img
        max-width="600"
        src="@/assets/img/bootgen_full.png"
        alt="BootGen logo"
      ></v-img>
      <h1 class="slogan">
        Starter Project Generator
      </h1> 
      <h2 class="slogan">
        Develop ASP.NET 5 + Vue 3 applications much faster!
      </h2>
      <p>
        BootGen generates a well coded, human readable, ASP.NET 5 project with Vue 3 based on the sample JSON data you provide. From this sample data it infers the types you need, and creates entity classes, data services, controllers and Vuex state management that fits your use case. 
      </p>
      <div class="d-flex justify-center video">
        <iframe width="560" height="315" src="https://www.youtube.com/embed/0YmlGPWXDv0" title="BootGen tutorial" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
      </div>
      <p>Create sample JSON data, and generate your application. Edit the JSON, and regenerate until you are satisfied with the result. Changes will be highlighted in the generated files.</p>
      <p>When you are finnished click download, and start coding!</p>
      <p>We generate code for you, not just the compiler. We intend to create code that is easy to read, understand and edit.</p>
      <div class="slogan">
        <v-btn class="ma-2" to="/editor" color="primary"> Try Online </v-btn>
        <v-btn class="ma-2" to="/advanced-features" color="primary"> Advanced Features </v-btn>
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
.video {
  margin-top: 30px;
  margin-bottom: 30px;
}

@media screen and (max-width: 576px) {
  .v-image {
    margin-bottom: 50px;
  }
}
</style>
