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
		filesById: Array<{id: number; file: GeneratedFile}>(),
		id: 0,
    };
	},
	created: async function (){
		this.setTree();
	},
  methods: {
		setTree: function (){
			this.files.forEach(file => this.addToTree(file));
			this.addToFile(this.tree);
			this.removeEmptyNode(this.tree);
		},
		removeEmptyNode: function(node: Node){
			if(node && node.children){
				for(let i = 0; i < node.children.length; i++){
					if(node.children[i] && node.children[i].name === ""){
						node.children.splice(i, 1);
					}
				}
			}
		},
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
		addToFile: function(tree: Node){
			this.filesById.forEach(fileById => {
				if(tree.children){
					tree.children.forEach(child => {
						if(fileById.id === child.id){
							const node = {id: ++this.id, name: fileById.file.name};
							let parent = child;
							if(fileById.file.path === ""){
								parent = tree;
							}
							const isExists = this.getChild(parent, fileById.file.name);
							if(parent && !isExists){
								if(!parent.children){
									parent.children = [node];
								}else{
									parent.children.push(node);
								}
							}
						}
						this.addToFile(child);
					});
				}
			});
		},
		addToTree: function(file: GeneratedFile){
			const path = file.path.split('/');
			let currentNode: Node = this.tree;
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
			this.filesById.push({id: currentNode.id, file: file});
		},
		getFile: function(node: Node){
			for(let i = 0; this.filesById.length; i++){
				if(this.filesById[i].file.name === node.name){
					return this.filesById[i].file;
				}
			}
		},
		selectFile: function (node: Node[]){
			if(node[0]){
				this.$emit("select-file", this.getFile(node[0]));
			}
		},
  },
});
</script>
