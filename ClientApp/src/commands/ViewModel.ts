import { GeneratedFile } from '@/models/GeneratedFile';
import { Project } from '@/models/Project';
import { getJsonLength } from '@/utils/Helper';
import { prettyPrint, validateJson } from '@/utils/PrettyPrint';
import { Compare } from '@/utils/TextCompare';
import { CRC32 } from 'crc_32_ts';
import { UndoStack } from '../utils/UndoStack';
import { Command } from './Command';

export class ViewModel {
    generatedFiles = Array<GeneratedFile>();
    previousFiles = Array<GeneratedFile>();
    undoStack = new UndoStack();
    crc32ProjectName = CRC32.str('My Project');
    crc32Saved = 0;
    activeProject: Project = {id: -1, ownerId: -1, name: 'My Project', json: '', backend: 'ASP.NET', frontend: 'Vue 2 + JS'};
    newProjectDialog = false;
    activeFile: GeneratedFile = {name: '', path: '', content: ''};
    jsonErrors = Array<{line: number; color: string}>();
    backend = 'ASP.NET';
    frontend = 'Vue 2 + JS';
    generateLoading = false;
    isCompare = true;
    showChanges = true;
    highlightedDifferences: Array<{line: number; color: string}> = [];
    downLoading = false;
    projectSettings = false;
    snackbar = {
        dismissible: true,
        visible: false,
        type: '',
        icon: 'mdi-alert-circle',
        text: '',
        timeout: 5000,
    }
    public get isPristine(): boolean {
        const top = this.undoStack.top();
        if(top){
            if((top.crc32 === CRC32.str(this.activeProject.json)) &&
                (this.crc32ProjectName === CRC32.str(this.activeProject.name)) &&
                (this.backend === this.activeProject.backend) &&
                (this.frontend === this.activeProject.frontend)
            ){
                return true;
            }
        }
        return false;
    }
    public get isJsonPristine(): boolean {
        const top = this.undoStack.top();
        if(top){
            if(top.crc32 === CRC32.str(this.activeProject.json)){
            return true;
            }
        }
        return false;
    }
    public get crc32ForSaving(): number {
      return CRC32.str(this.activeProject.name + this.activeProject.json);
    }
    public get saveDisabled(): boolean {
        return this.crc32Saved === this.crc32ForSaving;
    }
    
    public setSnackbar(type: string, text: string, timeout: number){
        this.snackbar.dismissible = true;
        this.snackbar.timeout = timeout;
        this.snackbar.type = type;
        this.snackbar.text = text;
        this.snackbar.visible = true;
    }

    public hideSnackbar(){
        this.snackbar.visible = false;
    }

    
    public setHighlightedDifferences(){
        if(this.previousFiles.length > 0 && this.isCompare){
            this.highlightedDifferences = [];
            if(this.showChanges){
                for(let i = 0; i < this.previousFiles.length; i++){
                    if(this.previousFiles[i].name === this.activeFile.name && this.previousFiles[i].path === this.activeFile.path){
                        const compare = new Compare(this.activeFile.content.split('\n'), this.previousFiles[i].content.split('\n'));
                        const changes = compare.getChanges();
                        changes.forEach(v =>{
                            this.highlightedDifferences.push({ line : v, color:'rgba(0, 111, 197, 0.3)' })
                        })
                        break;
                    }
                }
            }
        }
    }
        
    public setSelectedFile (fileName: string, filePath: string) {
        for (let i = 0; i < this.generatedFiles.length; i++) {
            if (this.generatedFiles[i].name === fileName && this.generatedFiles[i].path === filePath) {
                this.activeFile = this.generatedFiles[i];
                break;
            }
        }
    }
    public setActiveFile (){
        if(!this.activeFile.name){
        this.setSelectedFile('restapi.yml', '');
        }else{
        this.setSelectedFile(this.activeFile.name, this.activeFile.path);
        }
        this.setHighlightedDifferences();
    }

    public validateAndGenerate (generateCommand: Command) {
        this.jsonErrors = [];
        const result = validateJson(this.activeProject.json);
        if(!result.error) {
            prettyPrint(this.activeProject.json);
            const jsonLength = getJsonLength(this.activeProject.json);
            if(jsonLength > 2000) {
                this.setSnackbar('orange darken-2', `Exceeded character limit: ${jsonLength} / 2000`, -1);
                return;
            }
            if(generateCommand.action){
                generateCommand.action();
            }
        } else {
            this.jsonErrors.push({line: result.line, color: 'rgba(255, 0, 0, 0.3)'});
            this.setSnackbar('orange darken-2', result.message, -1);
        }
    }
}