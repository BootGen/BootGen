
import { Command } from './Command';
import { CommandType } from './CommandStore';
import { ViewModel } from './ViewModel';

export class OpenNewProjectDialogCommand implements Command {
    name = 'new-project';
    type = CommandType.OpenNewProjectDialog;
    viewModel: ViewModel;
    icon = 'mdi-plus';
    text = 'New project';
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