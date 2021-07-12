import { extend } from 'vee-validate';
import { required, email, min } from 'vee-validate/dist/rules';

extend('required', {
    ...required,
    message: '{_field_} can not be empty',
});

extend('min', {
    ...min,
    message: '{_field_} may not be less than {length} characters',
});

extend('username', {
    validate(value) {
        return (/^(?![_. ])(?!.*[_. ]{2})[A-zÀ-ű0-9._ ]+(?<![. ])$/.test(value));
    },
    message: 'User name must be valid',
});

extend('email', {
    ...email,
    message: 'Email must be valid',
});

extend('password', {
    params: ['target'],
    validate(value, { target }: any) {
        return value === target;
    },
    message: 'Password confirmation does not match',
});

extend('oldPassword', {
    params: ['target'],
    validate(value, { target }: any) {
        return !(value === target);
    },
    message: 'Password cannot match the current one',
});
