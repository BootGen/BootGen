import { CommandType } from './CommandStore';
import { ViewModel } from './ViewModel';

export interface Command {
    name: string;
    type: CommandType;
    viewModel: ViewModel;
	icon: string;
	text: string;
	disabled: boolean;
    progress: boolean;
    action: () => void;
}
