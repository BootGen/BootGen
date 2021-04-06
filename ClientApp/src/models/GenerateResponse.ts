import { GeneratedFile } from './GeneratedFile';

export interface GenerateResponse {
    success: boolean;
    errorMessage: string;
    errorLine: number|null;
    generatedFiles: GeneratedFile[];
}
