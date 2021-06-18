import { prettyPrint, validateJson } from "./PrettyPrint";
import { ViewModel } from "./ViewModel";

export interface Command {
    name: string;
    viewModel: ViewModel;
	icon: string;
	hoverText: string;
	disabled: boolean;
    progress: boolean;
    action: () => void
}

export class PrettyPrintCommand implements Command {
    name = 'pretty-print';
    viewModel: ViewModel;
    icon = 'mdi-format-align-left';
    hoverText = 'Formatting';
    public get disabled() : boolean {
        return this.viewModel.activeProject.json === ''
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