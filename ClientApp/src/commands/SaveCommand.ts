import { Command } from './Command';
import { ViewModel } from './ViewModel';
import { Project } from '@/models/Project';
import store from '../store/index';

function getProjectByName(projectName: string): Project | null{
    for(const i in store.state.projects.items){
        if(projectName === store.state.projects.items[i].name){
            return store.state.projects.items[i];
        }
    }
    return null;
}  

export class SaveCommand implements Command {
    name = 'save';
    viewModel: ViewModel;
    icon = 'mdi-floppy';
    text = 'Save';
    public get disabled(): boolean {
        return this.viewModel.saveDisabled || this.viewModel.generateLoading;
    }
    progress = false;
    constructor(viewModel: ViewModel) {
        this.viewModel = viewModel;
    }

    async action() {
        if(this.viewModel.activeProject.name){
          const project = getProjectByName(this.viewModel.activeProject.name);
          if (!project && this.viewModel.activeProject.id === -1) {
            this.viewModel.setSnackbar('success', 'The new project was successfully created!', 5000);
            this.viewModel.crc32Saved = this.viewModel.crc32ForSaving;
            this.viewModel.activeProject.id = 0;
            if(store.state.auth.user){
                this.viewModel.activeProject.ownerId = store.state.auth.user.id;
            }
            this.viewModel.activeProject = await store.dispatch('projects/addProject', this.viewModel.activeProject);
          } else if(project && project.id !== this.viewModel.activeProject.id) {
            this.viewModel.setSnackbar('error', 'This name is already in use, please enter another name!', 5000);
          } else {
            this.viewModel.crc32Saved = this.viewModel.crc32ForSaving;
            this.viewModel.setSnackbar('success', 'Project updated successfully!', 5000);
            await store.dispatch('projects/updateProject', this.viewModel.activeProject);
            this.viewModel.setSnackbar('success', 'Project updated successfully!', 5000);
          }
        }else{
            this.viewModel.setSnackbar('error', 'This name is incorrect!', 5000);
        }
    }
}
