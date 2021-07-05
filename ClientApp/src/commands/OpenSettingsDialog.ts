import { Command } from './Command';
import { CommandType } from './CommandStore';
import { ViewModel } from './ViewModel';

export class OpenSettingsDialogCommand implements Command {
    name = 'project-settings';
    type = CommandType.OpenSettingsDialog;
    viewModel: ViewModel;
    icon = 'mdi-cog';
    text = 'Project settings';
    public get disabled(): boolean {
        return this.viewModel.generateLoading;
    }
    progress = false;
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }

    action() {
        this.viewModel.projectSettings = true;
    }
}