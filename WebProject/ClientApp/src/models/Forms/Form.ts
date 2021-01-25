import { Button } from './Button';
import { Field } from './Field';
import { FormError } from './FormError';

export interface Form {
    title: string;
    model: any;
    fields: Field[];
    submit: Button;
    cancel?: Button;
    errors?: FormError[];
}