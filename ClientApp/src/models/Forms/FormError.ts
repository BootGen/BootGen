export enum ErrorType{
    Required,
    Min,
    Email,
    Password
}

export interface FormError {
    type: ErrorType;
    message: string;
}