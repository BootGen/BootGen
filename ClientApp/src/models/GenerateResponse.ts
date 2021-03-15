import { GeneratedFile } from './GeneratedFile';

export interface GenerateResponse {
    success: Boolean;
    errorMessage: string;
    errorLine: number|null;
    generatedFiles: GeneratedFile[];
}