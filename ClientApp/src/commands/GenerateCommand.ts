import api from '@/api';
import { Command } from './Command';
import { ViewModel } from './ViewModel';
import { prettyPrint, validateJson } from '../utils/PrettyPrint';
import { getJsonLength, toCamelCase } from '../utils/Helper';
import store from '../store/index';
import { GenerateResponse } from '@/models/GenerateResponse';
import { CRC32 } from 'crc_32_ts';


export class GenerateCommand implements Command {
    name = 'generate';
    viewModel: ViewModel;
    icon = 'mdi-arrow-right-bold';
    text = 'Generate';
    public get disabled(): boolean {
        return this.viewModel.isPristine || this.viewModel.generateLoading || this.viewModel.activeProject.name === '';
    }
    public get progress(): boolean {
        return this.viewModel.generateLoading;
    }
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }
    
    async action() {
        this.viewModel.jsonErrors = [];
        const result = validateJson(this.viewModel.activeProject.json);
        if(!result.error) {
            prettyPrint(this.viewModel.activeProject.json);
            const jsonLength = getJsonLength(this.viewModel.activeProject.json);
            if(jsonLength > 2000) {
                this.viewModel.setSnackbar('orange darken-2', `Exceeded character limit: ${jsonLength} / 2000`, -1);
                return;
            }

            this.viewModel.generateLoading = true;

            const generateResult: GenerateResponse = await api.generate({
              data: this.viewModel.activeProject.json,
              nameSpace: toCamelCase(this.viewModel.activeProject.name),
              backend: this.viewModel.activeProject.backend,
              frontend: this.viewModel.activeProject.frontend
            });
            if(generateResult.errorMessage){
                this.viewModel.setSnackbar('orange darken-2', generateResult.errorMessage, -1);
              if(generateResult.errorLine !== null){
                this.viewModel.jsonErrors.push({line: generateResult.errorLine-1, color: 'rgba(255, 0, 0, 0.3)'});
              }
              this.viewModel.generateLoading = false;
              return;
            }
            this.viewModel.previousFiles = [...this.viewModel.generatedFiles];
            this.viewModel.generatedFiles = generateResult.generatedFiles;
            store.commit('projects/setLastProject', this.viewModel.activeProject);
            store.commit('projects/setLastGeneratedFiles', this.viewModel.generatedFiles);
            this.viewModel.crc32ProjectName = CRC32.str(this.viewModel.activeProject.name);
            if(this.viewModel.activeProject.backend === this.viewModel.backend && this.viewModel.activeProject.frontend === this.viewModel.frontend){
              this.viewModel.showChanges = true;
              this.viewModel.highlightedDifferences = [];
            }else{
              this.viewModel.showChanges = false;
            }
            this.viewModel.backend = this.viewModel.activeProject.backend;
            this.viewModel.frontend = this.viewModel.activeProject.frontend;
            if(this.viewModel.undoStack.top()?.content !== this.viewModel.activeProject.json){
              this.viewModel.undoStack.push(this.viewModel.activeProject.json);
            }
            this.viewModel.setActiveFile();
            this.viewModel.generateLoading = false;
        } else {
            this.viewModel.jsonErrors.push({line: result.line, color: 'rgba(255, 0, 0, 0.3)'});
            this.viewModel.setSnackbar('orange darken-2', result.message, -1);
        }  
    }
}