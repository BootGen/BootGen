
export interface AppError {
    kind: string;
    type?: string;
    lineNumber?: number;
    columnNumber?: number;
    fileName?: string;
    message?: string;
    info?: string;
    stackTrace?: string;
}