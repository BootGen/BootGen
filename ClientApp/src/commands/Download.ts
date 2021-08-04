import api from '@/api';
import { Command } from './Command';
import { CommandType } from './CommandStore';
import { ViewModel } from './ViewModel';
import { toPascalCase } from '../utils/Helper';

export class DownloadCommand implements Command {
    name = 'download';
    type = CommandType.Download;
    viewModel: ViewModel;
    icon = 'mdi-download';
    text = 'Download';
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
            await api.download({
                data: this.viewModel.activeProject.json,
                nameSpace: toPascalCase(this.viewModel.activeProject.name),
                backend: this.viewModel.activeProject.backend,
                frontend: this.viewModel.activeProject.frontend
            });
        this.viewModel.downLoading = false;
      }
    }
}
