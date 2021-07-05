import { Command } from './Command';
import { ViewModel } from './ViewModel';

export class UndoCommand implements Command {
    name = 'undo';
    viewModel: ViewModel;
    icon = 'mdi-undo';
    text = 'Rollback to the last generated state';
    public get disabled(): boolean {
        return (this.viewModel.undoStack.length() < 2 && this.viewModel.isJsonPristine) || this.viewModel.generateLoading;
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
        this.viewModel.setSnackbar('info', 'Everything restored to its previous generated state', 5000);
    }
}