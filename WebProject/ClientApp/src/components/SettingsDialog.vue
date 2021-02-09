<template>
  <v-dialog v-model="dialog" persistent max-width="500px">
    <template v-slot:activator="{ on, attrs }">
			<v-btn class="ml-2" color="white" elevation="1" fab small v-bind="attrs" v-on="on">
				<v-icon color="primary">mdi-cog</v-icon>
			</v-btn>
    </template>
    <v-card>
      <v-container>
        <h2 class="text-center">Settings</h2>
        <FormComponent :form="form"></FormComponent>
      </v-container>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from "vue";
import { FieldType } from '../models/Forms/Field';
import { Form } from "../models/Forms/Form";
import FormComponent from "../components/FormComponent.vue";
import { GenerateRequest } from "../models/GenerateRequest"
export default Vue.extend({
  components: {
    FormComponent,
  },
  computed: {
    form: function (): Form {
      const settings = {
        namespace: this.$store.state.projectSettings.item.nameSpace,
        engine: "SQLite",
        connection: "",
        generateTS: this.$store.state.projectSettings.item.generateClient};
      return {
        title: "Settings",
        model: settings,
        fields: [
          {property: "namespace", placeholder: "Namespace", type: FieldType.Text, validation: "min:3|required"},
          {property: "engine", placeholder: "Database engine:", data: ["SQLite", "MySql", "Postgres", "MSSql"], type: FieldType.Radio, validation: "required"},
          {property: "connection", placeholder: "Connection", type: FieldType.Text, validation: ""},
          {property: "generateTS", placeholder: "Generate typescript files", type: FieldType.Checkbox, validation: ""},
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
      const data: GenerateRequest = {
        data: this.$store.state.projectSettings.item.data,
        generateClient: this.form.model.generateTS,
        nameSpace: this.form.model.namespace
      };
      await this.$store.dispatch("updateProjectSettings", data);
      this.close();
    },
  },
});
</script>
