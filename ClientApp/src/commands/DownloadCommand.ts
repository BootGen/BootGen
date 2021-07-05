import api from '@/api';
import { Command } from './Command';
import { ViewModel } from './ViewModel';
import { delay, toCamelCase } from '../utils/Helper';

export class DownloadCommand implements Command {
    name = 'download';
    viewModel: ViewModel;
    icon = 'mdi-download';
    text = 'Download';
    isCircleShape = true;
    public get disabled(): boolean {
        return !this.viewModel.isPristine || this.viewModel.downLoading || this.viewModel.activeProject.name === '';
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
