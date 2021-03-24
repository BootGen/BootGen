<template>
  <v-container fluid class="cm">
    <codemirror v-if="mode == 'json'" :id="cmId" :value="value" @input="onInput" return-object :options="cmOptions" @scroll="onScroll"  @scrollCursorIntoView="cursorIntoView()" />
    <codemirror v-else :id="cmId" :value="value" :options="cmOptions" @scrollCursorIntoView="cursorIntoView()" />
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import { validateJson } from '../utils/PrettyPrint';
import { codemirror } from 'vue-codemirror';
import 'codemirror/theme/material.css';
import 'codemirror/lib/codemirror.css';
import "codemirror/mode/clike/clike.js";
import "codemirror/mode/yaml/yaml.js";
import "codemirror/mode/vue/vue.js";
import "codemirror/mode/javascript/javascript.js";

export default Vue.extend({
  props: {
    cmId: String,
    value: String,
    mode: String,
    readOnly: Boolean,
    errorLine: Number
  },
  components: {
    codemirror,
  },
  watch: {
    mode: {
      handler(mode: string) {
        if(mode === "json"){
          this.cmOptions.mode = 'text/javascript';
        }if(mode === "cs"){
          this.cmOptions.mode = 'text/x-csharp';
        }else if(mode === "ts"){
          this.cmOptions.mode = 'text/typescript';
        }else if(mode === "vue"){
          this.cmOptions.mode = 'text/x-vue';
        }else{
          this.cmOptions.mode = 'text/x-yaml';
        }
      }
    },
    errorLine: {
      handler(errorLine: number) {
        if(errorLine === -1){
          this.unsetHighlight();
        }else{
          this.highlightLine(errorLine, "red");
        }
      }
    }
  },
  created: function(){
    this.cmOptions.readOnly = this.readOnly;
  },
  data: function () {
    return {
      cmOptions: {
        theme: 'material',
        tabSize: 2,
        mode: 'text/javascript',
        lineNumbers: true,
        line: true,
        readOnly: false,
      },
      minLine: 5000000,
      maxLine: -1,
    }
  },
  methods: {
    onInput: function(content: string) {
      this.$emit('input', content);
    },
    onScroll: function(){
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
      const result = validateJson(this.value);
      if(result.error){
        this.highlightLine(result.line, "red");
        this.$emit("on-error", result.message);
      }else{
        this.$emit("on-error");
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
    cursorIntoView: function(){
      this.$emit("cursor-into-view");
    }
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
