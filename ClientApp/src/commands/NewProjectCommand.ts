
import { Command } from './Command';
import { ViewModel } from './ViewModel';

export class NewProjectCommand implements Command {
    name = 'new-project';
    viewModel: ViewModel;
    icon = 'mdi-plus';
    text = 'New project';
    isCircleShape = false;
    public get disabled(): boolean {
        return this.viewModel.generateLoading;
    }
    progress = false;
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }

    action() {
        this.viewModel.newProjectDialog = true;
    }
}