<template>
  <base-material-generator-card>
    <template v-slot:heading>
      <div class="d-flex align-center justify-space-between pa-2">
        <div class="display-1 font-weight-light pa-2">Generated Files</div>
      </div>
    </template>
    <tree-view
      class="treeView"
      v-if="generatedFiles.length > 0"
      :files="generatedFiles"
      :previousFiles="previousFiles"
      :openPath="openPath"
      :isCompare="isCompare"
      @select-file="selectFile"
    ></tree-view>
  </base-material-generator-card>
</template>

<script lang="ts">
import Vue from 'vue';
import TreeView from './TreeView.vue';
import { GeneratedFile } from '../models/GeneratedFile';

export default Vue.extend({
  components: {
    TreeView,
  },
  props: {
    generatedFiles: {
      type: Array as () => GeneratedFile[],
    },
    previousFiles: {
      type: Array as () => GeneratedFile[],
    },
    openPath: String,
    isCompare: Boolean,
  },
  data: function () {
    return {};
  },
  methods: {
    selectFile: function (data: GeneratedFile) {
      this.$gtag?.event('select-file');
      this.$emit('select-file', data);
    },
  },
});
</script>

<style>
.treeView {
  height: calc(100% - 40px);
  overflow: auto;
}
.v-treeview-node__label {
  overflow: unset !important;
}
</style>