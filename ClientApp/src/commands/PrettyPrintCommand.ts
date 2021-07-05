import { Command } from './Command';
import { ViewModel } from './ViewModel';
import { prettyPrint, validateJson } from '../utils/PrettyPrint';

export class PrettyPrintCommand implements Command {
    name = 'pretty-print';
    viewModel: ViewModel;
    icon = 'mdi-format-align-left';
    text = 'Formatting';
    isCircleShape = true;
    public get disabled(): boolean {
        return this.viewModel.activeProject.json === '' || this.viewModel.generateLoading;
    }
    progress = false;
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }

    action() {
        const result = validateJson(this.viewModel.activeProject.json);
        if (result.error) {
          this.viewModel.jsonErrors.push({line: result.line, color: 'rgba(255, 0, 0, 0.3)'});
          this.viewModel.setSnackbar('orange darken-2', result.message, -1);
        }
        this.viewModel.activeProject.json = prettyPrint(this.viewModel.activeProject.json);
    }
}