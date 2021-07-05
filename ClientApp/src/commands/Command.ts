import { ViewModel } from './ViewModel';

export interface Command {
    name: string;
    viewModel: ViewModel;
	icon: string;
	text: string;
    isCircleShape: boolean;
	color?: string;
	disabled: boolean;
    progress: boolean;
    action: () => void;
}
