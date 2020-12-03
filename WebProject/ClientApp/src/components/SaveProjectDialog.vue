<template>
  <v-dialog v-model="dialog" persistent max-width="500px">
    <template v-slot:activator="{ on, attrs }">
      <div class="d-flex align-center pa-1" v-bind="attrs" v-on="on">
        <v-btn class="mr-4" color="primary" small>Save</v-btn>
      </div>
    </template>
    <v-card>
      <v-container>
        <h2 class="text-center">Create a new tag</h2>
        <FormComponent :form="form"></FormComponent>
      </v-container>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from "vue";
import { FieldType } from '../models/Forms/Field';
import { Form } from "../models/Forms/Form";
import FormComponent from "@/components/FormComponent.vue";

export default Vue.extend({
  props: [
    "project"
  ],
  components: {
    FormComponent,
  },
  computed: {
    form: function (): Form {
      return {
        title: "Save project",
        model: {name: this.project.name},
        fields: [
          {property: "name", placeholder: "Name", type: FieldType.Text, validation: "required|min:3"},
        ],
        submit: {name: "Save", color: "primary", action: this.save},
        cancel: {name: "Cancel", color: "red--text", action: this.close}
      }
    }
  },
  data: function () {
    return {
      dialog: false,
    };
  },
  methods: {
    close: function () {
      this.dialog = false;
    },
    save: async function () {
      if(!this.project.id){
        this.project.id = 0;
        this.project.name = this.form.model.name;
        this.project.ownerId = this.$root.$data.user.id;
        await this.$store.dispatch("addProject", this.project);
      }else{
        await this.$store.dispatch("updateProject", this.project);
      }
      this.close();
    },
  },
});
</script>