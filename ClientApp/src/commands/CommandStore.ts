import { Command } from './Command';
import { CompareCommand } from './Compare';
import { DownloadCommand } from './Download';
import { GenerateCommand } from './Generate';
import { OpenNewProjectDialogCommand } from './OpenNewProjectDialog';
import { OpenSettingsDialogCommand } from './OpenSettingsDialog';
import { PrettyPrintCommand } from './PrettyPrint';
import { SaveCommand } from './Save';
import { UndoCommand } from './Undo';
import { ViewModel } from './ViewModel';

export enum CommandType {
    Compare,
    Download,
    Generate,
    OpenNewProjectDialog,
    OpenSettingsDialog,
    PrettyPrint,
    Save,
    Undo
}

export class CommandStore {
    viewModel: ViewModel;
    commands: Command[];
    gtag: any;
    public command (commandType: CommandType): Command | null {
        for(let i = 0; i < this.commands.length; i++){
            if(this.commands[i].type === commandType){
                return this.commands[i];
            }
        }
        return null;
    }

    constructor(viewModel: ViewModel, gtag: any) {
        this.viewModel = viewModel;
        this.commands = [];
        this.commands.push(new CompareCommand(this.viewModel));
        this.commands.push(new DownloadCommand(this.viewModel));
        this.commands.push(new GenerateCommand(this.viewModel));
        this.commands.push(new OpenNewProjectDialogCommand(this.viewModel));
        this.commands.push(new OpenSettingsDialogCommand(this.viewModel));
        this.commands.push(new PrettyPrintCommand(this.viewModel));
        this.commands.push(new SaveCommand(this.viewModel));
        this.commands.push(new UndoCommand(this.viewModel));
        this.gtag = gtag;
    }

    do(commandType: CommandType) {
        for(let i = 0; i < this.commands.length; i++){
            if(this.commands[i].type === commandType){
                this.gtag?.event(this.commands[i].name);
                this.commands[i].action();
                break;
            }
        }
    }
}