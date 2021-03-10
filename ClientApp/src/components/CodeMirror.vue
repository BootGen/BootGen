<template>
  <v-container fluid class="cm">
    <codemirror :id="cmId" v-model="activeProject.json" :options="cmOptions" @scroll="onScroll"/>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { jsonError } from '../utils/PrettyPrint';
import { codemirror } from 'vue-codemirror'
import 'codemirror/lib/codemirror.css'
import "codemirror/mode/javascript/javascript.js";
import 'codemirror/theme/material.css';
import { Project } from "../models/Project";

export default Vue.extend({
  props: {
    cmId: String,
    activeProject: {
			type: Object as () => Project
		},
    error: {
      type: Object as () => { line: number; message: string }
    }
  },
  components: {
    codemirror,
  },
  watch: {
    error: {
      handler(error: {line: number; message: string}) {
        if(error.line === -1){
          this.unsetHighlight();
        }else{
          this.highlightLine(error.line, "red");
        }
      }
    }
  },
  data: function () {
    return {
      cmOptions: {
        theme: 'material',
        tabSize: 2,
        mode: 'text/javascript',
        lineNumbers: true,
        line: true,
      },
      minLine: 5000000,
      maxLine: -1,
    }
  },
  methods: {
    onScroll: function() {
      this.unsetHighlight();
      const elementById = document.getElementById(this.cmId);
      if(!elementById){
        return;
      }
      const list = elementById.getElementsByClassName("CodeMirror-linenumber");
      this.minLine = 5000000;
      this.maxLine = -1;
      for (let i = 1; i < list.length; i++) {
        const textval = list[i].textContent;
        if (textval) {
          const lineNum = parseInt(textval, 10);
          if (lineNum < this.minLine)
            this.minLine = lineNum;
          if (lineNum > this.maxLine)
            this.maxLine = lineNum;
        }
      }
      const error = jsonError(this.activeProject.json);
      if(error !== false){
        this.highlightLine(error.line, "red");
        this.$emit("set-snackbar", "orange darken-2", error.message, true, -1);
      }else{
        this.$emit("set-snackbar");
      }
    },
    unsetHighlight: function (){
      const elementById = document.getElementById(this.cmId);
      if(!elementById){
        return;
      }
      const e = elementById.getElementsByClassName("CodeMirror-line");
      for(let i = 0; i < e.length; i++){
        e[i].setAttribute("style", "background-color: unset;");
      }
    },
    highlightLine: function (line: number, color: string){
      this.unsetHighlight();
      const elementById = document.getElementById(this.cmId);
      if(!elementById || (this.minLine > line && this.minLine < 5000000) || (this.maxLine < line && this.maxLine > -1)){
        return;
      }
      if(this.minLine < line && line < this.maxLine){
        elementById.getElementsByClassName("CodeMirror-line")[line-this.minLine+2].setAttribute("style", `background-color:${color};`);
      }else{
        elementById.getElementsByClassName("CodeMirror-line")[line].setAttribute("style", `background-color:${color};`);
      }
    },
  },
});
</script>

<style lang="css">
  .cm{
    margin: 0!important;
    padding: 0!important;
    position: relative;
    height: 100%;
  }
  .vue-codemirror{
    width: 100%;
    height: calc(100% - 40px);
    margin-bottom: 0!important;
    padding-bottom: 0!important;
  }
  .CodeMirror-scroll{
    overflow: auto!important;
    margin: 0!important;
    padding: 0!important;
  }
  .CodeMirror{
    height: 100%;
    z-index: 0;
  }
</style>