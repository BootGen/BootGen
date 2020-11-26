<template>
  <v-container fluid>
    <v-row>
      <v-col cols="8">
        <p v-if="activeFile && activeFile.content">{{ activeFile.content }}</p>
      </v-col>
      <v-col cols="4">
        <v-treeview :items="tree" @update:active="treeClick" return-object activatable hoverable dense open-on-click></v-treeview>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";

interface Element {
  id: number;
  name: string;
  content?: string;
  children: Element[];
}

export default Vue.extend({
  props:[
    "files"
  ],
  components: {
  },
  computed: {
    tree: function(): Element[]{
      let data: Element[] = [];
      let id = 0;
      for(const i in this.files){
        data = this.placement(data, this.files[i], id);
        id += 2;
      }
      return data;
    }
  },
  data: function () {
    return {
      activeFile: null as (null | Element),
    };
  },
  methods: {
    treeClick: function(data: Element[]){
      if(data[0] && data[0].content){
        this.activeFile = data[0];
      }
    },
    placement: function(element: Element[], file: {name: string; path: string; content: string}, id: number): Element[]{
      let result = false;
      if(file.path === ""){
        element.push({id: id++, name: file.name, content: file.content, children: []});
      }else{
        for(const i in element){
          if(element[i].name === file.path){
            element[i].children.push({id: id++, name: file.name, content: file.content, children: []});
            result = true;
            break;
          }else if(element[i].children.length > 0){
            result = true
            break;
          }
        }
        if(!result){
          element.push({id: id++, name: file.path, children: [{id: id++, name: file.name, content: file.content, children: []}]})
        }
      }
      return element;
    }
  },
});
</script>
