import api from '@/api';
import { prettyPrint, validateJson } from './PrettyPrint';
import { delay, toCamelCase } from './Helper';
import { ViewModel } from './ViewModel';
import store from '../store/index';
import { GenerateResponse } from '@/models/GenerateResponse';
import { CRC32 } from 'crc_32_ts';
import { Project } from '@/models/Project';

export interface Command {
    name: string;
    viewModel: ViewModel;
	icon: string;
	hoverText: string;
	color?: string;
	disabled: boolean;
    progress: boolean;
    action: () => void;
}

function getProjectByName(projectName: string): Project | null{
    for(const i in store.state.projects.items){
        if(projectName === store.state.projects.items[i].name){
            return store.state.projects.items[i];
        }
    }
    return null;
}

const viewModel = new ViewModel ();

export function setSnackbar(this: any, type: string, text: string, timeout: number){
    console.log(text);
    viewModel.snackbar.dismissible = true;
    viewModel.snackbar.timeout = timeout;
    viewModel.snackbar.type = type;
    viewModel.snackbar.text = text;
    viewModel.snackbar.visible = true;
}

export class ProjectSettingsCommand implements Command {
    name = 'project-settings';
    viewModel: ViewModel;
    icon = 'mdi-cog';
    hoverText = 'Project settings';
    public get disabled(): boolean {
        return !this.viewModel.generateLoading;
    }
    progress = false;
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }

    action() {
        this.viewModel.projectSettings = true;
    }
    
}

export class UndoCommand implements Command {
    name = 'undo';
    viewModel: ViewModel;
    icon = 'mdi-undo';
    hoverText = 'Rollback to the last generated state';
    public get disabled(): boolean {
        return !((this.viewModel.undoStack.length() < 2 && this.viewModel.isJsonPristine) || this.viewModel.generateLoading);
    }
    progress = false;
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }

    action() {
        if (this.viewModel.isPristine) {
            this.viewModel.undoStack.pop();
        }
        const top = this.viewModel.undoStack.top();
        if(top) {
          this.viewModel.activeProject.json = top.content;
        }
        setSnackbar('info', 'Everything restored to its previous generated state', 5000);
    }
}

export class SaveCommand implements Command {
    name = 'save';
    viewModel: ViewModel;
    icon = 'mdi-floppy';
    hoverText = 'Save';
    public get disabled(): boolean {
        return !(this.viewModel.saveDisabled || this.viewModel.generateLoading);
    }
    progress = false;
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }

    async action() {
        if(this.viewModel.activeProject.name){
          const project = getProjectByName(this.viewModel.activeProject.name);
          if (!project && this.viewModel.activeProject.id === -1) {
            setSnackbar('success', 'The new project was successfully created!', 5000);
            this.viewModel.crc32Saved = this.viewModel.crc32ForSaving;
            this.viewModel.activeProject.id = 0;
            if(store.state.auth.user){
                this.viewModel.activeProject.ownerId = store.state.auth.user.id;
            }
            this.viewModel.activeProject = await store.dispatch('projects/addProject', this.viewModel.activeProject);
          } else if(project && project.id !== this.viewModel.activeProject.id) {
            setSnackbar('error', 'This name is already in use, please enter another name!', 5000);
          } else {
            this.viewModel.crc32Saved = this.viewModel.crc32ForSaving;
            setSnackbar('success', 'Project updated successfully!', 5000);
            await store.dispatch('projects/updateProject', this.viewModel.activeProject);
            setSnackbar('success', 'Project updated successfully!', 5000);
          }
        }else{
          setSnackbar('error', 'This name is incorrect!', 5000);
        }
    }
}

export class PrettyPrintCommand implements Command {
    name = 'pretty-print';
    viewModel: ViewModel;
    icon = 'mdi-format-align-left';
    hoverText = 'Formatting';
    public get disabled(): boolean {
        return !(this.viewModel.activeProject.json === '' || this.viewModel.generateLoading);
    }
    progress = false;
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }

    action() {
        const result = validateJson(this.viewModel.activeProject.json);
        if (result.error) {
          this.viewModel.jsonErrors.push({line: result.line, color: 'rgba(255, 0, 0, 0.3)'});
          setSnackbar('orange darken-2', result.message, -1);
        }
        this.viewModel.activeProject.json = prettyPrint(this.viewModel.activeProject.json);
    }
}

export class GenerateCommand implements Command {
    name = 'generate';
    viewModel: ViewModel;
    icon = 'mdi-arrow-right-bold';
    hoverText = 'Generate';
    public get disabled(): boolean {
        return !(this.viewModel.isPristine || this.viewModel.generateLoading || this.viewModel.activeProject.name === '');
    }
    public get progress(): boolean {
        return this.viewModel.generateLoading;
    }
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }
    
    async action() {
      this.viewModel.generateLoading = true;
      const generateResult: GenerateResponse = await api.generate({
        data: this.viewModel.activeProject.json,
        nameSpace: toCamelCase(this.viewModel.activeProject.name),
        backend: this.viewModel.activeProject.backend,
        frontend: this.viewModel.activeProject.frontend
      });
      if(generateResult.errorMessage){
        setSnackbar('orange darken-2', generateResult.errorMessage, -1);
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
    }
}

export class CompareCommand implements Command {
    name = 'set-compare';
    viewModel: ViewModel;
    icon = 'mdi-file-compare';
    public get hoverText(): string {
        if(!this.viewModel.isCompare){
            return 'Show Changes: On';
        }
        return 'Show Changes: Off';
    }
    public get color(): string {
        if(!this.viewModel.isCompare){
            return 'rgba(255, 255, 255, 0.2)';
        }
        return '#ffffff';
    }
    
    disabled = true;
    progress = false;
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }
    
    action() {
        if(this.viewModel.isCompare){
            this.viewModel.isCompare = false;
            this.viewModel.showChanges = false;
            this.viewModel.highlightedDifferences = [];
        }else{
            this.viewModel.isCompare = true;
            this.viewModel.showChanges = true;
            this.viewModel.setHighlightedDifferences();
        }
    }
}
export class DownloadCommand implements Command {
    name = 'download';
    viewModel: ViewModel;
    icon = 'mdi-download';
    hoverText = 'Download';
    public get disabled(): boolean {
        return !(!this.viewModel.isPristine || this.viewModel.downLoading || this.viewModel.activeProject.name === '');
    }
    public get progress(): boolean {
        return this.viewModel.downLoading;
    }
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }
    
    async action() {
        if(!this.viewModel.downLoading){
            this.viewModel.downLoading = true;
            await Promise.all([
            delay(3000),
            api.download({
                data: this.viewModel.activeProject.json,
                nameSpace: toCamelCase(this.viewModel.activeProject.name),
                backend: this.viewModel.activeProject.backend,
                frontend: this.viewModel.activeProject.frontend
            })
        ]);
        this.viewModel.downLoading = false;
      }
    }
}
