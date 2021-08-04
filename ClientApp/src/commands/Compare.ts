import { Command } from './Command';
import { CommandType } from './CommandStore';
import { ViewModel } from './ViewModel';

export class CompareCommand implements Command {
    name = 'set-compare';
    type = CommandType.Compare;
    viewModel: ViewModel;
    public get icon(): string {
        if(!this.viewModel.showDifferences){
            return 'mdi-file-document';
        }
        return 'mdi-file-compare';
    }
    public get text(): string {
        if(!this.viewModel.showDifferences){
            return 'Show Changes: On';
        }
        return 'Show Changes: Off';
    }
    
    disabled = false;
    progress = false;
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }
    
    action() {
        if(this.viewModel.showDifferences){
            this.viewModel.showDifferences = false;
            this.viewModel.showChanges = false;
            this.viewModel.highlightedDifferences = [];
        }else{
            this.viewModel.showDifferences = true;
            this.viewModel.showChanges = true;
            this.viewModel.setHighlightedDifferences();
        }
    }
}