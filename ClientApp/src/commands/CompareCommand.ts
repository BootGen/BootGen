import { Command } from './Command';
import { ViewModel } from './ViewModel';

export class CompareCommand implements Command {
    name = 'set-compare';
    viewModel: ViewModel;
    public get icon(): string {
        if(!this.viewModel.isCompare){
            return 'mdi-file-document';
        }
        return 'mdi-file-compare';
    }
    public get text(): string {
        if(!this.viewModel.isCompare){
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