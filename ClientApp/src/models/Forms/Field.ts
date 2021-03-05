export enum FieldType{
    Text,
    Textarea,
    Checkbox,
    Password,
    ColorPicker,
    Radio,
}

export interface Field {
    property: string;
    placeholder: string;
    type: FieldType;
    validation: string;
    data?: any;
}