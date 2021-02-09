<template>
	<div>
		<v-treeview class="text-break" active-class="info--text" :items="tree.children" @update:active="selectFile" return-object activatable hoverable dense open-on-click></v-treeview>
	</div>
</template>

<script lang="ts">
import Vue from "vue";
import { GeneratedFile } from "../models/GeneratedFile";

interface Node {
	id: number;
	name: string;
	children?: Array<Node>;
}

export default Vue.extend({
	props: {
		files: {
			type: Array as () => GeneratedFile[]
		},
	},
  data: function () {
    return {
		tree: {id: 0, name: "Root", children: []},
		filesById: Array<GeneratedFile>(),
		id: 0,
    };
	},
	created: function (){
		this.files.forEach(file => this.addToTree(file));
	},
  methods: {
		getChild: function(node: Node, name: string): Node | null{
			if(!node.children){
				return null;
			}
			for(let j = 0; j < node.children.length; j++){
				if(node.children[j] && node.children[j].name === name){
					return node.children[j];
				}
			}
			return null;
		},
		addToTree: function(file: GeneratedFile){
			const path = file.path.split('/');
			let currentNode: Node = this.tree;
			if(path[0] !== ""){
				path.forEach(part => {
					const childNode = this.getChild(currentNode, part);
					if(!childNode){
						const node: Node = {id: ++this.id, name: part};
						if(!currentNode.children){
							currentNode.children = [node];
						}else{
							currentNode.children.push(node);
						}
						currentNode = node;
					}else{
						currentNode = childNode;
					}
				});
			}
			if(!currentNode.children){
				currentNode.children = [{id: ++this.id, name: file.name}];
			}else{
				currentNode.children.push({id: ++this.id, name: file.name});
			}
			this.filesById[this.id] = file;
		},
		getFile: function(node: Node){
			return this.filesById[node.id];
		},
		selectFile: function (node: Node[]){
			if(node[0]){
				this.$emit("select-file", this.getFile(node[0]));
			}
		},
  },
});
</script>
