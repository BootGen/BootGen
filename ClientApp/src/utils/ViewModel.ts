import { GeneratedFile } from '@/models/GeneratedFile';
import { Project } from '@/models/Project';
import { CRC32 } from 'crc_32_ts';
import { UndoStack } from './UndoStack';

export class ViewModel {
    generatedFiles = Array<GeneratedFile>();
    previousFiles = Array<GeneratedFile>();
    undoStack = new UndoStack();
    crc32ProjectName = CRC32.str('My Project');
    crc32Saved = 0;
    activeProject: Project = {id: -1, ownerId: -1, name: 'My Project', json: '', backend: 'ASP.NET', frontend: 'Vue 2 + JS'};
    activeFile: GeneratedFile = {name: '', path: '', content: ''};
    jsonErrors = Array<{line: number; color: string}>();
    setSnackbar: (type: string, text: string, timeout: number) => void = function (type: string, text: string, timeout: number) {console.log('setSnackbar')};
}