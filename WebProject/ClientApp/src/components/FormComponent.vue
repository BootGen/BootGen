<template>
  <div>
    <ValidationObserver v-if="form && form.fields" v-slot="{ invalid }">
      <ValidationProvider v-slot="{ errors }" v-for="(field, indexi) in form.fields" :key="indexi" :name="field.placeholder" :rules="field.validation">
        <!-- Text -->
        <v-text-field v-if="field.type === FieldType.Text"
          v-model="form.model[field.property]"
          :label="field.placeholder"
          :error-messages="errors"
          type="text"
          required
        ></v-text-field>
        <!-- Textarea -->
        <v-textarea v-if="field.type === FieldType.Textarea"
          v-model="form.model[field.property]"
          :label="field.placeholder"
          rows="2"
          :error-messages="errors"
          required
        ></v-textarea>
        <!-- Checkbox -->
        <v-checkbox v-if="field.type === FieldType.Checkbox"
          v-model="form.model[field.property]"
          :label="field.placeholder"
          required
        ></v-checkbox>
        <!-- Password -->
        <v-text-field v-if="field.type === FieldType.Password"
          v-model="form.model[field.property]"
          :label="field.placeholder"
          :error-messages="errors"
          type="password"
          required
        ></v-text-field>
        <!-- ColorPicker -->
        <v-text-field class="mb-8" v-if="field.type === FieldType.ColorPicker" v-model="form.model[field.property]" hide-details readonly>
          <template v-slot:append>
            <v-menu v-model="menu" :close-on-content-click="false">
              <template v-slot:activator="{ on }">
                <div class="rounded-lg pa-4 ma-1" v-bind:style="{ background: form.model[field.property] }" v-on="on" />
              </template>
              <v-card>
                <v-card-text class="pa-0">
                  <v-color-picker v-model="form.model[field.property]" flat />
                </v-card-text>
              </v-card>
            </v-menu>
          </template>
        </v-text-field>
        <!--Radio-->
        <v-radio-group v-if="field.type === FieldType.Radio" v-model="form.model[field.property]">
          <label>{{ field.placeholder }}</label>
          <v-radio v-for="n in field.data" :key="n" :label="n" :value="n"></v-radio>
        </v-radio-group>
      </ValidationProvider>
      <!-- Buttons -->
      <div class="d-flex">
        <v-btn class="mr-4" :color="form.submit.color" large :disabled="invalid" @click="form.submit.action">
          {{ form.submit.name }}
        </v-btn>
        <v-btn v-if="form.cancel" class="mr-4" :color="form.cancel.color" large @click="form.cancel.action">
          {{ form.cancel.name }}
        </v-btn>
      </div>
    </ValidationObserver>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { FieldType } from '../models/Forms/Field';
import { ErrorType, FormError } from '../models/Forms/FormError';
import { required, min, email } from 'vee-validate/dist/rules';
import { extend, ValidationObserver, ValidationProvider } from 'vee-validate';

export default Vue.extend({
  created: function() {
    extend('required', {
      ...required,
      message: this.validationMessage(ErrorType.Required),
    });

    extend('min', {
      ...min,
      message: this.validationMessage(ErrorType.Min),
    });

    extend('email', {
      ...email,
      message: this.validationMessage(ErrorType.Email),
    });

    extend('password', {
      params: ['target'],
      validate(value, {target}: any) {
        return value === target;
      },
      message: this.validationMessage(ErrorType.Password),
    });
  },
  props: [
    "form"
  ],
  components: {
    ValidationObserver,
    ValidationProvider,
  },
  data: function () {
    return {
      menu: false,
      FieldType: FieldType,
    };
  },
  methods: {
    validationMessage: function (type: ErrorType): string{
      let message = "";
      if(this.form.errors){
        this.form.errors.map((error: FormError) => {
          if(type === error.type){
            message = error.message;
          }
        });
      }
      if(message){
        return message;
      }else{
        switch(type){
          case ErrorType.Required:
            return "{_field_} cannot be empty";
          case ErrorType.Min:
            return "{_field_} may not be less than {length} characters";
          case ErrorType.Email:
            return "Email must be valid";
          case ErrorType.Password:
            return "Password confirmation does not match";
          default:
            return "untreated";
        }
      }
    }
  }
});
</script>