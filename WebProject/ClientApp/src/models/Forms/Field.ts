export enum FieldType{
    Text,
    Textarea,
    Checkbox,
    Password,
    ColorPicker,
}

export interface Field {
    property: string;
    placeholder: string;
    type: FieldType;
    validation: string;
}