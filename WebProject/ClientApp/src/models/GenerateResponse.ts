import { GeneratedFile } from './GeneratedFile';

export interface GenerateResponse {
    success: Boolean;
    errorMessage: string;
    generatedFiles: GeneratedFile[];
}