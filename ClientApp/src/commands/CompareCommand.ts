import { Command } from './Command';
import { ViewModel } from './ViewModel';

export class CompareCommand implements Command {
    name = 'set-compare';
    viewModel: ViewModel;
    icon = 'mdi-file-compare';
    public get text(): string {
        if(!this.viewModel.isCompare){
            return 'Show Changes: On';
        }
        return 'Show Changes: Off';
    }
    isCircleShape = true;
    public get color(): string {
        if(!this.viewModel.isCompare){
            return 'rgba(255, 255, 255, 0.2)';
        }
        return '#ffffff';
    }
    
    disabled = true;
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