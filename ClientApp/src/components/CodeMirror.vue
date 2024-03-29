<template>
  <v-container fluid class="cm">
    <codemirror v-if="!cmOptions.readOnly" :id="cmId" :value="value" @input="onInput" return-object :options="cmOptions" @scroll="onScroll" />
    <codemirror v-else :id="cmId" :value="value" :options="cmOptions" @scroll="onScroll" ref="vueCm" />
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import { codemirror } from 'vue-codemirror';
import 'codemirror/theme/material.css';
import 'codemirror/lib/codemirror.css';
import 'codemirror/mode/clike/clike.js';
import 'codemirror/mode/yaml/yaml.js';
import 'codemirror/mode/vue/vue.js';
import 'codemirror/mode/javascript/javascript.js';
import 'codemirror/mode/htmlmixed/htmlmixed.js';
import 'codemirror/mode/markdown/markdown.js';
import 'codemirror/mode/shell/shell.js';
import 'codemirror/mode/xml/xml.js';
import 'codemirror/mode/dockerfile/dockerfile.js';

export default Vue.extend({
  props: {
    cmId: String,
    value: String,
    mode: String,
    readOnly: Boolean,
    linesToColor: {
      type: Array as () => Array<{line: number; color: string}>
    },
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
    }
  },
  components: {
    codemirror,
  },
  watch: {
    mode: function (mode: string) {
        if(mode === 'json'){
          this.cmOptions.mode = 'text/javascript';
        }else if(mode === 'cs'){
          this.cmOptions.mode = 'text/x-csharp';
        }else if(mode === 'ts'){
          this.cmOptions.mode = 'text/typescript';
        }else if(mode === 'js'){
          this.cmOptions.mode = 'text/javascript';
        }else if(mode === 'vue'){
          this.cmOptions.mode = 'text/x-vue';
        }else if(mode === 'html'){
          this.cmOptions.mode = 'text/html';
        }else if(mode === 'csproj'){
          this.cmOptions.mode = 'application/xml';
        }else if(mode === 'md'){
          this.cmOptions.mode = 'text/x-markdown';
        }else if(mode === 'sh'){
          this.cmOptions.mode = 'text/x-sh';
        }else if(mode === 'yml'){
          this.cmOptions.mode = 'text/x-yaml';
        }else{
          this.cmOptions.mode = 'text/x-dockerfile';
        }
    },
    linesToColor: function (linesToColor: {line: number; color: string}[]) {
      if(linesToColor.length === 0){
        this.unsetHighlight();
      }else{
        this.highlight();
        if(this.linesToColor.length > 0){
          this.focus();
        }
      }
    },
    readOnly: function(readOnly: boolean){
      this.cmOptions.readOnly = readOnly;
    }
  },
  updated: function() {
    this.unsetHighlight();
    this.highlight();
  },
  created: function(){
    this.cmOptions.readOnly = this.readOnly;
  },
  methods: {
    focus: function() {
      const comp: any = this.$refs?.vueCm;
      const cm: any = comp?.codemirror;
      if(cm){
        cm.focus();
        cm.setCursor(this.linesToColor[0].line, 0);
      }
    },
    onInput: function(content: string) {
      this.$emit('input', content);
      this.$emit('change-json');
      this.unsetHighlight();
    },
    onScroll: function(){
      this.unsetHighlight();
      this.highlight();
    },
    unsetHighlight: function (){
      const elementById = document.getElementById(this.cmId);
      if(!elementById){
        return;
      }
      const e = elementById.getElementsByClassName('CodeMirror-line');
      for(let i = 0; i < e.length; i++){
        e[i].setAttribute('style', 'background-color: unset;');
      }
    },
    highlight: function() {
      const elementById = document.getElementById(this.cmId);
      if (!elementById)
        return;
      const list = elementById.getElementsByClassName('CodeMirror-linenumber');
      for (let i = 0; i < list.length; i++) {
        const lineNumberElement = list[i];
        const textVal = lineNumberElement.textContent;
        if (textVal) {
          const lineNum = parseInt(textVal, 10);
          this.linesToColor.forEach(lineToColor => {
            if (lineToColor.line + 1 === lineNum) {
              const lineElement = lineNumberElement.parentElement?.parentElement?.querySelector('.CodeMirror-line');
              if (lineElement)
                lineElement.setAttribute('style', `background-color:${lineToColor.color};`);
            }
          });
        }
      }
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
